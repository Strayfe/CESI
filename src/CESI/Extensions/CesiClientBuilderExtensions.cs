using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CESI.APIs;
using CESI.AuthNZ;
using CESI.Configuration;
using CESI.Handlers;
using CESI.StaticData;
using CESI.StaticData.DbContexts;
using CESI.StaticData.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CESI.Extensions
{
    public static class CesiClientBuilderExtensions
    {
        public static CesiClientBuilder AddRequiredPlatformServices(this CesiClientBuilder builder)
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //builder.Services.AddOptions();
            builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<CesiConfiguration>>().Value);
            builder.Services.AddHttpClient();
            
            return builder;
        }

        public static CesiClientBuilder AddCesiCoreServices(this CesiClientBuilder builder)
        {
            builder.Services.AddScoped<CesiHttpHandler>();

            return builder;
        }

        public static CesiClientBuilder AddMySqlStaticData(this CesiClientBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var cesiConfiguration = serviceProvider.GetRequiredService<IOptions<CesiConfiguration>>().Value;
            
            builder.Services.AddDbContext<StaticDataContext>(options =>
            {
                options.UseMySQL(cesiConfiguration.DatabaseOptions?.ConnectionString);
            });
            
            builder.Services.TryAddScoped<IStaticDataImportHandler, MySqlStaticDataImportHandler>();
            return builder;
        }

        public static CesiClientBuilder AddCesiClientAuthentication(this CesiClientBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var esiOptions = serviceProvider.GetRequiredService<IOptions<CesiConfiguration>>().Value;
            
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme = "EVE_SSO";
                })
                .AddCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        
                        {
                            if (context.Principal.Identity.IsAuthenticated)
                            {
                                // TODO: move to Custom CookieAuthenticationEvents class
                                var tokens = context.Properties.GetTokens().ToList();
                                var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
                                var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
                                var expiration = tokens.FirstOrDefault(t => t.Name == "expires_at");
                                var expires = DateTime.Parse(expiration.Value, DateTimeFormatInfo.InvariantInfo);
                                
                                if (expires < DateTime.Now && 
                                    !string.IsNullOrWhiteSpace(esiOptions.ClientId) &&
                                    !string.IsNullOrWhiteSpace(esiOptions.ClientSecret))
                                {
                                    var byteArray =
                                        Encoding.ASCII.GetBytes(esiOptions.ClientId + ":" + esiOptions.ClientSecret);
                                    
                                    var requestContent = new FormUrlEncodedContent(new []
                                    {
                                        new KeyValuePair<string, string>("grant_type", "refresh_token"), 
                                        new KeyValuePair<string, string>("refresh_token", refreshToken.Value) 
                                    });

                                    requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                                    var request = new HttpRequestMessage
                                    {
                                        RequestUri = new Uri($"{CesiConstants.SsoOrigin}/v2/oauth/token"),
                                        Method = HttpMethod.Post,
                                        Content = requestContent
                                    };
                                    
                                    request.Headers.Authorization = 
                                        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                                    var httpClient = new HttpClient();

                                    var response = await httpClient.SendAsync(request);
                                    
                                    if (!response.IsSuccessStatusCode) context.RejectPrincipal();

                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                                    refreshToken.Value = tokenResponse.RefreshToken;
                                    accessToken.Value = tokenResponse.AccessToken;
                                    expiration.Value = (DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn))
                                        .ToString(CultureInfo.InvariantCulture);
                                    
                                    context.Properties.StoreTokens(tokens);
                                    context.ShouldRenew = true;
                                }
                            }
                        }
                    };
                })
                .AddOAuth("EVE_SSO", options =>
                {
                    options.AuthorizationEndpoint = $"{CesiConstants.SsoOrigin}/v2/oauth/authorize/";

                    if (esiOptions.Scopes != null && esiOptions.Scopes.Any())
                    {
                        foreach (var scope in esiOptions.Scopes)
                        {
                            options.Scope.Add(scope);
                        }
                    }

                    options.CallbackPath = esiOptions.CallbackPath;
                    
                    options.ClientId = esiOptions.ClientId;
                    options.ClientSecret = esiOptions.ClientSecret;
                    options.TokenEndpoint = $"{CesiConstants.SsoOrigin}/v2/oauth/token";

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "CharacterID");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "CharacterName");
                    
                    options.ClaimActions.MapJsonKey(ClaimTypes.Expiration, "ExpiresOn");
                    options.ClaimActions.MapJsonKey("Scopes", "Scopes");
                    options.ClaimActions.MapJsonKey("TokenType", "TokenType");
                    options.ClaimActions.MapJsonKey("CharacterOwnerHash", "CharacterOwnerHash");
                    options.ClaimActions.MapJsonKey("IntellectualProperty", "IntellectualProperty");

                    options.SaveTokens = true;
                    
                    options.Events = esiOptions.AuthenticationEvents;
                    
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, $"{CesiConstants.SsoOrigin}/oauth/verify");
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization =
                                new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var userData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(userData);
                        }
                    };
                });

            return builder;
        }

        public static CesiClientBuilder AddCesiEndpoints(this CesiClientBuilder builder)
        {
            builder.Services.AddScoped<AssetEndpoints>();
            builder.Services.AddScoped<CharacterEndpoints>();
            builder.Services.AddScoped<UniverseEndpoints>();
        
            return builder;
        }
    }
}
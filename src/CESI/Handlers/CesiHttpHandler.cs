using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CESI.Configuration;
using CESI.ResponseModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CESI.Handlers
{
    public class CesiHttpHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CesiHttpHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CesiResponse<T>> GetAsync<T>(string uri, string ifNoneMatch = null, Dictionary<string, string> queryParameters = null)
        {
            return await RequestAsync<T>(HttpMethod.Get, uri, ifNoneMatch, queryParameters);
        }

        private async Task<CesiResponse<T>> RequestAsync<T>(HttpMethod method, string uri, string ifNoneMatch = null, 
            Dictionary<string, string> queryParameters = null, object body = null)
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams.Add("datasource", "tranquility");

            if (queryParameters != null)
            {
                foreach (var (key, value) in queryParameters)
                {
                    queryParams.Add(key, value);
                }
            }

            var builder = new UriBuilder(CesiConstants.EsiOrigin)
            {
                Path = uri,
                Query = queryParams.ToString() ?? ""
            };

            var request = new HttpRequestMessage
            {
                RequestUri = builder.Uri,
                Method = method
            };

            if ((method == HttpMethod.Post || method == HttpMethod.Put) && body != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            }

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                await _httpContextAccessor.HttpContext.AuthenticateAsync();
            }

            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (ifNoneMatch != null)
            {
                request.Headers.Add("If-None-Match", ifNoneMatch);
            }

            var response = await _httpClient.SendAsync(request);

            return await ProcessResponse<T>(response);
        }

        private static async Task<CesiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            var model = new CesiResponse<T>();

            if (!response.IsSuccessStatusCode) return model;
            
            /*if (response.Headers.Contains("warning"))
            {
                model.LegacyWarning = true;
                model.Message = string.Join(", ", response.Headers.GetValues("warning"));
            }

            model.Expires = response.Content.Headers.Expires;
            model.LastModified = response.Content.Headers.LastModified;

            if (response.Headers.TryGetValues("X-Pages", out var pages))
            {
                model.Pages = int.TryParse(pages.FirstOrDefault(), out var nPages) ? nPages : 1;
            }

            if (response.Headers.TryGetValues("Content-Language", out var language))
            {
                model.Language = language.FirstOrDefault();
            }

            if (response.Headers.TryGetValues("ETag", out var eTag))
            {
                model.ETag = eTag.FirstOrDefault();
            }*/

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return model;
            }

            if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var decompressed = new GZipStream(stream, CompressionMode.Decompress);
                using var reader = new StreamReader(decompressed);
                model.JsonString = await reader.ReadToEndAsync();
            }
            else
            {
                model.JsonString = await response.Content.ReadAsStringAsync();
            }

            return model;
        }
    }
}
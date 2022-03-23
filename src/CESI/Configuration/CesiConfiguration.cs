using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace CESI.Configuration
{
    public class CesiConfiguration
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? CallbackPath { get; set; } = "/account/callback";
        
        public IEnumerable<string>? Scopes { get; set; }
        
        public OAuthEvents? AuthenticationEvents { get; set; }
        public DatabaseOptions? DatabaseOptions { get; set; }
    }

    public class DatabaseOptions
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 3306;
        public string Database { get; set; } = "cesi";
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Charset { get; set; } = "utf8";
        public bool ConvertZeroDatetime { get; set; } = true;
        public string ConnectionString => $"Server={Server};Port={Port};Database={Database};" +
                                          $"User={User};Password={Password};" +
                                          $"Charset={Charset};ConvertZeroDateTime={ConvertZeroDatetime}";
    }
}
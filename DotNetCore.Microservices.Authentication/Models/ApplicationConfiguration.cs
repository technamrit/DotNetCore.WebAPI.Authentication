using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Microservices.Authentication.Models
{
    public class ApplicationConfiguration
    {
        private const string AppName = "Microservices.Authentication";
        public string ApplicationName 
        { 
            get
            {
                return AppName;
            }
        }
        public ConnectionStrings ConnectionStrings { get; set; }
        public JWT JWT { get; set; }
        public string AllowedHosts { get; set; }
        public string AllowedMethods { get; set; }
        public string AllowedHeaders { get; set; }
        public bool AllowedCredentials { get; set; }
    }

    public class ConnectionStrings
    {
        public string DatabaseConnectionString { get; set; }
    }

    public class JWT
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string IssuerSigningKey { get; set; }
    }
}

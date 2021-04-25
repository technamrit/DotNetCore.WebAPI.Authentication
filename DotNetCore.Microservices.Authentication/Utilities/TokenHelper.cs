using DotNetCore.Microservices.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace DotNetCore.Microservices.Authentication.Utilities
{
    public class TokenHelper : ITokenHelper
    {
        private readonly ApplicationConfiguration applicationConfiguration;
        public TokenHelper(IOptions<ApplicationConfiguration> applicationConfiguration)
        {
            this.applicationConfiguration = applicationConfiguration?.Value;
        }

        public JwtSecurityToken GetSecurityToken(List<Claim> claims)
        {
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationConfiguration.JWT.IssuerSigningKey));

            var token = new JwtSecurityToken(
                    issuer: applicationConfiguration.JWT.ValidIssuer,
                    audience: applicationConfiguration.JWT.ValidAudience,
                    expires: DateTime.Now.AddHours(3),
                    claims: claims,
                    signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        

    }
}

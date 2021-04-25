using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DotNetCore.Microservices.Authentication.Utilities
{
    public interface ITokenHelper
    {
        JwtSecurityToken GetSecurityToken(List<Claim> claims);
    }
}

using DotNetCore.Microservices.Authentication.Models;
using DotNetCore.Microservices.Authentication.Repository;
using DotNetCore.Microservices.Authentication.Utilities;
using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DotNetCore.Microservices.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase, IAuthenticationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AuthenticationController> logger;
        private readonly IUserRepository userRepository;
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly ITokenHelper tokenHelper;

        public AuthenticationController(UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, IUserRepository userRepository, IOptions<ApplicationConfiguration> applicationConfiguration, ITokenHelper tokenHelper)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.userRepository = userRepository;
            this.applicationConfiguration = applicationConfiguration?.Value;
            this.tokenHelper = tokenHelper;
        }

        /// <summary>
        /// SignIn User
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> SignInAsync([FromBody] SignIn credentials)
        {
            var user = await userManager.FindByNameAsync(credentials.Username);
            
            if(user != null && await userManager.CheckPasswordAsync(user, credentials.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRoles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var securityToken = tokenHelper.GetSecurityToken(userClaims);

                logger.LogInformation($"User Signed In - Username : {user.UserName}");

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                    expiration = securityToken.ValidTo
                });
            }
            return Unauthorized();
        }

        /// <summary>
        ///  Registe User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register user)
        {
            var userExists = await userManager.FindByNameAsync(user.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Status = "Error", Message = "User already exists !" });

            var response = await userRepository.AddUserAsync(user);

            if(!response.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Status = "Error", Message = "User creation failed !" });

            return Ok(new BaseResponse { Status = "Success", Message = "User created successfully !!" });
        }
    }
}

using DotNetCore.Microservices.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace DotNetCore.Microservices.Authentication.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IdentityResult> AddUserAsync(Register user)
        {
            IdentityResult response = null;
            ApplicationUser newUser = new ApplicationUser()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.Username
            };

            var userCreationResponse = await userManager.CreateAsync(newUser, user.Password);
            if (!await roleManager.RoleExistsAsync(user.Role))
                await roleManager.CreateAsync(new IdentityRole(user.Role));

            if(userCreationResponse.Succeeded && await roleManager.RoleExistsAsync(user.Role))
            {
                response = await userManager.AddToRoleAsync(newUser, user.Role);
            }

            if(response != null)
            {
                if (!response.Succeeded)
                    await DeleteUserAsync(newUser);
            }

            return response;
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            IdentityResult response = null;

            var userExists = await userManager.FindByNameAsync(user.UserName);
            if (userExists != null)
                response = await userManager.DeleteAsync(user);

            return response;
        }
    }
}

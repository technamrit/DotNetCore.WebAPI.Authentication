using DotNetCore.Microservices.Authentication.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DotNetCore.Microservices.Authentication.Repository
{
    public interface IUserRepository
    {
        Task<IdentityResult> AddUserAsync(Register user);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);

    }
}

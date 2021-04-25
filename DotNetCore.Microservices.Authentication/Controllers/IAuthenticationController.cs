using DotNetCore.Microservices.Authentication.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCore.Microservices.Authentication.Controllers
{
    public interface IAuthenticationController
    {
        Task<IActionResult> SignInAsync(SignIn credentials);
        Task<IActionResult> RegisterAsync(Register user);
    }
}

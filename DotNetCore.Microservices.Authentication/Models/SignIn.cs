using System.ComponentModel.DataAnnotations;


namespace DotNetCore.Microservices.Authentication.Models
{
    public class SignIn
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}

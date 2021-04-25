using DotNetCore.Microservices.Authentication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore.Microservices.Authentication.DataAccessLayer
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public  ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserLogin.Models;
using UserLogin.Models.ViewModels;

namespace UserLogin.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected AppDbContext()
        {
        }

        public DbSet<Userprofiles> UserProfiles { get; set; }
    }

}

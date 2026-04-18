using Microsoft.AspNetCore.Identity;

namespace UserLogin.Models
{
    public class Users : IdentityUser
    {
        public String Name { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace UserLogin.Models
{
    public class Users : IdentityUser
    {
        public String FullName { get; set; }
    }
}

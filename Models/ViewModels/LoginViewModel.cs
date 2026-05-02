using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage= "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }


        [Display(Name ="Remember Me?")]
        public bool RememberMe { get; set; }
    }
}

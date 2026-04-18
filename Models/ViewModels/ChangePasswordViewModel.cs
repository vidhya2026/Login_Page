using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name ="New Password")]
        public string NewPassword { get; set; }
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be {2} & at max{1} character")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password doesn't Match")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is Required")]
        public string ConfirmNewPassword { get; set; }
    }
}

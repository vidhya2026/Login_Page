using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "New Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is Required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password doesn't Match")]
        public string ConfirmNewPassword { get; set; }
    }
}

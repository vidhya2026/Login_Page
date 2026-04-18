using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is Required")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength =8, ErrorMessage ="The {0} must be {2} & at max{1} character")]
        [Compare("ConfirmPassword",ErrorMessage ="Password doesn't Match")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is Required")]
        public string ConfirmPassword {  get; set; }
    }
}

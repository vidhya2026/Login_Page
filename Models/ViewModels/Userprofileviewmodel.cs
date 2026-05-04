using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models.ViewModels
{
    public class UserProfileViewModel
    {
        // Hidden field — 0 = new record (Add), >0 = existing record (Edit)
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        // ^[a-zA-Z\s]+$ — only letters and spaces allowed.
        // ^ and $ anchor the check to the full string, not just part of it.
        // \s allows spaces so "John Doe" is valid; digits and symbols are rejected.
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessage = "Name must contain letters only. No numbers or special characters.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        // Range enforces minimum 1 and maximum 120 on the server side.
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please select a gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        // ^\d{10}$ — exactly 10 digits, nothing else.
        // \d means any digit (0-9). {10} means exactly 10 of them.
        // This runs on the server after the client-side JS has already blocked non-digits.
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Phone number must be exactly 10 digits")]
        public string Phone { get; set; }

        // IFormFile is the ASP.NET Core uploaded-file type.
        // Nullable (?) because on Edit the user may skip re-uploading.
        public IFormFile? ImageFile { get; set; }

        // Holds the existing image URL on the Edit form so the view can show a preview.
        public string? ExistingImagePath { get; set; }
    }
}
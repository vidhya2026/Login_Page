using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models
{
    // This model represents a single user profile stored in the database.
    // It is NOT the Identity user (Users.cs) — it's a separate table for app-level user data.
    public class Userprofiles
    {
        // Primary key — EF Core auto-increments this integer for every new row.
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessage = "Name must contain letters only — no numbers or special characters")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        // Age is a plain int so the default model binder rejects non-numeric input automatically.
        // Range keeps the value between 1 and 120 — any value outside that fails validation.
        [Required(ErrorMessage = "Age is required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        // Gender is stored as a string; the allowed values are enforced in the view via a
        // <select> dropdown, keeping the controller and model clean of switch-case logic.
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        // EmailAddress attribute runs the built-in RFC-5322 format check beyond just [Required].
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        // Phone attribute adds a lightweight regex for common phone formats.
        // We also add a custom regex to ensure only digits, spaces, dashes, and + are used.
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[\d\s\-\+\(\)]{7,15}$",
            ErrorMessage = "Phone number must be 7–15 digits and may include +, -, spaces, or parentheses")]
        public string Phone { get; set; }

        // ImagePath stores only the relative URL string (e.g. "/uploads/abc.jpg").
        // The actual file is saved to wwwroot/uploads by the controller.
        // We do NOT store the raw IFormFile here because EF Core cannot persist it to the DB.
        public string? ImagePath { get; set; }
    }
}
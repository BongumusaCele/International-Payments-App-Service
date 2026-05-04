using System.ComponentModel.DataAnnotations;
using InternationalPaymentsAPI.Helpers;

namespace InternationalPaymentsAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "First name may contain letters, spaces, hyphens, and apostrophes only.")]
        public string first_Name { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "Last name may contain letters, spaces, hyphens, and apostrophes only.")]
        public string last_Name { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.IdNumber, ErrorMessage = "ID number must be exactly 13 digits.")]
        public string id_Number { get; set; }

        [Required]
        [EmailAddress]
        public string? email_Address { get; set; }

        [Required]
        [Range(100000, 2147483647, ErrorMessage = "Account number must be between 6 and 10 digits.")]
        public int account_Number { get; set; }

        [Required]
        public int currency_Id { get; set; }

        public string? preferred_Currency { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.Username, ErrorMessage = "Username must start with a letter and use 3 to 30 letters, numbers, dots, underscores, or hyphens.")]
        public string username { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.StrongPassword, ErrorMessage = "Password must be 12 to 128 characters and include uppercase, lowercase, number, and special character.")]
        public string password { get; set; }

        [Required]
        [Compare("password")]
        public string confirm_Password { get; set; }
    }
}

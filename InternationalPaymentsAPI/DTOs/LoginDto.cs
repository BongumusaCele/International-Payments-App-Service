using System.ComponentModel.DataAnnotations;
using InternationalPaymentsAPI.Helpers;

namespace InternationalPaymentsAPI.DTOs
{
    public class LoginDto
    {
        [Required]
        [RegularExpression(ValidationPatterns.Username, ErrorMessage = "Username must start with a letter and use 3 to 30 letters, numbers, dots, underscores, or hyphens.")]
        public string username { get; set; }

        [Required]
        [Range(100000, 2147483647, ErrorMessage = "Account number must be between 6 and 10 digits.")]
        public int account_Number { get; set; }

        [StringLength(128, MinimumLength = 1)]
        public string? password { get; set; }

        [StringLength(128, MinimumLength = 1)]
        public string? password_Hash { get; set; }

        public string? GetSubmittedPassword()
        {
            return password ?? password_Hash;
        }
    }
}

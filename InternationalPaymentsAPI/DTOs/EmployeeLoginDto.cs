using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class EmployeeLoginDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,30}$",
            ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
        public string username { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9@#$!%*?&._-]{8,100}$",
            ErrorMessage = "Password contains invalid characters.")]
        public string password { get; set; } = string.Empty;
    }
}

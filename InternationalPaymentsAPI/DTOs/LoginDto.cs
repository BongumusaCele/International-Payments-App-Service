using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class LoginDto
    {
        [Required]
        public string username { get; set; } = string.Empty;

        [Required]
        public int account_Number { get; set; }

        public string? password { get; set; }

        public string? password_Hash { get; set; }

        public string GetPassword() => password ?? password_Hash ?? string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class LoginDto
    {
        [Required]
        public string username { get; set; }

        [Required]
        public int account_Number { get; set; }

        [Required]
        public string password { get; set; }
    }
}
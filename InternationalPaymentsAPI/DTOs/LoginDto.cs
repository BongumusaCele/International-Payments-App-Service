using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username field is empty")]
        public string username { get; set; }

        [Required(ErrorMessage = "Account number field is empty")]
        public int account_Number { get; set; }

        [Required(ErrorMessage = "Password field is empty")]
        [DataType(DataType.Password)]
        public string password_Hash { get; set; }
    }
}

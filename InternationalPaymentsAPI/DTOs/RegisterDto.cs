using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name field is empty")]
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Last name field is empty")]
        public string last_Name { get; set; }

        [Required(ErrorMessage = "ID number field is empty")]
        public string id_Number { get; set; }

        [EmailAddress]
        public string? email_Address { get; set; }

        [Required(ErrorMessage = "Account number field is empty")]
        public int account_Number { get; set; }

        public string? preferred_Currency { get; set; }

        [Required(ErrorMessage = "Username field is empty")]
        public string username { get; set; }

        [Required(ErrorMessage = "Password field is empty")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirm password field is empty")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string confirm_Password { get; set; }
    }
} 
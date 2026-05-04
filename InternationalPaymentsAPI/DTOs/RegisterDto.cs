using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string first_Name { get; set; }

        [Required]
        public string last_Name { get; set; }

        public string id_Number { get; set; }

        [EmailAddress]
        public string? email_Address { get; set; }

        [Required]
        public int account_Number { get; set; }

        [Required]
        public int currency_Id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [Compare("password")]
        public string confirm_Password { get; set; }
    }
}
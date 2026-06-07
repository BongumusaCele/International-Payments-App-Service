using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class EmployeeLoginDto
    {
        [Required]
        public string username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
    }
}

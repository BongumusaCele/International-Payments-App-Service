using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblEmployee")]
    public class EmployeeModel
    {
        [Key]
        public int employee_Id { get; set; }

        [Required, StringLength(100)]
        public string username { get; set; } = string.Empty;

        [Required]
        public string password_Hash { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string full_Name { get; set; } = string.Empty;

        public bool is_Active { get; set; } = true;
    }
}
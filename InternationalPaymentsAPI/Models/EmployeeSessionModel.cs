using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblEmployeeSession")]
    public class EmployeeSessionModel
    {
        [Key]
        public int employee_Session_Id { get; set; }

        public int employee_Id { get; set; }

        public DateTime login_Time { get; set; } = DateTime.UtcNow;
        public DateTime? logout_Time { get; set; }

        public bool is_Active { get; set; } = true;

        [Required]
        public string session_Token_Hash { get; set; } = string.Empty;

        public DateTime expires_On { get; set; }

        public EmployeeModel Employee { get; set; } = null!;
    }
}

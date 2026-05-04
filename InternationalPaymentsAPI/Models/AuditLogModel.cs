using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblAuditLog")]
    public class AuditLogModel
    {
        [Key]
        [Column("audit_Id")]
        public int audit_Id { get; set; }

        [Column("customer_Id")]
        public int? customer_Id { get; set; }

        [Required]
        [Column("action_Type")]
        public string action_Type { get; set; }

        [Column("table_Name")]
        public string? table_Name { get; set; }

        [Column("record_Id")]
        public int? record_Id { get; set; }

        [Column("old_Value")]
        public string? old_Value { get; set; }

        [Column("new_Value")]
        public string? new_Value { get; set; }

        [Column("details")]
        public string? details { get; set; }

        [Column("created_On")]
        public DateTime created_On { get; set; } = DateTime.Now;

        public CustomerModel? Customer { get; set; }
    }
}
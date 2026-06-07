using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblPayment")]
    public class PaymentModel
    {
        [Key]
        public int payment_Id { get; set; }

        public int customer_Id { get; set; }
        public int beneficiary_Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        [Required, StringLength(3)]
        public string currency { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string provider { get; set; } = "SWIFT";

        [Required, StringLength(20)]
        public string swift_Code { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string status { get; set; } = "Pending";

        public DateTime created_On { get; set; } = DateTime.UtcNow;
        public DateTime? verified_On { get; set; }
        public int? verified_By_Employee_Id { get; set; }
        public DateTime? submitted_To_Swift_On { get; set; }

        public DateTime? rejected_On { get; set; }

        public int? rejected_By_Employee_Id { get; set; }

        public string? rejection_Reason { get; set; }

        public CustomerModel Customer { get; set; } = null!;
        public BeneficiaryModel Beneficiary { get; set; } = null!;
        public EmployeeModel? VerifiedByEmployee { get; set; }
    }
}

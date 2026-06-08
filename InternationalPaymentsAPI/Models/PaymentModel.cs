using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblPayment")]
    public class PaymentModel
    {
        [Key]
        [Column("payment_Id")]
        public int payment_Id { get; set; }

        [Required]
        [Column("customer_Id")]
        public int customer_Id { get; set; }

        [Required]
        [Column("beneficiary_Id")]
        public int beneficiary_Id { get; set; }

        [Required]
        [Column("from_Currency_Id")]
        public int from_Currency_Id { get; set; }

        [Required]
        [Column("to_Currency_Id")]
        public int to_Currency_Id { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        [Required]
        [Column("exchange_Rate_Used", TypeName = "decimal(18,4)")]
        public decimal exchange_Rate_Used { get; set; }

        [Required]
        [Column("converted_Amount", TypeName = "decimal(18,2)")]
        public decimal converted_Amount { get; set; }

        [Column("payment_Reference")]
        public string? payment_Reference { get; set; }

        [Column("payment_Reason")]
        public string? payment_Reason { get; set; }

        [Required]
        [StringLength(50)]
        [Column("payment_Provider")]
        public string payment_Provider { get; set; } = "SWIFT";

        [Required]
        [StringLength(20)]
        [Column("swift_Code")]
        public string swift_Code { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Column("status")]
        public string status { get; set; } = "Pending";

        [Column("created_On")]
        public DateTime created_On { get; set; } = DateTime.UtcNow;

        [Column("updated_On")]
        public DateTime? updated_On { get; set; }

        public DateTime? verified_On { get; set; }
        public int? verified_By_Employee_Id { get; set; }
        public DateTime? submitted_To_Swift_On { get; set; }
        public DateTime? rejected_On { get; set; }
        public int? rejected_By_Employee_Id { get; set; }
        public string? rejection_Reason { get; set; }

        public CustomerModel Customer { get; set; } = null!;
        public BeneficiaryModel Beneficiary { get; set; } = null!;
        public CurrencyModel FromCurrency { get; set; } = null!;
        public CurrencyModel ToCurrency { get; set; } = null!;
        public EmployeeModel? VerifiedByEmployee { get; set; }
    }
}

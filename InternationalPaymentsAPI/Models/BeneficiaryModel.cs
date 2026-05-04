using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblBeneficiary")]
    public class BeneficiaryModel
    {
        [Key]
        [Column("beneficiary_Id")]
        public int beneficiary_Id { get; set; }

        [ForeignKey("customer_Id")]
        [Column("customer_Id")]
        public int customer_Id { get; set; }

        [Required]
        [Column("currency_Id")]
        public int currency_Id { get; set; }

        [Required(ErrorMessage = "Beneficiary name field is empty")]
        [StringLength(100)]
        [Column("beneficiary_Name")]
        public string beneficiary_Name { get; set; }

        [Required(ErrorMessage = "Bank name field is empty")]
        [StringLength(100)]
        [Column("bank_Name")]
        public string bank_Name { get; set; }

        [Required(ErrorMessage = "Account number field is empty")]
        [StringLength(20)]
        [Column("account_Number")]
        public string account_Number { get; set; }

        [StringLength(20)]
        [Column("swift_Code")]
        public string? swift_Code { get; set; }

        [Column("country")]
        public string? country { get; set; }

        public CustomerModel Customer { get; set; }
        public CurrencyModel Currency { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblCustomer")]
    public class CustomerModel
    {
        [Key]
        [Column("customer_Id")]
        public int customer_Id { get; set; }

        [Required(ErrorMessage = "First name field is empty")]
        [StringLength(150)]
        [Column("first_Name")]
        public string first_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name field is empty")]
        [StringLength(150)]
        [Column("last_Name")]
        public string last_Name { get; set; } = string.Empty;

        [Column("id_Number")]
        [Required(ErrorMessage = "ID number field is empty")]
        [StringLength(13, ErrorMessage = "ID number must be exactly 13 digits")]
        public string id_Number { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        [Column("email_Address")]
        public string? email_Address { get; set; }

        [Required(ErrorMessage = "Account number field is empty")]
        [Column("account_Number")]
        public int account_Number { get; set; }

        [Required(ErrorMessage = "Currency field is required")]
        [Column("currency_Id")]
        public int currency_Id { get; set; }

        public CurrencyModel Currency { get; set; }

        [Required(ErrorMessage = "Username field is empty")]
        [StringLength(100)]
        [Column("username")]
        public string username { get; set; } = string.Empty;


        [Required(ErrorMessage = " Password field is empty")]
        [Column("password_hash")]
        [DataType(DataType.Password)]
        public string password_Hash { get; set; } = string.Empty;


        [Required]
        [Column("created_On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public ICollection<BeneficiaryModel> Beneficiaries { get; set; }
           = new List<BeneficiaryModel>();

        public ICollection<CustomerSessionModel> CustomerSessions { get; set; }
            = new List<CustomerSessionModel>();

        public ICollection<MfaChallengeModel> MfaChallenges { get; set; }
            = new List<MfaChallengeModel>();


    }
}

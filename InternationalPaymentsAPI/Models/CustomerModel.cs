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
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Last name field is empty")]
        [StringLength(150)]
        [Column("last_Name")]
        public string last_Name { get; set; }

        [Column("id_Number")]
        public int id_Number { get; set; }

        [EmailAddress]
        [StringLength(150)]
        [Column("email_Address")]
        public string? email_Address { get; set; }

        [Required(ErrorMessage = "Account number field is empty")]
        [Column("account_Number")]
        public int account_Number { get; set; }

        [StringLength(50)]
        [Column("preferred_Currency")]
        public string? preferred_Currency { get; set; }

        [Required(ErrorMessage = "Username field is empty")]
        [StringLength(100)]
        [Column("username")]
        public string username { get; set; }


        [Required(ErrorMessage = " Password field is empty")]
        [Column("password_hash")]
        [DataType(DataType.Password)]
        public string password_Hash { get; set; }


        [Required]
        [Column("created_On")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;


    }
}

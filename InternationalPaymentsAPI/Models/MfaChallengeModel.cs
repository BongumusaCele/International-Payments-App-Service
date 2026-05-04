using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblMfaChallenge")]
    public class MfaChallengeModel
    {
        [Key]
        [Column("mfa_Challenge_Id")]
        public Guid mfa_Challenge_Id { get; set; }

        [Column("customer_Id")]
        public int customer_Id { get; set; }

        [Required]
        [Column("otp_Code_Hash")]
        public string otp_Code_Hash { get; set; }

        [Column("created_On")]
        public DateTime created_On { get; set; } = DateTime.UtcNow;

        [Column("expires_On")]
        public DateTime expires_On { get; set; }

        [Column("consumed_On")]
        public DateTime? consumed_On { get; set; }

        [Column("attempt_Count")]
        public int attempt_Count { get; set; }

        public CustomerModel Customer { get; set; }
    }
}

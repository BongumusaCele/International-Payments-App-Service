using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class VerifyMfaDto
    {
        [Required]
        public Guid mfa_Challenge_Id { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string otp_Code { get; set; }
    }
}

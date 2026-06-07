using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class RejectPaymentDto
    {
        [Required]
        public string rejection_Reason { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class RejectPaymentDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 .,'\-]{5,250}$",
            ErrorMessage = "Reason may only contain letters, numbers, spaces, and basic punctuation.")]
        public string rejection_Reason { get; set; } = string.Empty;
    }
}

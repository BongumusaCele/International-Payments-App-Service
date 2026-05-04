using System.ComponentModel.DataAnnotations;
using InternationalPaymentsAPI.Helpers;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int customer_Id { get; set; }

        [Required]
        public int beneficiary_Id { get; set; }

        [Required]
        [Range(typeof(decimal), "1.00", "1000000.00", ErrorMessage = "Amount must be between 1.00 and 1,000,000.00.")]
        public decimal amount { get; set; }

        [Required]
        [StringLength(35, MinimumLength = 3)]
        [RegularExpression(ValidationPatterns.PaymentReference, ErrorMessage = "Payment reference must be 3 to 35 characters and use letters, numbers, spaces, dots, underscores, slashes, hashes, or hyphens.")]
        public string? payment_Reference { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.PaymentProvider, ErrorMessage = "Payment provider must be SWIFT, Bank Transfer, or EFT.")]
        public string payment_Provider { get; set; } = "SWIFT";

        [Required(ErrorMessage = "SWIFT code is required")]
        [StringLength(11, MinimumLength = 8)]
        [RegularExpression(ValidationPatterns.SwiftCode, ErrorMessage = "SWIFT code must be 8 or 11 uppercase letters and numbers.")]
        public string swift_Code { get; set; }

        [StringLength(120)]
        public string? payment_Reason { get; set; }
    }
}

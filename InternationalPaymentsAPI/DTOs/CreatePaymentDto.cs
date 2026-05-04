using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int customer_Id { get; set; }

        [Required]
        public int beneficiary_Id { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal amount { get; set; }

        public string? payment_Reference { get; set; }

        public string payment_Provider { get; set; } = "SWIFT";

        [Required(ErrorMessage = "SWIFT code is required")]
        public string swift_Code { get; set; }

        public string? payment_Reason { get; set; }
    }
}
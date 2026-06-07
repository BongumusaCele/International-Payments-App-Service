using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int beneficiary_Id { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal amount { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{3}$",
            ErrorMessage = "Currency must be a valid 3-letter code, e.g. USD.")]
        public string currency { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z]{2,30}$",
            ErrorMessage = "Provider may only contain uppercase letters.")]
        public string provider { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z0-9]{8,11}$",
            ErrorMessage = "SWIFT code must be 8 to 11 uppercase letters/numbers.")]
        public string swift_Code { get; set; } = string.Empty;
    }
}
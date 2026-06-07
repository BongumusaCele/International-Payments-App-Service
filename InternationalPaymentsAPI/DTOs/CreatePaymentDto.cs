using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int beneficiary_Id { get; set; }

        [Range(1, 999999999)]
        public decimal amount { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{3}$")]
        public string currency { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(SWIFT)$")]
        public string provider { get; set; } = "SWIFT";

        [Required]
        [RegularExpression(@"^[A-Z0-9]{8}([A-Z0-9]{3})?$")]
        public string swift_Code { get; set; } = string.Empty;
    }
}

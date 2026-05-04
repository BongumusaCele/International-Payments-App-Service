using System.ComponentModel.DataAnnotations;
using InternationalPaymentsAPI.Helpers;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreateBeneficiaryDto
    {
        [Required]
        public int customer_Id { get; set; }

        public int? currency_Id { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "Beneficiary name may contain letters, spaces, hyphens, and apostrophes only.")]
        public string beneficiary_Name { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.BankName, ErrorMessage = "Bank name contains unsupported characters.")]
        public string bank_Name { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.NumericAccountNumber, ErrorMessage = "Account number must contain 6 to 20 digits only.")]
        public string account_Number { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 8)]
        [RegularExpression(ValidationPatterns.SwiftCode, ErrorMessage = "SWIFT code must be 8 or 11 uppercase letters and numbers.")]
        public string? swift_Code { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.Country, ErrorMessage = "Country may contain letters, spaces, dots, hyphens, and apostrophes only.")]
        public string? country { get; set; }
    }
}

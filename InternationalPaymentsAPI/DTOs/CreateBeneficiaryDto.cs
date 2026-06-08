using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreateBeneficiaryDto
    {
        [Required]
        public int customer_Id { get; set; }

        public int currency_Id { get; set; }

        [Required]
        public string beneficiary_Name { get; set; } = string.Empty;

        [Required]
        public string bank_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string account_Number { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string swift_Code { get; set; } = string.Empty;

        public string? country { get; set; }
    }
}

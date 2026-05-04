using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class UpdateBeneficiaryDto
    {
        [Required]
        public string beneficiary_Name { get; set; }

        [Required]
        public string bank_Name { get; set; }

        [Required]
        public int account_Number { get; set; }

        [Required]
        public string swift_Code { get; set; }

        [Required]
        public string country { get; set; }
    }
}
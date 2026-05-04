using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreateBeneficiaryDto
    {
        [Required]
        public int customer_Id { get; set; }

        [Required]
        public int currency_Id { get; set; }

        [Required]
        public string beneficiary_Name { get; set; }

        [Required]
        public string bank_Name { get; set; }

        [Required]
        public int account_Number { get; set; }

        public string? country { get; set; }
    }
}
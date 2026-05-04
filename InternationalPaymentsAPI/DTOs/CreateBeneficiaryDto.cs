using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreateBeneficiaryDto
    {
        [Required]
        public int customer_Id { get; set; }

        [Required]
        public string beneficiary_Name { get; set; }

        [Required]
        public string bank_Name { get; set; }

        [Required]
        [StringLength(20)]
        public string account_Number { get; set; }

        [Required]
        public string swift_Code { get; set; }

        [Required]
        public string country { get; set; }
    }
}

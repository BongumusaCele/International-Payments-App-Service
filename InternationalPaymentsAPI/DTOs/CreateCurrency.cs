using System.ComponentModel.DataAnnotations;

namespace InternationalPaymentsAPI.DTOs
{
    public class CreateCurrencyDto
    {
        [Required]
        public string currency_Name { get; set; }

        [Required]
        public string currency_Code { get; set; }

        [Required]
        public decimal exchange_Rate { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternationalPaymentsAPI.Models
{
    [Table("tblCurrency")]
    public class CurrencyModel
    {
        [Key]
        [Column("currency_Id")]
        public int currency_Id { get; set; }

        [Required]
        [Column("currency_Name")]
        public string currency_Name { get; set; }

        [Required]
        [Column("currency_Code")]
        public string currency_Code { get; set; }

        [Required]
        [Column("exchange_Rate")]
        public decimal exchange_Rate { get; set; }
    }
}
namespace InternationalPaymentsAPI.DTOs
{
    public class BeneficiaryResponseDto
    {
        public int beneficiary_Id { get; set; }
        public int customer_Id { get; set; }
        public int currency_Id { get; set; }
        public string? currency_Code { get; set; }
        public string? currency_Name { get; set; }
        public string beneficiary_Name { get; set; } = string.Empty;
        public string bank_Name { get; set; } = string.Empty;
        public string account_Number { get; set; } = string.Empty;
        public string swift_Code { get; set; } = string.Empty;
        public string? country { get; set; }
    }
}

namespace InternationalPaymentsAPI.DTOs
{
    public class BeneficiaryResponseDto
    {
        public int beneficiary_Id { get; set; }

        public int customer_Id { get; set; }

        public string beneficiary_Name { get; set; }

        public string bank_Name { get; set; }

        public int account_Number { get; set; }

        public string swift_Code { get; set; }

        public string country { get; set; }
    }
}
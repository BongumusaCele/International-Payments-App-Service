namespace InternationalPaymentsAPI.DTOs
{
    public class PaymentResponseDto
    {
        public int payment_Id { get; set; }
        public int customer_Id { get; set; }
        public int beneficiary_Id { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; } = string.Empty;
        public string provider { get; set; } = string.Empty;
        public string swift_Code { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public DateTime created_On { get; set; }
        public DateTime? verified_On { get; set; }
        public DateTime? submitted_To_Swift_On { get; set; }
    }
}
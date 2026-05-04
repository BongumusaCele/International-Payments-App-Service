namespace InternationalPaymentsAPI.DTOs
{
    public class PaymentResponseDto
    {
        public bool success { get; set; }
        public string message { get; set; }

        public int payment_Id { get; set; }
        public int customer_Id { get; set; }
        public int beneficiary_Id { get; set; }

        public decimal amount { get; set; }
        public string from_Currency { get; set; }
        public string to_Currency { get; set; }
        public decimal exchange_Rate_Used { get; set; }
        public decimal converted_Amount { get; set; }

        public string? beneficiary_Name { get; set; }
        public string? recipient_Account_Number { get; set; }
        public string? recipient_Bank_Name { get; set; }
        public string? swift_Code { get; set; }
        public string? payment_Reference { get; set; }
        public string? payment_Provider { get; set; }
        public string? payment_Reason { get; set; }
        public string status { get; set; }
        public DateTime created_On { get; set; }
        public DateTime updated_On { get; set; }
    }
}

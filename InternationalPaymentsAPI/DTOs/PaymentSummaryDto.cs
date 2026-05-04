namespace InternationalPaymentsAPI.DTOs
{
    public class PaymentSummaryDto
    {
        public int total_Payments { get; set; }
        public decimal total_Amount { get; set; }
        public int pending_Count { get; set; }
        public int under_Review_Count { get; set; }
        public int approved_Count { get; set; }
        public int rejected_Count { get; set; }
        public int completed_Count { get; set; }
    }
}

namespace InternationalPaymentsAPI.DTOs
{
    public class RegisterResponseDto
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int? customer_Id { get; set; }
        public string? username { get; set; }
    }
}
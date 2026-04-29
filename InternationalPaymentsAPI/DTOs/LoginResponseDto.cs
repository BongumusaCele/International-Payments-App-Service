namespace InternationalPaymentsAPI.DTOs
{
    public class LoginResponseDto
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int? customer_Id { get; set; }
        public string? full_Name { get; set; }
        public string? username { get; set; }
        public int? account_Number { get; set; }
        public string? preferred_Currency { get; set; }
    }
}

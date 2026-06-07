namespace InternationalPaymentsAPI.DTOs
{
    public class EmployeeLoginResponseDto
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public string? token { get; set; }
        public DateTime? token_Expires_On { get; set; }
        public int? employee_Id { get; set; }
        public string? username { get; set; }
        public string? full_Name { get; set; }
    }
}
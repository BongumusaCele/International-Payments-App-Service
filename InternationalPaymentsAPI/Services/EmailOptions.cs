namespace InternationalPaymentsAPI.Services
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromAddress { get; set; } = "";
        public string FromName { get; set; } = "International Payments";
        public bool EnableSsl { get; set; } = true;
        public bool EnableSending { get; set; }
    }
}

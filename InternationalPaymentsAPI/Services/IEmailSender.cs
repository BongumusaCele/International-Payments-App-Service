namespace InternationalPaymentsAPI.Services
{
    public interface IEmailSender
    {
        Task SendOtpAsync(string toAddress, string otpCode);
    }
}

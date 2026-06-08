namespace InternationalPaymentsAPI.Services
{
    public interface ISwiftService
    {
        Task<bool> SubmitPaymentAsync(int paymentId);
    }
}
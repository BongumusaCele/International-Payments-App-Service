using InternationalPaymentsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Services
{
    public class SwiftService : ISwiftService
    {
        private readonly ApplicationDbContext _context;

        public SwiftService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SubmitPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);

            if (payment == null)
                return false;

            if (payment.status != "Verified")
                return false;

            // Simulate SWIFT submission
            payment.status = "SubmittedToSwift";
            payment.submitted_To_Swift_On = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

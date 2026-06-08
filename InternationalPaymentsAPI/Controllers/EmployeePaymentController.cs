using InternationalPaymentsAPI.Auth;
using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [Route("api/employee/payments")]
    [ApiController]
    [Authorize(AuthenticationSchemes = EmployeeAuthenticationHandler.SchemeName)]
    public class EmployeePaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISwiftService _swiftService;

        public EmployeePaymentController(ApplicationDbContext context, ISwiftService swiftService)
        {
            _context = context;
            _swiftService = swiftService;
        }

        [HttpGet]
        [HttpGet("pending")]
        public async Task<IActionResult> GetReviewQueue()
        {
            var reviewStatuses = new[] { "Pending", "Verified", "SubmittedToSwift", "Rejected" };

            var payments = await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Beneficiary)
                .Include(p => p.FromCurrency)
                .Include(p => p.ToCurrency)
                .Where(p => reviewStatuses.Contains(p.status))
                .OrderByDescending(p => p.created_On)
                .Select(p => new PaymentResponseDto
                {
                    payment_Id = p.payment_Id,
                    customer_Id = p.customer_Id,
                    customer_Name = p.Customer.first_Name + " " + p.Customer.last_Name,
                    customer_Account_Number = p.Customer.account_Number,
                    beneficiary_Id = p.beneficiary_Id,
                    amount = p.amount,
                    from_Currency = p.FromCurrency.currency_Code,
                    to_Currency = p.ToCurrency.currency_Code,
                    exchange_Rate_Used = p.exchange_Rate_Used,
                    converted_Amount = p.converted_Amount,
                    beneficiary_Name = p.Beneficiary.beneficiary_Name,
                    recipient_Account_Number = p.Beneficiary.account_Number,
                    recipient_Bank_Name = p.Beneficiary.bank_Name,
                    recipient_Country = p.Beneficiary.country,
                    swift_Code = p.swift_Code,
                    payment_Provider = p.payment_Provider,
                    payment_Reference = p.payment_Reference,
                    payment_Reason = p.payment_Reason,
                    status = p.status,
                    created_On = p.created_On,
                    updated_On = p.updated_On,
                    verified_On = p.verified_On,
                    submitted_To_Swift_On = p.submitted_To_Swift_On,
                    rejected_On = p.rejected_On,
                    rejection_Reason = p.rejection_Reason
                })
                .ToListAsync();

            return Ok(payments);
        }

        [HttpPost("{paymentId}/verify")]
        public async Task<IActionResult> VerifyPayment(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);

            if (payment == null)
                return NotFound(new { success = false, message = "Payment not found." });

            if (payment.status != "Pending")
                return BadRequest(new { success = false, message = "Only pending payments can be verified." });

            int employeeId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            payment.status = "Verified";
            payment.verified_On = DateTime.UtcNow;
            payment.verified_By_Employee_Id = employeeId;
            payment.updated_On = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Payment verified successfully." });
        }

        [HttpPost("{paymentId}/submit-to-swift")]
        public async Task<IActionResult> SubmitToSwift(int paymentId)
        {
            bool submitted = await _swiftService.SubmitPaymentAsync(paymentId);

            if (!submitted)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Payment must exist and be verified first."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Payment submitted to SWIFT successfully."
            });
        }

        [HttpPost("{paymentId}/reject")]
        public async Task<IActionResult> RejectPayment(int paymentId, RejectPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _context.Payments.FindAsync(paymentId);

            if (payment == null)
                return NotFound(new { success = false, message = "Payment not found." });

            if (payment.status != "Pending")
                return BadRequest(new { success = false, message = "Only pending payments can be rejected." });

            int employeeId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            payment.status = "Rejected";
            payment.rejected_On = DateTime.UtcNow;
            payment.rejected_By_Employee_Id = employeeId;
            payment.rejection_Reason = dto.rejection_Reason;
            payment.updated_On = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Payment rejected successfully."
            });
        }
    }
}

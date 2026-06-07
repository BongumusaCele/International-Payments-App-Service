using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Extensions;
using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("pay-now")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int customerId = User.GetCustomerId();

            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b =>
                    b.beneficiary_Id == dto.beneficiary_Id &&
                    b.customer_Id == customerId);

            if (beneficiary == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Beneficiary not found."
                });
            }

            var payment = new PaymentModel
            {
                customer_Id = customerId,
                beneficiary_Id = beneficiary.beneficiary_Id,
                amount = dto.amount,
                currency = dto.currency.ToUpper(),
                provider = dto.provider.ToUpper(),
                swift_Code = dto.swift_Code.ToUpper(),
                status = "Pending",
                created_On = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Payment submitted successfully.",
                payment_Id = payment.payment_Id
            });
        }

        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            int customerId = User.GetCustomerId();

            var payments = await _context.Payments
                .Where(p => p.customer_Id == customerId)
                .Select(p => new PaymentResponseDto
                {
                    payment_Id = p.payment_Id,
                    customer_Id = p.customer_Id,
                    beneficiary_Id = p.beneficiary_Id,
                    amount = p.amount,
                    currency = p.currency,
                    provider = p.provider,
                    swift_Code = p.swift_Code,
                    status = p.status,
                    created_On = p.created_On,
                    verified_On = p.verified_On,
                    submitted_To_Swift_On = p.submitted_To_Swift_On
                })
                .ToListAsync();

            return Ok(payments);
        }
    }
}

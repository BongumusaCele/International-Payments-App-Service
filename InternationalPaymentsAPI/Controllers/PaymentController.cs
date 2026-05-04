using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.swift_Code) ||
                dto.swift_Code.Length < 8 ||
                dto.swift_Code.Length > 11)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid SWIFT code. It must be between 8 and 11 characters."
                });
            }

            var customer = await _context.Customers
                .Include(c => c.Currency)
                .FirstOrDefaultAsync(c => c.customer_Id == dto.customer_Id);

            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found." });

            var beneficiary = await _context.Beneficiaries
                .Include(b => b.Currency)
                .FirstOrDefaultAsync(b =>
                    b.beneficiary_Id == dto.beneficiary_Id &&
                    b.customer_Id == dto.customer_Id);

            if (beneficiary == null)
                return NotFound(new { success = false, message = "Beneficiary not found for this customer." });

            var fromCurrency = customer.Currency;
            var toCurrency = beneficiary.Currency;

            if (fromCurrency == null || toCurrency == null)
                return BadRequest(new { success = false, message = "Currency information is missing." });

            decimal convertedAmount;

            if (fromCurrency.currency_Id == toCurrency.currency_Id)
            {
                convertedAmount = dto.amount;
            }
            else
            {
                convertedAmount = dto.amount / toCurrency.exchange_Rate;
            }

            var payment = new PaymentModel
            {
                customer_Id = customer.customer_Id,
                beneficiary_Id = beneficiary.beneficiary_Id,
                from_Currency_Id = fromCurrency.currency_Id,
                to_Currency_Id = toCurrency.currency_Id,
                amount = dto.amount,
                exchange_Rate_Used = toCurrency.exchange_Rate,
                converted_Amount = Math.Round(convertedAmount, 2),

                payment_Provider = string.IsNullOrWhiteSpace(dto.payment_Provider)
                    ? "SWIFT"
                    : dto.payment_Provider,

                swift_Code = dto.swift_Code.ToUpper(),

                payment_Reference = dto.payment_Reference,
                payment_Reason = dto.payment_Reason,
                status = "Pending",
                created_On = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var audit = new AuditLogModel
            {
                customer_Id = customer.customer_Id,
                action_Type = "CREATE_PAYMENT",
                table_Name = "tblPayment",
                record_Id = payment.payment_Id,
                new_Value = $"Amount: {payment.amount}, Converted: {payment.converted_Amount}, Provider: {payment.payment_Provider}, SWIFT: {payment.swift_Code}",
                details = "Customer created a payment request.",
                created_On = DateTime.Now
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();

            return Ok(new PaymentResponseDto
            {
                success = true,
                message = "Payment created successfully.",
                payment_Id = payment.payment_Id,
                customer_Id = payment.customer_Id,
                beneficiary_Id = payment.beneficiary_Id,
                amount = payment.amount,
                from_Currency = fromCurrency.currency_Code,
                to_Currency = toCurrency.currency_Code,
                exchange_Rate_Used = payment.exchange_Rate_Used,
                converted_Amount = payment.converted_Amount,
                status = payment.status
            });

        }
        [HttpGet("audit/customer/{customerId}")]
        public async Task<IActionResult> GetPaymentAuditByCustomerId(int customerId)
        {
            var auditLogs = await _context.AuditLogs
                .Where(a => a.customer_Id == customerId && a.table_Name == "tblPayment")
                .OrderByDescending(a => a.created_On)
                .Select(a => new
                {
                    a.audit_Id,
                    a.customer_Id,
                    a.action_Type,
                    a.table_Name,
                    a.record_Id,
                    a.old_Value,
                    a.new_Value,
                    a.details,
                    a.created_On
                })
                .ToListAsync();

            return Ok(auditLogs);
        }
    }
}
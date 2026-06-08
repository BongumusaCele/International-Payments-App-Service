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
    public class BeneficiaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BeneficiaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBeneficiary(CreateBeneficiaryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int authenticatedCustomerId = User.GetCustomerId();
            if (dto.customer_Id != authenticatedCustomerId)
                return Forbid();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.customer_Id == dto.customer_Id);

            if (customer == null)
                return NotFound(new { success = false, message = "Customer not found." });

            int currencyId = dto.currency_Id > 0 ? dto.currency_Id : customer.currency_Id;
            bool currencyExists = await _context.Currencies
                .AnyAsync(c => c.currency_Id == currencyId);

            if (!currencyExists)
                return NotFound(new { success = false, message = "Currency not found." });

            var beneficiary = new BeneficiaryModel
            {
                customer_Id = dto.customer_Id,
                currency_Id = currencyId,
                beneficiary_Name = dto.beneficiary_Name,
                bank_Name = dto.bank_Name,
                account_Number = dto.account_Number,
                swift_Code = dto.swift_Code.ToUpperInvariant(),
                country = dto.country
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Beneficiary added successfully.",
                beneficiary_Id = beneficiary.beneficiary_Id
            });
        }

        [HttpPut("update/{beneficiaryId}")]
        public async Task<IActionResult> UpdateBeneficiary(int beneficiaryId, UpdateBeneficiaryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.beneficiary_Id == beneficiaryId);

            if (beneficiary == null)
                return NotFound(new { success = false, message = "Beneficiary not found." });

            if (beneficiary.customer_Id != User.GetCustomerId())
                return Forbid();

            if (dto.currency_Id > 0)
            {
                bool currencyExists = await _context.Currencies
                    .AnyAsync(c => c.currency_Id == dto.currency_Id);

                if (!currencyExists)
                    return NotFound(new { success = false, message = "Currency not found." });

                beneficiary.currency_Id = dto.currency_Id;
            }

            beneficiary.beneficiary_Name = dto.beneficiary_Name;
            beneficiary.bank_Name = dto.bank_Name;
            beneficiary.account_Number = dto.account_Number;
            beneficiary.swift_Code = dto.swift_Code.ToUpperInvariant();
            beneficiary.country = dto.country;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Beneficiary updated successfully."
            });
        }

        [HttpDelete("delete/{beneficiaryId}")]
        public async Task<IActionResult> DeleteBeneficiary(int beneficiaryId)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.beneficiary_Id == beneficiaryId);

            if (beneficiary == null)
                return NotFound(new { success = false, message = "Beneficiary not found." });

            if (beneficiary.customer_Id != User.GetCustomerId())
                return Forbid();

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Beneficiary deleted successfully."
            });
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetBeneficiariesByCustomer(int customerId)
        {
            if (customerId != User.GetCustomerId())
                return Forbid();

            var beneficiaries = await _context.Beneficiaries
                .Include(b => b.Currency)
                .Where(b => b.customer_Id == customerId)
                .Select(b => new BeneficiaryResponseDto
                {
                    beneficiary_Id = b.beneficiary_Id,
                    customer_Id = b.customer_Id,
                    currency_Id = b.currency_Id,
                    currency_Code = b.Currency.currency_Code,
                    currency_Name = b.Currency.currency_Name,
                    beneficiary_Name = b.beneficiary_Name,
                    bank_Name = b.bank_Name,
                    account_Number = b.account_Number,
                    swift_Code = b.swift_Code,
                    country = b.country
                })
                .ToListAsync();

            return Ok(beneficiaries);
        }
    }
}

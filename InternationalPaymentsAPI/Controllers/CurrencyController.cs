using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CurrencyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return Ok(currencies);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCurrency(CreateCurrencyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currencyExists = await _context.Currencies
                .AnyAsync(c => c.currency_Code == dto.currency_Code);

            if (currencyExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Currency code already exists."
                });
            }

            var currency = new CurrencyModel
            {
                currency_Name = dto.currency_Name,
                currency_Code = dto.currency_Code,
                exchange_Rate = dto.exchange_Rate
            };

            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Currency created successfully.",
                currency_Id = currency.currency_Id
            });
        }
    }
}
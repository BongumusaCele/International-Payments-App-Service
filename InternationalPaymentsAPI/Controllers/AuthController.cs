using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Helpers;
using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Email check (only if provided)
            if (!string.IsNullOrWhiteSpace(dto.email_Address))
            {
                var emailExists = await _context.Customers
                    .AnyAsync(c => c.email_Address == dto.email_Address);

                if (emailExists)
                    return BadRequest(new { success = false, message = "Email already exists." });
            }

            // Username check
            if (await _context.Customers.AnyAsync(c => c.username == dto.username))
                return BadRequest(new { success = false, message = "Username already exists." });

            // Account number check
            if (await _context.Customers.AnyAsync(c => c.account_Number == dto.account_Number))
                return BadRequest(new { success = false, message = "Account number already exists." });

            // ID check
            if (await _context.Customers.AnyAsync(c => c.id_Number == dto.id_Number))
                return BadRequest(new { success = false, message = "ID number already exists." });

            // Currency check
            var currencyExists = await _context.Currencies
                .AnyAsync(c => c.currency_Id == dto.currency_Id);

            if (!currencyExists)
                return BadRequest(new { success = false, message = "Invalid currency." });

            // Hash password
            string hashedPassword = PasswordHelper.HashPassword(dto.password);

            var customer = new CustomerModel
            {
                first_Name = dto.first_Name,
                last_Name = dto.last_Name,
                id_Number = dto.id_Number,
                email_Address = dto.email_Address,
                account_Number = dto.account_Number,
                currency_Id = dto.currency_Id,
                username = dto.username,
                password_Hash = hashedPassword,
                CreatedOn = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new RegisterResponseDto
            {
                success = true,
                message = "Registration successful.",
                customer_Id = customer.customer_Id,
                username = customer.username
            });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string hashedPassword = PasswordHelper.HashPassword(dto.password);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.username == dto.username &&
                    c.account_Number == dto.account_Number &&
                    c.password_Hash == hashedPassword);

            if (customer == null)
            {
                return Unauthorized(new LoginResponseDto
                {
                    success = false,
                    message = "Invalid username, account number or password."
                });
            }

            // Create session
            var session = new CustomerSessionModel
            {
                customer_Id = customer.customer_Id,
                login_Time = DateTime.Now,
                is_Active = true
            };

            _context.CustomerSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new LoginResponseDto
            {
                success = true,
                message = "Login successful.",
                customer_Id = customer.customer_Id,
                full_Name = customer.first_Name + " " + customer.last_Name,
                username = customer.username,
                account_Number = customer.account_Number,
                currency_Id = customer.currency_Id
            });
        }

        // ================= LOGOUT =================
        [HttpPost("logout/{customerId}")]
        public async Task<IActionResult> Logout(int customerId)
        {
            var session = await _context.CustomerSessions
                .Where(s => s.customer_Id == customerId && s.is_Active)
                .OrderByDescending(s => s.login_Time)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No active session found."
                });
            }

            session.logout_Time = DateTime.Now;
            session.is_Active = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Logout successful."
            });
        }
    }
}
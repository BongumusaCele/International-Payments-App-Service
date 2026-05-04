using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Extensions;
using InternationalPaymentsAPI.Helpers;
using InternationalPaymentsAPI.Models;
using InternationalPaymentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const int MfaExpiryMinutes = 10;
        private const int SessionExpiryHours = 8;
        private const int MaxMfaAttempts = 5;

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IEmailSender emailSender, ILogger<AuthController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrWhiteSpace(dto.email_Address))
            {
                var emailExists = await _context.Customers
                    .AnyAsync(c => c.email_Address == dto.email_Address);

                if (emailExists)
                    return BadRequest(new { success = false, message = "Email already exists." });
            }

            if (await _context.Customers.AnyAsync(c => c.username == dto.username))
                return BadRequest(new { success = false, message = "Username already exists." });

            if (await _context.Customers.AnyAsync(c => c.account_Number == dto.account_Number))
                return BadRequest(new { success = false, message = "Account number already exists." });

            if (await _context.Customers.AnyAsync(c => c.id_Number == dto.id_Number))
                return BadRequest(new { success = false, message = "ID number already exists." });

            int currencyId = dto.currency_Id;
            if (currencyId <= 0 && !string.IsNullOrWhiteSpace(dto.preferred_Currency))
            {
                currencyId = await _context.Currencies
                    .Where(c => c.currency_Code == dto.preferred_Currency)
                    .Select(c => c.currency_Id)
                    .FirstOrDefaultAsync();
            }

            var currencyExists = await _context.Currencies
                .AnyAsync(c => c.currency_Id == currencyId);

            if (!currencyExists)
                return BadRequest(new { success = false, message = "Invalid currency." });

            var customer = new CustomerModel
            {
                first_Name = dto.first_Name,
                last_Name = dto.last_Name,
                id_Number = dto.id_Number,
                email_Address = dto.email_Address,
                account_Number = dto.account_Number,
                currency_Id = currencyId,
                username = dto.username,
                password_Hash = PasswordHelper.HashPassword(dto.password),
                CreatedOn = DateTime.UtcNow
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

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? submittedPassword = dto.GetSubmittedPassword();
            if (string.IsNullOrWhiteSpace(submittedPassword))
            {
                return BadRequest(new LoginResponseDto
                {
                    success = false,
                    message = "Password is required."
                });
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.username == dto.username &&
                    c.account_Number == dto.account_Number);

            if (customer == null)
            {
                _logger.LogWarning("Login failed for username {Username} and account number {AccountNumber}: customer not found.", dto.username, dto.account_Number);
                return Unauthorized(new LoginResponseDto
                {
                    success = false,
                    message = "Invalid username, account number, or password."
                });
            }

            if (!PasswordHelper.VerifyPassword(submittedPassword, customer.password_Hash, out bool needsRehash))
            {
                _logger.LogWarning("Login failed for customer {CustomerId}: invalid password.", customer.customer_Id);
                return Unauthorized(new LoginResponseDto
                {
                    success = false,
                    message = "Invalid username, account number, or password."
                });
            }

            if (needsRehash)
            {
                customer.password_Hash = PasswordHelper.HashPassword(submittedPassword);
            }

            if (string.IsNullOrWhiteSpace(customer.email_Address))
            {
                _logger.LogWarning("Login failed for customer {CustomerId}: no email address for MFA.", customer.customer_Id);
                return BadRequest(new LoginResponseDto
                {
                    success = false,
                    message = "This account does not have an email address for MFA."
                });
            }

            string otpCode = SecurityTokenHelper.CreateOtpCode();
            var challenge = new MfaChallengeModel
            {
                mfa_Challenge_Id = Guid.NewGuid(),
                customer_Id = customer.customer_Id,
                otp_Code_Hash = SecurityTokenHelper.HashSecret(otpCode),
                created_On = DateTime.UtcNow,
                expires_On = DateTime.UtcNow.AddMinutes(MfaExpiryMinutes),
                attempt_Count = 0
            };

            _context.MfaChallenges.Add(challenge);
            await _context.SaveChangesAsync();

            await _emailSender.SendOtpAsync(customer.email_Address, otpCode);
            _logger.LogInformation("MFA OTP sent for customer {CustomerId} to {Email}.", customer.customer_Id, customer.email_Address);

            return Ok(new LoginResponseDto
            {
                success = true,
                requires_Mfa = true,
                message = "Verification code sent to your email address.",
                mfa_Challenge_Id = challenge.mfa_Challenge_Id,
                customer_Id = customer.customer_Id,
                username = customer.username
            });
        }

        [HttpPost("verify-mfa")]
        [AllowAnonymous]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> VerifyMfa(VerifyMfaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var challenge = await _context.MfaChallenges
                .Include(c => c.Customer)
                .ThenInclude(c => c.Currency)
                .FirstOrDefaultAsync(c => c.mfa_Challenge_Id == dto.mfa_Challenge_Id);

            if (challenge == null ||
                challenge.consumed_On != null ||
                challenge.expires_On <= DateTime.UtcNow ||
                challenge.attempt_Count >= MaxMfaAttempts)
            {
                return Unauthorized(new LoginResponseDto
                {
                    success = false,
                    message = "Invalid or expired verification code."
                });
            }

            if (!SecurityTokenHelper.SecretMatches(dto.otp_Code, challenge.otp_Code_Hash))
            {
                challenge.attempt_Count += 1;
                await _context.SaveChangesAsync();

                return Unauthorized(new LoginResponseDto
                {
                    success = false,
                    message = "Invalid or expired verification code."
                });
            }

            challenge.consumed_On = DateTime.UtcNow;

            string sessionToken = SecurityTokenHelper.CreateSessionToken();
            var session = new CustomerSessionModel
            {
                customer_Id = challenge.customer_Id,
                login_Time = DateTime.UtcNow,
                is_Active = true,
                session_Token_Hash = SecurityTokenHelper.HashSecret(sessionToken),
                expires_On = DateTime.UtcNow.AddHours(SessionExpiryHours)
            };

            _context.CustomerSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new LoginResponseDto
            {
                success = true,
                requires_Mfa = false,
                message = "Login successful.",
                token = sessionToken,
                token_Expires_On = session.expires_On,
                customer_Id = challenge.Customer.customer_Id,
                full_Name = challenge.Customer.first_Name + " " + challenge.Customer.last_Name,
                username = challenge.Customer.username,
                account_Number = challenge.Customer.account_Number,
                currency_Id = challenge.Customer.currency_Id,
                preferred_Currency = challenge.Customer.Currency?.currency_Code
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            int sessionId = User.GetSessionId();

            var session = await _context.CustomerSessions
                .FirstOrDefaultAsync(s => s.session_Id == sessionId && s.is_Active);

            if (session == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No active session found."
                });
            }

            session.logout_Time = DateTime.UtcNow;
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

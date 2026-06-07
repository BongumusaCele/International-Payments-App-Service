using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.DTOs;
using InternationalPaymentsAPI.Helpers;
using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Controllers
{
    [Route("api/employee/auth")]
    [ApiController]
    public class EmployeeAuthController : ControllerBase
    {
        private const int SessionExpiryHours = 8;

        private readonly ApplicationDbContext _context;

        public EmployeeAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(EmployeeLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.username == dto.username && e.is_Active);

            if (employee == null)
            {
                return Unauthorized(new EmployeeLoginResponseDto
                {
                    success = false,
                    message = "Invalid username or password."
                });
            }

            if (!PasswordHelper.VerifyPassword(dto.password, employee.password_Hash, out bool needsRehash))
            {
                return Unauthorized(new EmployeeLoginResponseDto
                {
                    success = false,
                    message = "Invalid username or password."
                });
            }

            if (needsRehash)
                employee.password_Hash = PasswordHelper.HashPassword(dto.password);

            string sessionToken = SecurityTokenHelper.CreateSessionToken();

            Console.WriteLine($"LOGIN TOKEN : {sessionToken}");
            Console.WriteLine($"LOGIN HASH  : {SecurityTokenHelper.HashSecret(sessionToken)}");

            var session = new EmployeeSessionModel
            {
                employee_Id = employee.employee_Id,
                login_Time = DateTime.UtcNow,
                is_Active = true,
                session_Token_Hash = SecurityTokenHelper.HashSecret(sessionToken),
                expires_On = DateTime.UtcNow.AddHours(SessionExpiryHours)
            };

            Console.WriteLine($"STORED HASH : {session.session_Token_Hash}");

            _context.EmployeeSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new EmployeeLoginResponseDto
            {
                success = true,
                message = "Employee login successful.",
                token = sessionToken,
                token_Expires_On = session.expires_On,
                employee_Id = employee.employee_Id,
                username = employee.username,
                full_Name = employee.full_Name
            });
        }
        [HttpGet("generate-hash")]
        [AllowAnonymous]
        public IActionResult GenerateHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password is required."
                });
            }

            string hash = PasswordHelper.HashPassword(password);

            return Ok(new
            {
                password,
                hash
            });
        }
       
    }
}
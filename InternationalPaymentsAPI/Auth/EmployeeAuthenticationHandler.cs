using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace InternationalPaymentsAPI.Auth
{
    public class EmployeeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "EmployeeBearer";

        private readonly ApplicationDbContext _context;

        public EmployeeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ApplicationDbContext context)
            : base(options, logger, encoder)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? authorization = Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authorization) ||
                !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            string token = authorization["Bearer ".Length..].Trim();

            string tokenHash = SecurityTokenHelper.HashSecret(token);


            var session = await _context.EmployeeSessions
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s =>
                    s.session_Token_Hash == tokenHash &&
                    s.is_Active &&
                    s.expires_On > DateTime.UtcNow &&
                    s.Employee.is_Active);


            if (session == null)
                return AuthenticateResult.Fail("Invalid or expired employee session.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, session.employee_Id.ToString()),
                new(ClaimTypes.Name, session.Employee.username),
                new("employee_session_id", session.employee_Session_Id.ToString()),
                new(ClaimTypes.Role, "Employee")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        
    }
}

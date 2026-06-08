using System.Security.Claims;
using System.Text.Encodings.Web;
using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InternationalPaymentsAPI.Auth
{
    public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "SessionBearer";

        private readonly ApplicationDbContext _context;

        public SessionAuthenticationHandler(
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
            if (string.IsNullOrWhiteSpace(token))
            {
                return AuthenticateResult.Fail("Missing bearer token.");
            }

            string tokenHash = SecurityTokenHelper.HashSecret(token);
            DateTime utcNow = DateTime.UtcNow;

            var session = await _context.CustomerSessions
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s =>
                    s.session_Token_Hash == tokenHash &&
                    s.is_Active &&
                    s.expires_On > utcNow);

            if (session == null)
            {
                return AuthenticateResult.Fail("Invalid or expired session.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, session.customer_Id.ToString()),
                new(ClaimTypes.Name, session.Customer.username),
                new("session_id", session.session_Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}

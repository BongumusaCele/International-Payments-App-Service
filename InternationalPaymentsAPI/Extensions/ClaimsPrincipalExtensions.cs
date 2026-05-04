using System.Security.Claims;

namespace InternationalPaymentsAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetCustomerId(this ClaimsPrincipal user)
        {
            string? customerId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(customerId, out int parsedCustomerId) ? parsedCustomerId : 0;
        }

        public static int GetSessionId(this ClaimsPrincipal user)
        {
            string? sessionId = user.FindFirstValue("session_id");
            return int.TryParse(sessionId, out int parsedSessionId) ? parsedSessionId : 0;
        }
    }
}

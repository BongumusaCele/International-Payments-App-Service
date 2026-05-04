using System.Security.Cryptography;
using System.Text;

namespace InternationalPaymentsAPI.Helpers
{
    public static class SecurityTokenHelper
    {
        public static string CreateSessionToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public static string CreateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        public static string HashSecret(string value)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }

        public static bool SecretMatches(string value, string storedHash)
        {
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(HashSecret(value)),
                Encoding.UTF8.GetBytes(storedHash));
        }
    }
}

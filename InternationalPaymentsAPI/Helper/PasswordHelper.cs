using System.Security.Cryptography;
using System.Text;

namespace InternationalPaymentsAPI.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;
        private const string Prefix = "PBKDF2";

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash, out bool needsRehash)
        {
            needsRehash = false;

            if (storedHash.StartsWith($"{Prefix}$", StringComparison.Ordinal))
            {
                return VerifyPbkdf2Password(password, storedHash, out needsRehash);
            }

            needsRehash = true;
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(HashLegacySha256(password)),
                Encoding.UTF8.GetBytes(storedHash));
        }

        private static bool VerifyPbkdf2Password(string password, string storedHash, out bool needsRehash)
        {
            needsRehash = false;
            string[] parts = storedHash.Split('$');

            if (parts.Length != 4 || !int.TryParse(parts[1], out int iterations))
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            needsRehash = iterations < Iterations;
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        private static string HashLegacySha256(string password)
        {
            using SHA256 sha256 = SHA256.Create();

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}

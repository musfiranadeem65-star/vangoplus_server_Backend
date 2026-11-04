using System.Security.Cryptography;
using System.Text;

namespace vangoplus_server.Application.Auth
{
    /// <summary>
    /// Hashes and checks passwords with BCrypt.
    ///
    /// Accounts created before this change were stored as a plain SHA-256 hash
    /// with no salt, which is fast enough to attack with a word list. Those
    /// hashes are still accepted at sign-in so nobody is locked out, and the
    /// row is quietly re-hashed with BCrypt the next time that person signs in
    /// (see <see cref="NeedsUpgrade"/>). A legacy hash is never written again.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            if (IsLegacyHash(storedHash))
                return FixedTimeEquals(LegacyHash(password), storedHash);

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Not a hash this code can read; treat it as a failed sign-in
                // rather than a server error.
                return false;
            }
        }

        /// <summary>True when the stored hash should be replaced after a
        /// successful sign-in.</summary>
        public static bool NeedsUpgrade(string storedHash) => IsLegacyHash(storedHash);

        // A BCrypt hash always starts with $2 and is 60 characters long, so
        // anything else is one of the old SHA-256 values.
        private static bool IsLegacyHash(string storedHash) =>
            !storedHash.StartsWith("$2", StringComparison.Ordinal);

        private static string LegacyHash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool FixedTimeEquals(string a, string b) =>
            CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(a),
                Encoding.UTF8.GetBytes(b)
            );
    }
}

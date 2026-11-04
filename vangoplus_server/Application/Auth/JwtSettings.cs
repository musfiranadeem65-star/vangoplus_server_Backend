using System.Text;

namespace vangoplus_server.Application.Auth
{
    /// <summary>
    /// Where the signing key comes from, and how long a token lasts.
    ///
    /// The key is a secret. It is read from configuration (an environment
    /// variable, or appsettings.Development.json, which is not committed), and
    /// outside development the application refuses to start without one. A key
    /// committed to a public repository is the same as having no key at all.
    /// </summary>
    public class JwtSettings
    {
        public const string DevelopmentOnlyKey =
            "vangoplus-development-only-signing-key-do-not-use-in-production";

        public string Key { get; init; } = DevelopmentOnlyKey;
        public string Issuer { get; init; } = "vangoplus";
        public string Audience { get; init; } = "vangoplus-client";
        public int ExpiryHours { get; init; } = 12;

        public static JwtSettings Load(IConfiguration config, bool isDevelopment)
        {
            var key = config["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                if (!isDevelopment)
                    throw new InvalidOperationException(
                        "Jwt:Key is not configured. Set it with " +
                        "dotnet user-secrets set \"Jwt:Key\" \"<a long random string>\" " +
                        "or the Jwt__Key environment variable."
                    );

                key = DevelopmentOnlyKey;
            }

            // HMAC-SHA256 needs at least 256 bits of key material, and the
            // handler throws an unhelpful error when the key is shorter.
            if (Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException(
                    "Jwt:Key must be at least 32 characters long."
                );

            return new JwtSettings
            {
                Key = key,
                Issuer = config["Jwt:Issuer"] ?? "vangoplus",
                Audience = config["Jwt:Audience"] ?? "vangoplus-client",
                ExpiryHours = int.TryParse(config["Jwt:ExpiryHours"], out var h) ? h : 12
            };
        }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Blueprint.API.Logic.Helpers
{
    /// <summary>
    /// Implements password verification and hashing using ASP.NET Core's PasswordHasher.
    /// </summary>
    public sealed class PasswordVerifier : IPasswordVerifier
    {
        private readonly PasswordHasher<string> _hasher = new();
        /// <summary>
        /// Verifies a plaintext password against a stored hash.
        /// </summary>
        /// <param name="username">The username used as salt context.</param>
        /// <param name="storedHash">The stored password hash.</param>
        /// <param name="plaintextPassword">The plaintext password input.</param>
        /// <returns>True if verification succeeds; otherwise false.</returns>
        public bool Verify(string username, string storedHash, string plaintextPassword)
        {
            var result = _hasher.VerifyHashedPassword(username, storedHash, plaintextPassword);
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Hashes a plaintext password using a salt contextualized by username.
        /// </summary>
        /// <param name="username">The username used as salt context.</param>
        /// <param name="plaintextPassword">The plaintext password to hash.</param>
        /// <returns>The salted hash string.</returns>
        public string Hash(string username, string plaintextPassword)
        {
            return _hasher.HashPassword(username, plaintextPassword);
        }
    }
}

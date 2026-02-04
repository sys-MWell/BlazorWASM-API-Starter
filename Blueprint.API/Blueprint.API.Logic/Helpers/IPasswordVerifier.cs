namespace Blueprint.API.Logic.Helpers
{
    /// <summary>
    /// Provides password hashing and verification services for authentication workflows.
    /// </summary>
    public interface IPasswordVerifier
    {
        /// <summary>
        /// Verifies a plaintext password against a stored hash.
        /// </summary>
        /// <param name="username">The username used as salt context.</param>
        /// <param name="storedHash">The stored password hash.</param>
        /// <param name="plaintextPassword">The plaintext password input.</param>
        /// <returns>True if verification succeeds; otherwise false.</returns>
        bool Verify(string username, string storedHash, string plaintextPassword);
        /// <summary>
        /// Hashes a plaintext password using a salt contextualized by username.
        /// </summary>
        /// <param name="username">The username used as salt context.</param>
        /// <param name="plaintextPassword">The plaintext password to hash.</param>
        /// <returns>The salted hash string.</returns>
        string Hash(string username, string plaintextPassword);
    }
}

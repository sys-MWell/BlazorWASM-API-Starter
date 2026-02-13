namespace Blueprint.API.Logic.Validation
{
    /// <summary>
    /// Constants for validation rules used across validators.
    /// Centralizes configuration for easy maintenance and consistency.
    /// </summary>
    public static class ValidationConstants
    {
        /// <summary>
        /// Username validation constants.
        /// </summary>
        public static class Username
        {
            /// <summary>Minimum length for usernames.</summary>
            public const int MinLength = 3;

            /// <summary>Maximum length for usernames.</summary>
            public const int MaxLength = 50;

            /// <summary>Regex pattern for allowed username characters (alphanumeric, underscore, hyphen).</summary>
            public const string AllowedCharactersPattern = @"^[a-zA-Z0-9_-]+$";

            /// <summary>Error message for invalid characters.</summary>
            public const string AllowedCharactersMessage = "Username can only contain letters, numbers, underscores, and hyphens";
        }

        /// <summary>
        /// Password validation constants.
        /// </summary>
        public static class Password
        {
            /// <summary>Minimum length for passwords.</summary>
            public const int MinLength = 8;

            /// <summary>Maximum length for passwords.</summary>
            public const int MaxLength = 128;

            /// <summary>Regex pattern requiring at least one uppercase letter.</summary>
            public const string UppercasePattern = @"[A-Z]";

            /// <summary>Regex pattern requiring at least one lowercase letter.</summary>
            public const string LowercasePattern = @"[a-z]";

            /// <summary>Regex pattern requiring at least one digit.</summary>
            public const string DigitPattern = @"[0-9]";

            /// <summary>Regex pattern requiring at least one special character.</summary>
            public const string SpecialCharacterPattern = @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]";

            /// <summary>Error message for missing uppercase letter.</summary>
            public const string UppercaseMessage = "Password must contain at least one uppercase letter";

            /// <summary>Error message for missing lowercase letter.</summary>
            public const string LowercaseMessage = "Password must contain at least one lowercase letter";

            /// <summary>Error message for missing digit.</summary>
            public const string DigitMessage = "Password must contain at least one number";

            /// <summary>Error message for missing special character.</summary>
            public const string SpecialCharacterMessage = "Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;':\"\\,.<>/?)";
        }
    }
}

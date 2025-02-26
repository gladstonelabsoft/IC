using System.Text.RegularExpressions;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    /// <summary>
    /// Provides methods for email validation.
    /// </summary>
    public static class EmailValidator
    {
        private const string SimpleEmailPattern = @"^[^@]+@[^@]+$";

        /// <summary>
        /// Determines whether the given string is a valid email format.
        /// </summary>
        /// <param name="email">The email string to validate.</param>
        /// <returns>true if the email format is valid; otherwise, false.</returns>
        public static bool IsValidFormat(string email)
        {
            return Regex.IsMatch(email, SimpleEmailPattern);
        }
    }
}
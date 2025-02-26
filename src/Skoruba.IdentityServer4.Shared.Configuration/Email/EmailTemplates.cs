
using System.Text;

namespace Skoruba.IdentityServer4.Shared.Configuration.Email
{
    public static class EmailTemplates
    {
        public static string GetPasswordResetTemplate(string resetLink, string productName = "Identity Server")
        {
            var builder = new StringBuilder();
            builder.AppendLine($"<h2>Reset Your {productName} Password</h2>");
            builder.AppendLine("<p>A password reset was requested for your account.</p>");
            builder.AppendLine($"<p>Please click the link below to reset your password:</p>");
            builder.AppendLine($"<p><a href='{resetLink}'>Reset Password</a></p>");
            builder.AppendLine("<p>If you did not request this reset, please ignore this email.</p>");
            builder.AppendLine("<p>This link will expire in 1 hour.</p>");
            return builder.ToString();
        }
    }
}

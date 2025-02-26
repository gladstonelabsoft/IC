using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.HttpRequestService
{
    [ExcludeFromCodeCoverage]
    public class LabsoftHttpResponse
    {
        public readonly int StatusCode;
        public readonly string Message;
        public readonly bool IsSuccessStatusCode;
        public readonly string RequestPayload;
        public readonly string Content;

        public LabsoftHttpResponse(
            int statusCode,
            string message,
            bool isSuccessStatusCode,
            string requestPayload,
            string content)
        {
            StatusCode = statusCode;
            Message = message;
            IsSuccessStatusCode = isSuccessStatusCode;
            RequestPayload = requestPayload;
            Content = content;
        }
    }
}

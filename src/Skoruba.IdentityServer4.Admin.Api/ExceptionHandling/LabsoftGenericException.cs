using System.Diagnostics.CodeAnalysis;
using System;

namespace Skoruba.IdentityServer4.Admin.Api.ExceptionHandling
{
    [ExcludeFromCodeCoverage]
    public class LabsoftGenericException : Exception
    {
        public LabsoftGenericException(
            string domainName,
            string errorDescription) :
                base(message: $"Labsoft{domainName}ErrorDescription: {errorDescription}")
        {
        }
    }
}

﻿using Skoruba.IdentityServer4.Admin.Api.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.Api.Resources
{
    public class ApiErrorResources : IApiErrorResources
    {
        public virtual ApiError CannotSetId()
        {
            return new ApiError
            {
                Code = nameof(CannotSetId),
                Description = ApiErrorResource.CannotSetId
            };
        }

        public virtual ApiError UserNotFoundByEmailAddress()
        {
            return new ApiError
            {
                Code = nameof(UserNotFoundByEmailAddress),
                Description = ApiErrorResource.UserNotFoundByEmailAddress
            };
        }
    }
}
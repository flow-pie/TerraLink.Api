using Microsoft.AspNetCore.Http.HttpResults;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.Auth;
using TerraLink.Api.Services.Auth;

namespace TerraLink.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("api/auth")
                .WithTags("Authentication");

            group.MapPost("login",LoginAsync)
                .AllowAnonymous();
            
            return endpoints;            
        }

        public static async Task<
            Results<
                Ok<LoginResponse>,
                UnauthorizedHttpResult,
                ForbidHttpResult,
                BadRequest<ErrorResponse>
            >
        > LoginAsync(
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken
        )
        {
            if(!ValidationHelper.TryValidate(
                request,
                out var validationErrors
            ))
            {
                return TypedResults.BadRequest(validationErrors.ToErrorResponse() );
            }

            var result = await authService.LoginAsync(
                request,
                cancellationToken
            );

            return result.Status switch
            {
                LoginStatus.Success
                    => TypedResults.Ok(
                        result.Response!
                    ),

                LoginStatus.InvalidCredentials
                    => TypedResults.Unauthorized(),

                LoginStatus.AccountInactive 
                    => TypedResults.Forbid(),

                LoginStatus.MfaRequired
                    => TypedResults.Forbid(),

                _ => TypedResults.Unauthorized()
            };
        }
    }
}
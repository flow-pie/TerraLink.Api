using System.Security.Claims;
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

            group.MapPost("login", LoginAsync)
                .AllowAnonymous();

            group.MapPost("/refresh", RefreshAsync)
                .AllowAnonymous();

            group.MapPost("/logout", LogoutAsync)
                .RequireAuthorization();

            return endpoints;
        }

        //handler functions

        private static async Task<
    Results<
        NoContent,
        UnauthorizedHttpResult,
        BadRequest<ErrorResponse>
    >
> LogoutAsync(
    LogoutRequest request,
    ClaimsPrincipal user,
    IAuthService authService,
    CancellationToken cancellationToken
)
        {
            if (!ValidationHelper.TryValidate(
                    request,
                    out var validationErrors))
            {
                return TypedResults.BadRequest(
                    validationErrors.ToErrorResponse()
                );
            }

            var userIdClaim = user.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!long.TryParse(
                    userIdClaim,
                    out var userId))
            {
                return TypedResults.Unauthorized();
            }

            var revoked = await authService.LogoutAsync(
                userId,
                request.RefreshToken,
                cancellationToken
            );

            if (!revoked)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.NoContent();
        }

        private static async Task<
            Results<
                Ok<RefreshTokenResponse>,
                UnauthorizedHttpResult,
                BadRequest<ErrorResponse>
            >
        > RefreshAsync(
            RefreshTokenRequest request,
            IAuthService authService,
            CancellationToken cancellationToken
        )
        {
            if (!ValidationHelper.TryValidate(
            request,
            out var validationErrors))
            {
                return TypedResults.BadRequest(
                    validationErrors.ToErrorResponse()
                );
            }

            var response =
                await authService.RefreshTokenAsync(
                    request,
                    cancellationToken
                );
            return response is null
                ? TypedResults.Unauthorized() : TypedResults.Ok(response);
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
            if (!ValidationHelper.TryValidate(
                request,
                out var validationErrors
            ))
            {
                return TypedResults.BadRequest(validationErrors.ToErrorResponse());
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
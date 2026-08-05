using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.Auth;
using TerraLink.Api.Services.Auth;

/// <summary>
/// read thing that only exist at HTTP level
/// This file contains handlers whose job is to run validation
/// call exactly one service method
/// Map whatever that service returns into a valid HTTP status code
/// </summary>
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

            group.MapPost("/forgot-password", ForgotPasswordAsync)
                .AllowAnonymous();

            group.MapPost("reset-password", ResetPasswordAsync)
                .AllowAnonymous();

            return endpoints;
        }

        private static async Task<
        Results<
            NoContent,
            BadRequest<ErrorResponse>
        >
        > ResetPasswordAsync(
            ResetPasswordRequest request,
            IAuthService authService,
            CancellationToken cancellationToken)
        {
            if (!ValidationHelper.TryValidate(
                    request,
                    out var validationErrors))
            {
                return TypedResults.BadRequest(
                    validationErrors.ToErrorResponse()
                );
            }

            var resetSucceeded =
                await authService.ResetPasswordAsync(
                    request,
                    cancellationToken
                );

            if (!resetSucceeded)
            {
                return TypedResults.BadRequest(
                    new ErrorResponse(
                        "The password reset token is invalid " +
                        "or has expired.",
                        new List<string>()
                    )
                );
            }

            return TypedResults.NoContent();
        }

        private static async Task<
            Results<
                Accepted<PasswordResetRequestResponse>,
                BadRequest<ErrorResponse>
                >
            > ForgotPasswordAsync(
                ForgotPasswordRequest request,
                IAuthService authService,
                CancellationToken cancellationToken)
        {
            if(!ValidationHelper.TryValidate(request, out var validationErrors))
                return TypedResults.BadRequest(validationErrors.ToErrorResponse());

            await authService.RequestPasswordResetAsync(request, cancellationToken);

           return TypedResults.Accepted(
                uri: (string?)null,
                value: new PasswordResetRequestResponse(
                    "If an account exists for that identifier, " +
                    "reset instructions have been sent."
                )
            );
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
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.Clients;
using TerraLink.Api.Services.Clients;

namespace TerraLink.Api.Endpoints;

public static class ClientEndpoints
{
    public static IEndpointRouteBuilder
        MapClientEndpoints(
            this IEndpointRouteBuilder app)
    {
        var group =
            app.MapGroup("/api/clients")
                .WithTags("Clients").DisableAntiforgery();

        group.MapPost(
            "/register",
            RegisterAsync
        )
        .AllowAnonymous();

        return app;
    }

    private static async Task<IResult>
        RegisterAsync(
            [FromForm] 
            RegisterClientRequest request,
            ClaimsPrincipal user,
            IClientService clientService,
            CancellationToken cancellationToken
        )
    {
        if (!ValidationHelper.TryValidate(
                request,
                out var validationErrors))
        {
            return Results.BadRequest(
                validationErrors
                    .ToErrorResponse()
            );
        }

        var userIdClaim =
            user.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        long? authenticatedUserId =
            long.TryParse(
                userIdClaim,
                out var userId
            )
                ? userId
                : null;

        try
        {
            var result =
                await clientService
                    .RegisterAsync(
                        request,
                        authenticatedUserId,
                        cancellationToken
                    );

            return result.Status switch
            {
                ClientRegistrationStatus.Success
                    => Results.Created(
                        $"/api/clients/" +
                        $"{result.Response!.ClientId}",
                        result.Response
                    ),

                ClientRegistrationStatus
                    .DuplicateNationalId
                    => Results.Conflict(
                        new ErrorResponse(
                            "A client with this " +
                            "national ID already exists.",
                            new List<string>
                            {
                                nameof(
                                    request
                                    .NationalId
                                )
                            }
                        )
                    ),

                ClientRegistrationStatus
                    .DuplicatePhone
                    => Results.Conflict(
                        new ErrorResponse(
                            "A client with this " +
                            "phone number already exists.",
                            new List<string>
                            {
                                nameof(
                                    request
                                    .Phone
                                )
                            }
                        )
                    ),

                ClientRegistrationStatus
                    .DuplicateEmail
                    => Results.Conflict(
                        new ErrorResponse(
                            "An account with this " +
                            "email already exists.",
                            new List<string>
                            {
                                nameof(
                                    request
                                    .Email
                                )
                            }
                        )
                    ),

                ClientRegistrationStatus
                    .InvalidOfficer
                    => Results.Forbid(),

                _ => Results.BadRequest()
            };
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(
                new ErrorResponse(
                    exception.Message,
                    new List<string>()
                )
            );
        }
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        var group = app.MapGroup("/api/clients")
            .WithTags("Clients").DisableAntiforgery();

        //GET /api/clients/register
        group.MapPost(
            "/register",
            RegisterAsync
        )
        .AllowAnonymous();

        //GET /api/clients?page=&pageSize=
        group.MapGet("/", GetAllClientsAsync)
            .RequireAuthorization(policy => policy.RequireRole("Loan Officer"));//only loan officers can call this endpoint

        //GET /api/clients/{clientId}
        group.MapGet("/{clientId}", GetClientByIdAsync)
            .RequireAuthorization(policy => policy.RequireRole("Loan Officer", "Client"));

        return group;
    }

    private static async Task<IResult> GetClientByIdAsync(
           long clientId,
           IClientService clientService,
           ClaimsPrincipal user,
           CancellationToken cancellationToken
       )
    {
        //Get current user from claim principal
        var loggedInUserId = user.GetUserId();

        //check if logged in user is a loan officer
        var isLoanOfficer = user.IsInRole("Loan Officer");
        
        if(!isLoanOfficer && loggedInUserId != clientId)
            Results.Forbid();

        var result = await clientService.GetClientByIdAsync(clientId, cancellationToken);

        if (result is null)
            return Results.NotFound($"Client with id {clientId} doesn't exist");

        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllClientsAsync(
        int page,
        int pageSize,
        IClientService clientService,
        CancellationToken cancellationToken
    )
    {
        var clients = await clientService.GetAllClientsAsync(
            page,
            pageSize,
            cancellationToken
        );

        return Results.Ok(clients);
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
using System.Security.Claims;
using TerraLink.Api.Common;

public static class KycEndpoints
{
    public static IEndpointRouteBuilder
        MapKycEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/kyc-documents")
            .WithTags("Kyc Documents");

        //POST /api/kyc-documents/{documentId}/verify
        group.MapPost("/{documentId}/reject", VerifyDocumentAsync)
            .RequireAuthorization(policy => policy.RequireRole("Loan Officer"));

        group.MapPost("/{id}/verify", VerifyDocumentAsync)
            .RequireAuthorization(
                policy => policy.RequireRole("Loan Officer")
            );

        return group;

    }

    private static async Task<IResult> VerifyDocumentAsync(
    long id,
    IKycDocumentService kycDocumentService,
    ClaimsPrincipal principal,
    CancellationToken cancellationToken)
    {
        var officerId = principal.GetUserId();

        try
        {
            var result = await kycDocumentService
                .VerifyDocumentAsync(
                    id,
                    officerId,
                    cancellationToken
                );

            if (result is null)
                return Results.NotFound(
                    $"KYC document with id {id} doesn't exist."
                );

            return Results.Ok(result);
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(
                new
                {
                    message = ex.Message
                }
            );
        }
    }
}
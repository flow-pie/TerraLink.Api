using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;

public class KycDocumentService(
    TerraLink.Api.Data.TerraLinkDbContext dbContext
) : IKycDocumentService
{
    public async Task<IReadOnlyList<KycDocumentResponse>>
        GetClientDocumentsAsync(
            long clientId,
            CancellationToken cancellationToken)
    {
        var documents = await dbContext.KycDocuments
            .Where(d => d.ClientId == clientId)
            .OrderBy(d => d.Id)
            .Select(d => new KycDocumentResponse(
                d.Id,
                d.DocType,
                d.FileUrl,
                d.Verified,
                d.VerifiedAt
            ))
            .ToListAsync(cancellationToken);

        return documents;
    }

    public async Task<bool> IsClientOwnerAsync(
    long clientId,
    long userId,
    CancellationToken cancellationToken)
    {
        return await dbContext.Clients
            .AnyAsync(
                c => c.Id == clientId &&
                     c.UserId == userId,
                cancellationToken
            );
    }

    public async Task<KycDocumentVerifiedResponse?>
        VerifyDocumentAsync(
            long documentId,
            long officerId,
            CancellationToken cancellationToken)
    {
        var document = await dbContext.KycDocuments
            .SingleOrDefaultAsync(
                d => d.Id == documentId,
                cancellationToken
            );

        if (document is null)
            return null;

        if (document.Verified)
            throw new ConflictException(
                "Document has already been verified."
            );

        document.Verified = true;
        document.VerifiedBy = officerId;
        document.VerifiedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new KycDocumentVerifiedResponse(
            document.Id,
            document.DocType,
            document.Verified,
            document.VerifiedBy!.Value,
            document.VerifiedAt!.Value
        );
    }

}
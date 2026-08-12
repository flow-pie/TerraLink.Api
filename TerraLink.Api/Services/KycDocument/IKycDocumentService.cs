public interface IKycDocumentService
{
    Task<IReadOnlyList<KycDocumentResponse>> GetClientDocumentsAsync(
        long clientId,
        CancellationToken cancellationToken
    );

    Task<KycDocumentVerifiedResponse?> VerifyDocumentAsync(
        long documentId,
        long officerId,
        CancellationToken cancellationToken
    );

    Task<bool> IsClientOwnerAsync(
    long clientId,
    long userId,
    CancellationToken cancellationToken
);
}
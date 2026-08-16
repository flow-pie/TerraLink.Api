using TerraLink.Api.DTOs.CreditScoring;

namespace TerraLink.Api.Services.CreditScoring;

public interface ICreditScoringService
{
    Task<List<CreditHistoryResponse>> GetCreditHistoryAsync(
        long clientId,
        CancellationToken cancellationToken
    );

    Task<List<AssetResponse>> GetClientAssetsAsync(
        long clientId,
        CancellationToken cancellationToken
    );

    Task<AssetResponse?> CreateAssetAsync(
        long clientId,
        CreateAssetRequest request,
        CancellationToken cancellationToken
    );

    Task<List<IncomeAssessmentResponse>> GetIncomeAssessmentsAsync(
        long clientId,
        CancellationToken cancellationToken
    );

    Task<IncomeAssessmentResponse?> CreateIncomeAssessmentAsync(
        long applicationId,
        CreateIncomeAssessmentRequest request,
        CancellationToken cancellationToken
    );

    Task<CreditScoreResponse?> GetCreditScoreAsync(
        long clientId,
        CancellationToken cancellationToken
    );
}

using TerraLink.Api.DTOs.Users;
public interface IUserService
{
    Task<UserProfileResponse?> GetMeAsync(
        long userId,
        CancellationToken cancellationToken
    );

    Task<UserProfileResponse?> UpdateMeAsync(
        long userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken
    );

    Task<PagedResponse<OfficerListItem>> 
    GetLoanOfficersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
}
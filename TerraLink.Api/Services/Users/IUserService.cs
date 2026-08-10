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
    
    Task<UserProfileResponse?> GetUserByIdAsync(
        long userId,
        CancellationToken cancellationToken
    );
}
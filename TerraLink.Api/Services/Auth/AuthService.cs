using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Auth;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Auth
{
    public class AuthService(
        TerraLinkDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService
    ) : IAuthService
    {
        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var identifier = request.Identifier.Trim().ToLower();

            var user = await dbContext.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(
                    user => user.Username == identifier
                        || user.Email == identifier
                        || user.EmployeeNo == identifier,
                    cancellationToken
                );

            if (user is null)
                return new LoginResult(
                    LoginStatus.InvalidCredentials
                );


            var passwordResult =
                    passwordHasher.VerifyHashedPassword(
                        user,
                        user.PasswordHash,
                        request.Password
            );

            if (passwordResult == PasswordVerificationResult.Failed)
                return new LoginResult(
                    LoginStatus.InvalidCredentials
                ); // password does not match

            if (user.Status != UserStatus.ACTIVE)
                return new LoginResult(
                    LoginStatus.AccountInactive
                ); // user is not active

            var accessToken =
            jwtService.GenerateAccessToken(user);

            var refreshToken =
                jwtService.GenerateRefreshToken();

            var refreshTokenHash =
                jwtService.HashToken(
                    refreshToken
                );

            var refreshTokenEntity =
                new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash = refreshTokenHash,
                    ExpiresAt =
                        jwtService.GetRefreshTokenExpiry()
                };

            dbContext.RefreshTokens.Add(
                refreshTokenEntity
            );

            user.LastLogin = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new LoginResult
            (
                Status : LoginStatus.Success,
                Response : new LoginResponse(
                    AccessToken : accessToken,
                    RefreshToken : refreshToken,
                    ExpiresIn : 3600,
                    User : new LoginUserResponse(
                        Id : user.Id,
                        RoleName : user.Role.Name
                    )
                )
            );
        }

    }
}
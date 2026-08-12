using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Auth;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Auth
{
    public class AuthService(
        TerraLinkDbContext dbContext,
        IJwtService jwtService
    ) : IAuthService
    {
        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var identifier = request.Identifier.Trim().ToLower();

            var user = await dbContext.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u =>
                    (u.Username != null &&
                     u.Username.ToLower() == identifier) ||

                    (u.Email != null &&
                     u.Email.ToLower() == identifier) ||

                    (u.EmployeeNo != null &&
                     u.EmployeeNo.ToLower() == identifier),
                    cancellationToken);

            if (user is null)
                return new LoginResult(
                    LoginStatus.InvalidCredentials
                );


            var isValid =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash
                );

            if (!isValid)
                return new LoginResult(
                    LoginStatus.InvalidCredentials
                ); // password does not match
                

            if (user.Status != UserStatus.ACTIVE)
                return new LoginResult(
                    LoginStatus.AccountInactive
                ); // user is not active

            if (user.MfaEnabled)
            {
                var mfaToken = jwtService.GenerateMfaToken(user);

                return new LoginResult(
                    Status: LoginStatus.MfaRequired,
                    MfaResponse: new MfaChallengeResponse(
                        MfaRequired: true,
                        MfaToken: mfaToken
                    )
                );
            }

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
                Status: LoginStatus.Success,
                Response: new LoginResponse(
                    AccessToken: accessToken,
                    RefreshToken: refreshToken,
                    ExpiresIn: 3600,
                    User: new LoginUserResponse(
                        Id: user.Id,
                        RoleName: user.Role.Name
                    )
                )
            );
        }

        public async Task<bool> LogoutAsync(
            long userId,
            string refreshToken,
            CancellationToken cancellationToken
        )
        {
            var tokenHash = jwtService.HashToken(
                refreshToken
            );

            var storedToken = await dbContext.RefreshTokens
                .SingleOrDefaultAsync(
                    token =>
                        token.UserId == userId &&
                        token.TokenHash == tokenHash &&
                        token.RevokedAt == null,
                    cancellationToken
                );

            if (storedToken is null)
            {
                return false;
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            return true;
        }
        public async Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var tokenHash = jwtService.HashToken(
                request.RefreshToken
            );

            var storedToken = await dbContext.RefreshTokens
                .Include(token => token.User)
                .ThenInclude(user => user.Role)
                .SingleOrDefaultAsync(
                    token => token.TokenHash == tokenHash,
                    cancellationToken
                );

            if (storedToken is null || !storedToken.IsActive)
                return null;

            if (storedToken.User.Status != UserStatus.ACTIVE)
                return null;

            var newRefreshToken = jwtService.GenerateRefreshToken();

            var newRefreshTokenHash = jwtService.HashToken(
                newRefreshToken
            );

            storedToken.RevokedAt = DateTime.UtcNow;

            storedToken.ReplacedByTokenHash = newRefreshTokenHash;

            var replacementToken = new RefreshToken
            {
                UserId = storedToken.UserId,
                TokenHash = newRefreshToken,
                ExpiresAt = jwtService.GetRefreshTokenExpiry()
            };

            dbContext.RefreshTokens.Add(
                replacementToken
            );

            var accessToken = jwtService.GenerateAccessToken(
                storedToken.User
            );

            await dbContext.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResponse(
                AccessToken: accessToken,
                RefreshToken: newRefreshToken,
                ExpiresIn: 3600
            );
        }

        public async Task RequestPasswordResetAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken
        )
        {
            var identifier = request.Identifier.Trim();

            var user = await dbContext.Users
                .SingleOrDefaultAsync(
                    user =>
                        user.Username == identifier ||
                        user.Email == identifier ||
                        user.EmployeeNo == identifier,
                    cancellationToken
                );

            // without revealing whether the account exists to prevent privacy leaks and attacks
            if (user is null)
            {
                return;
            }

            // Invalidate unused reset tokens for this user.
            var existingTokens =
                await dbContext.PasswordResetTokens
                    .Where(token =>
                        token.UserId == user.Id &&
                        token.UsedAt == null)
                    .ToListAsync(cancellationToken);

            foreach (var token in existingTokens)
            {
                token.UsedAt = DateTime.UtcNow;
            }

            var rawToken =
                jwtService.GenerateRefreshToken();

            var tokenHash =
                jwtService.HashToken(rawToken);

            var resetToken =
                new PasswordResetToken
                {
                    UserId = user.Id,
                    TokenHash = tokenHash,
                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(15)
                };

            dbContext.PasswordResetTokens.Add(
                resetToken
            );

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            // DEVELOPMENT ONLY. TODO : Replace with an email or sms
            Console.WriteLine(
                $"Password reset token for user " +
                $"{user.Id}: {rawToken}"
            );
        }

        public async Task<bool> ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken
        )
        {
            var tokenHash =
                jwtService.HashToken(
                    request.Token
                );

            var resetToken =
                await dbContext.PasswordResetTokens
                    .Include(token => token.User)
                    .SingleOrDefaultAsync(
                        token =>
                            token.TokenHash == tokenHash,
                        cancellationToken
                    );

            if (resetToken is null ||
                !resetToken.IsUsable)
            {
                return false;
            }

            var user = resetToken.User;

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.NewPassword
                );

            user.UpdatedAt = DateTime.UtcNow;

            resetToken.UsedAt = DateTime.UtcNow;

            // Password changed:
            // invalidate every active login session.
            var activeRefreshTokens =
                await dbContext.RefreshTokens
                    .Where(token =>
                        token.UserId == user.Id &&
                        token.RevokedAt == null)
                    .ToListAsync(cancellationToken);

            foreach (var refreshToken
                in activeRefreshTokens)
            {
                refreshToken.RevokedAt =
                    DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            return true;
        }

        public async Task<LoginResponse?> VerifyMfaAsync(
            MfaVerifyRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId =
                jwtService.ValidateMfaToken(
                    request.MfaToken
                );

            if (userId is null)
            {
                return null;
            }

            var user = await dbContext.Users
                .Include(user => user.Role)
                .SingleOrDefaultAsync(
                    user =>
                        user.Id == userId.Value,
                    cancellationToken
                );

            if (user is null ||
                user.Status != UserStatus.ACTIVE ||
                !user.MfaEnabled ||
                string.IsNullOrWhiteSpace(
                    user.MfaSecret))
            {
                return null;
            }

            var secretBytes =
                OtpNet.Base32Encoding
                    .ToBytes(user.MfaSecret);

            var totp =
                new OtpNet.Totp(
                    secretBytes
                );

            var isValid =
                totp.VerifyTotp(
                    request.Code,
                    out _,
                    new OtpNet.VerificationWindow(
                        previous: 1,
                        future: 1
                    )
                );

            if (!isValid)
            {
                return null;
            }

            var accessToken =
                jwtService.GenerateAccessToken(
                    user
                );

            var refreshToken =
                jwtService.GenerateRefreshToken();

            dbContext.RefreshTokens.Add(
                new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash =
                        jwtService.HashToken(
                            refreshToken
                        ),
                    ExpiresAt =
                        jwtService
                            .GetRefreshTokenExpiry()
                }
            );

            user.LastLogin = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            return new LoginResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresIn: 3600,
                User: new LoginUserResponse(
                    Id: user.Id,
                    RoleName: user.Role.Name
                )
            );
        }
    }
}
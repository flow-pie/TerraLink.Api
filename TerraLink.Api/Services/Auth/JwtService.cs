using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Auth
{
    public class JwtService(
     IOptions<JwtOptions> options
    ) : IJwtService
    {
        private readonly JwtOptions _options = options.Value;
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? user.Email ?? user.EmployeeNo ??user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateMfaToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.SecretKey
                    ?? throw new InvalidOperationException(
                        "JWT key is missing."
                    )
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),
                new Claim(
                    "token_type",
                    "mfa"
                )
            };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64); // 64 bytes = 512 bits
            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        }

        public string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hash = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hash);
        }

        public long? ValidateMfaToken(string token)
        {
            try
            {
                var principal =
                    new JwtSecurityTokenHandler()
                        .ValidateToken(
                            token,
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer =
                                    _options.Issuer,

                                ValidateAudience = true,
                                ValidAudience =
                                    _options.Audience,

                                ValidateLifetime = true,

                                ValidateIssuerSigningKey = true,

                                IssuerSigningKey =
                                    new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(
                                            _options.SecretKey
                                            ?? throw new InvalidOperationException(
                                                "JWT key is missing."
                                            )
                                        )
                                    ),

                                ClockSkew =
                                    TimeSpan.Zero
                            },
                            out _
                        );

                var tokenType =
                    principal.FindFirst(
                        "token_type"
                    )?.Value;

                if (tokenType != "mfa")
                {
                    return null;
                }

                var userId =
                    principal.FindFirst(
                        ClaimTypes.NameIdentifier
                    )?.Value;

                return long.TryParse(
                    userId,
                    out var id
                )
                    ? id
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
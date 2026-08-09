using System.Security.Claims;

namespace TerraLink.Api.Common;

public static class ClaimPrincipalExtensions
{
    public static long GetUserId(
        this ClaimsPrincipal user
    )
    {
        var value = user.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if(value is null)
            throw new UnauthorizedAccessException(
                "User id claim not found."
        );

        return long.Parse(value);
    }
}
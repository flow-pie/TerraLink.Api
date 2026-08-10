using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Users;
using TerraLink.Api.Models;

public class UserService(
    TerraLinkDbContext dbContext
) : IUserService
{
    public async Task<UserProfileResponse?> GetMeAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken
        );

        if(user is null)
            return null;

        return new UserProfileResponse(
               Id : user.Id,
               Username : user.Username ,
               Email: user.Email,
               EmployeeNo:  user.EmployeeNo,
               RoleName: user.Role.Name,
               Status:user.Status,
               MfaEnabled: user.MfaEnabled,
               LastLogin:user.LastLogin,
               CreatedAt: user.CreatedAt
        );
    }

    public async Task<
        UserProfileResponse?
    > GetUserByIdAsync(
        long userId, 
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.Role)
            .Where(u => u.Role.Name != "client")
            .SingleOrDefaultAsync(
                u => u.Id == userId,
                cancellationToken
            );

        if(user is null)
            return null;

        return new UserProfileResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            EmployeeNo: user.EmployeeNo,
            RoleName: user.Role.Name,
            Status : user.Status,
            MfaEnabled : user.MfaEnabled,
            LastLogin: user.LastLogin,
            CreatedAt: user.CreatedAt
        );
    }

    public async Task<UserProfileResponse?> UpdateMeAsync(
        long userId, 
        UpdateProfileRequest request, 
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                        .Include(u => u.Role)
                        .SingleOrDefaultAsync(u => u.Id == userId);

        if(user is null)
            return null;

        //check if user provided the email
        //and if diffrent from current email
        if(request.Email is not null && 
            !string.Equals(
                request.Email,
                user.Email,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            //check if provided email
            //belongs to anyone else in our DB
            var emailExists = await dbContext.Users
                .AnyAsync(
                    u => u.Id != userId &&
                    u.Email == request.Email,
                    cancellationToken
                );
            if(emailExists)
                throw new ConflictException(
                    "Email is already in use"
                );
        }

        if(request.Username is not null &&
            !string.Equals(
                request.Username,
                user.Username,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            var usernameExists = await dbContext.Users
                .AnyAsync(u => 
                    u.Id != user.Id &&
                    u.Username == request.Username,
                    cancellationToken
                );

            if(usernameExists)
                throw new ConflictException(
                    "Username is already in use");


        }

        if(request.MfaEnabled == true && string.IsNullOrWhiteSpace(user.MfaSecret))
            throw new ConflictException("MFA setup must be completed before enabling MFA");
        
        if(request.Email is not null)
            user.Email = request.Email;

        if(request.Username is not null)
            user.Username = request.Username;

        if(request.MfaEnabled.HasValue)
            user.MfaEnabled = request.MfaEnabled.Value;

        user.UpdatedAt = DateTime.Now;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse
        (
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            EmployeeNo: user.EmployeeNo,
            RoleName: user.Role.Name,
            Status: user.Status,
            MfaEnabled : user.MfaEnabled,
            LastLogin: user.LastLogin,
            CreatedAt: user.CreatedAt
        );
    }
}
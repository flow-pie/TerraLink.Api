using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs;
using TerraLink.Api.Models;

namespace TerraLink.Api.Endpoints
{
    public static class UserEndpoints
    {
        private const string GetUserByIdEndpointName = "GetUserById";
        private const string LoanOfficerRole = "Loan Officer";

        public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users")
            .WithTags("Users");
            //.RequireAuthorization(); TODO: Uncomment this line to require authorization for all user endpoints.

            //GET /
            app.MapGet("/", () => "Hello World!");

            //GET /api/users/me
            group.MapGet("/me", GetMe);

            //PATCH /api/users/me
            group.MapPatch("/me", UpdateMe);


            //GET /api/users?groupId=&page=&pageSize=
            group.MapGet("/", () => GetUsers);
            //.RequireAuthorization(policy => policy.RequireRole(LoanOfficerRole)); TODO: Uncomment this line to require authorization for this endpoint.


            //GET /api/users/{id}
            group.MapGet("/{id:long}", GetUserById)
            .WithName(GetUserByIdEndpointName);
            //.RequireAuthorization(policy => policy.RequireRole(LoanOfficerRole)); TODO: Uncomment this line to require authorization for this endpoint.


            //POST api/user
            group.MapPost("/", CreateOfficer);
            //.RequireAuthorization(policy => policy.RequireRole(LoanOfficerRole)); TODO: Uncomment this line to require authorization for this endpoint.

            return group;
        }
        //PUT /users/{id}
        // usersBasePath.MapPut("/{id}", (int id, UpdateUserDto updatedUser) =>
        //     {
        //         int index = users.FindIndex(user => user.Id == id);

        //         if (index == -1) return Results.NotFound();

        //         users[index] = new UserDto(

        //             id,
        //             updatedUser.EmployeeId ?? users[index].EmployeeId,
        //             updatedUser.FullName,
        //             updatedUser.Email ?? users[index].Email,
        //             updatedUser.Phone,
        //             updatedUser.Password,
        //             updatedUser.Role,
        //             updatedUser.Status,
        //             users[index].MfaEnabled,
        //             users[index].MfaSecret ?? "",
        //             users[index].LastLogin,
        //             users[index].CreatedAt,
        //             DateTime.UtcNow
        //         );

        //         return Results.NoContent();

        //     });

        //     usersBasePath.MapDelete("/{id}", (int id) =>
        //     {
        //         int removedCount = users.RemoveAll(user => user.Id == id);
        //         return removedCount > 0 ? Results.NoContent() : Results.NotFound();

        //     });

        private static long GetCurrentUserId(ClaimsPrincipal principal)
        {
            var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Token does not contain a NameIdentifier claim.");

            return long.Parse(claim);
        }

        private static UserResponseDto ToResponseDto(User u) => new()
        {
             Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            EmployeeNo = u.EmployeeNo,
            RoleName = u.Role.Name,
            //BranchId = u.BranchId,
            //BranchName = u.Branch?.Name,
            Status = u.Status.ToString(),
            MfaEnabled = u.MfaEnabled,
            LastLogin = u.LastLogin,
            CreatedAt = u.CreatedAt,
        };

            // ---------------------------------------------------------------
        // PATCH /api/users/me
        // ---------------------------------------------------------------
        private static async Task<Results<Ok<UserResponseDto>, NotFound, Conflict<ErrorResponse>, ValidationProblem>> UpdateMe(
            UpdateMeRequestDto dto,
            ClaimsPrincipal principal,
            TerraLinkDbContext db)
        {
            if (!ValidationHelper.TryValidate(dto, out var errors))
                return TypedResults.ValidationProblem(errors.ToDictionary(
                    e => e.MemberNames.FirstOrDefault() ?? string.Empty,
                    e => new[] { e.ErrorMessage ?? "Invalid value." }));
 
            var id = GetCurrentUserId(principal);
 
            var user = await db.Users
                .Include(u => u.Role)
                //.Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == id);
 
            if (user is null) return TypedResults.NotFound();
 
            if (dto.Username is not null)
            {
                var taken = await db.Users.AnyAsync(u => u.Id != user.Id && u.Username == dto.Username);
                if (taken) return TypedResults.Conflict(new ErrorResponse("That username is already in use.", new List<string> { "username" }));
                user.Username = dto.Username;
            }
 
            if (dto.Email is not null)
            {
                var taken = await db.Users.AnyAsync(u => u.Id != user.Id && u.Email == dto.Email);
                if (taken) return TypedResults.Conflict(new ErrorResponse("That email is already in use.",new List<string> { "email" }));
                user.Email = dto.Email;
            }
 
            // Only flips the flag — provisioning mfa_secret is a separate
            // /api/auth/mfa/setup step, deliberately not touched here.
            if (dto.MfaEnabled is not null)
            {
                if (dto.MfaEnabled == true && string.IsNullOrEmpty(user.MfaSecret))
                    return TypedResults.Conflict(new ErrorResponse("Complete MFA setup before enabling it.",new List<string> { "mfa_secret" }));
                user.MfaEnabled = dto.MfaEnabled.Value;
            }
 
            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
 
            return TypedResults.Ok(ToResponseDto(user));
        }

    }



}

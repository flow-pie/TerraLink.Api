using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Users;
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
            //.RequireAuthorization();// TODO: Uncomment this line to require authorization for all user endpoints.

            //GET /
            app.MapGet("/", () => "Hello World!");

            //GET /api/users/me
            group.MapGet("/me", GetMeAsync)
                .RequireAuthorization();

            //PATCH /api/users/me
            group.MapPatch("/me", UpdateMeAsync)
                .RequireAuthorization();


            //GET /api/users?groupId=&page=&pageSize=
            group.MapGet("/", GetUsers);
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

        private static async Task<IResult>
        GetMeAsync(
            ClaimsPrincipal user,
            IUserService userService,
            CancellationToken cancellationToken
            )
        {
            //get user id using an extension method
            var userId = user.GetUserId();

            //call the service
            var profile = await userService.GetMeAsync(
                userId,
                cancellationToken
            );

            if (profile is null)
                return Results.NotFound();

            return Results.Ok(profile);
        }

        private static async Task GetUsers(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private static async Task GetUserById(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private static async Task CreateOfficer(HttpContext context)
        {
            throw new NotImplementedException();
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

        private static UserProfileResponse ToResponseDto(User u) => new UserProfileResponse
        (
            Id: u.Id,
            Username: u.Username,
            Email: u.Email,
            EmployeeNo: u.EmployeeNo,
            RoleName: u.Role.Name,
            Status: u.Status,
            MfaEnabled: u.MfaEnabled,
            LastLogin: u.LastLogin,
            CreatedAt: u.CreatedAt
        );

        // ---------------------------------------------------------------
        // PATCH /api/users/me
        // ---------------------------------------------------------------
        private static async Task<IResult> UpdateMeAsync(
                UpdateProfileRequest request,
                ClaimsPrincipal principal,
                IUserService userService,
                CancellationToken cancellationToken
                )
        {
            try
            {
                var userId = principal.GetUserId();

                var profile = await userService.UpdateMeAsync(userId, request, cancellationToken);

                if (profile is null)
                    return Results.NotFound();

                return Results.Ok(profile);

            } catch(Exception ex)

            {
                return Results.Conflict(
                    new ConflictException(ex.Message)
                );
            }

        }

    }



}

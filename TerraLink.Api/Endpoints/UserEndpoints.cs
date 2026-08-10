using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Users;
using TerraLink.Api.Models;
using TerraLink.Api.Services.Clients;

namespace TerraLink.Api.Endpoints
{
    public static class UserEndpoints
    {
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

            //GET /api/users/{userId}
            group.MapGet("/{userId}", GetUserByIdAsync)
                .RequireAuthorization(policy => policy.RequireRole("Loan Officer"));

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

        private static async Task<IResult> GetUserByIdAsync(
            long userId,
            IUserService userService,
            CancellationToken cancellationToken
        )
        {
            var result = await userService.GetUserByIdAsync(userId, cancellationToken);

            if(result is null)
                return Results.NotFound($"User id {userId} doesn't exist");

            return Results.Ok(result);
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

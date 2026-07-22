using TerraLink.Api.DTOs;

namespace TerraLink.Api.Endpoints
{
    public static class UserEndpoints
    {
        const string GetUserEndpointName = "GetUser";

        private static readonly List<UserDto> users = [
            new(
                1,
                "TL-001",
                "Josto",
                "josto@example.com",
                "123-456-7890",
                "password123",
                "User",
                "Active",
                false,
                "",
                null,
                DateTime.UtcNow,
                null
            ),

            new(
                2,
                "TL-002",
                "Alice",
                "alice@example.com",
                "987-654-3210",
                "password456",
                "Admin",
                "Active",
                true,
                "MFASecret123",
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(-1)
            ),
        ];

        public static void MapUserEndpoints(this WebApplication app)
        {

            var usersBasePath = app.MapGroup("/users"); // Route group base path for users.

            //GET /
            app.MapGet("/", () => "Hello World!");

            //GET /users
            usersBasePath.MapGet("/", () => users);


            //GET /users/{id}
            usersBasePath.MapGet("/{id}", (long id) =>
            {
                var user = users.Find(user => user.Id == id);
                return user is null ? Results.NotFound() : Results.Ok(user);
            }
            ).WithName(GetUserEndpointName);


            //POST /user
            usersBasePath.MapPost("/", (CreateUserDto newUser) =>
            {
                UserDto user = new(
                    users.Count + 1,
                    newUser.EmployeeId ?? $"TL-{users.Count + 1:D3}",
                    newUser.FullName,
                    newUser.Email ?? "",
                    newUser.Phone,
                    newUser.Password,
                    newUser.Role,
                    newUser.Status,
                    false,
                    "",
                    null,
                    DateTime.UtcNow,   
                    null
                );

                users.Add(user);

                return Results.CreatedAtRoute(GetUserEndpointName, new { id = user.Id }, user);

            });

            //PUT /users/{id}
            usersBasePath.MapPut("/{id}", (int id, UpdateUserDto updatedUser) =>
            {
                int index = users.FindIndex(user => user.Id == id);

                if (index == -1) return Results.NotFound();

                users[index] = new UserDto(

                    id,
                    updatedUser.EmployeeId ?? users[index].EmployeeId,
                    updatedUser.FullName,
                    updatedUser.Email ?? users[index].Email,
                    updatedUser.Phone,
                    updatedUser.Password,
                    updatedUser.Role,
                    updatedUser.Status,
                    users[index].MfaEnabled,
                    users[index].MfaSecret ?? "",
                    users[index].LastLogin,
                    users[index].CreatedAt,
                    DateTime.UtcNow
                );

                return Results.NoContent();

            });

            usersBasePath.MapDelete("/{id}", (int id) =>
            {
                int removedCount = users.RemoveAll(user => user.Id == id);
                return removedCount > 0 ? Results.NoContent() : Results.NotFound();

            });

        }
    }
}

using TerraLink.Api.DTOs;

const string GetUserEndpointName = "GetUser";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

List<UserDto> users = [
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

//GET /
app.MapGet("/", () => "Hello World!");

//GET /users
app.MapGet("/users", ()=> users);


//GET /users/{id}
app.MapGet("/users/{id}", (long id)=>
{
    var user = users.Find(user => user.Id == id);
    return user is null? Results.NotFound() : Results.Ok(user);
}
).WithName(GetUserEndpointName);


//POST /game
app.MapPost("/users", (CreateUserDto newUser) =>
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

    return Results.CreatedAtRoute(GetUserEndpointName, new{id = user.Id}, user);

});

//PUT /users/{id}
app.MapPut("/users/{id}", (int id, UpdateUserDto updatedUser) =>
{
    int index = users.FindIndex(user => user.Id == id);

    if(index == -1) return Results.NotFound();
    
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

app.MapDelete("/users/{id}", (int id) =>
{  
    int removedCount = users.RemoveAll(user => user.Id == id);
    return removedCount >0  ? Results.NoContent() : Results.NotFound();

});

app.Run();

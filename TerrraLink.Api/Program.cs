using TerrraLink.Api.DTOs;

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
app.MapGet("/users/{id}", (long id)=> users.Find(user => user.Id==id) );


app.Run();

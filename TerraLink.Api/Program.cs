using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TerraLink.Api.Data;
using TerraLink.Api.Endpoints;
using TerraLink.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMySql<TerraLinkDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")) ?? throw new InvalidOperationException("Could not detect server version from connection string 'DefaultConnection'.")
);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication().AddJwtBearer();   
builder.Services.AddAuthorization(
//    options =>
//{
//    options.AddPolicy("read:users", policy => policy.RequireClaim("scope", "read:users"));
//    options.AddPolicy("write:users", policy => policy.RequireClaim("scope", "write:users"));
//}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// app.MapControllers();

app.MapUserEndpoints();

//perform database migration on startup
app.DbMigrate();

app.Run();

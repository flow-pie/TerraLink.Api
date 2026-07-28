using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMySql<TerraLink.Api.Data.TerraLinkDbContext>("server=localhost;port=3306;password=;user=root;database=TerraLink", new MySqlServerVersion(new Version(8, 0, 32)));

var app = builder.Build();  

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

app.MapUserEndpoints();

//perform database migration on startup
app.DbMigrate();

app.Run();

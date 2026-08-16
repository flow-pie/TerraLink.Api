using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.Endpoints;
using TerraLink.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TerraLink.Api.Services.Auth;
using TerraLink.Api.Services.Clients;
using System.Text.Json.Serialization;
using TerraLink.Api.Services.LoanProducts;
using TerraLink.Api.Services.LoanApplications;
using TerraLink.Api.Services.Loans;
using TerraLink.Api.Services.RepaymentSchedule;
using TerraLink.Api.Services.Payments;
using TerraLink.Api.Services.Disbursements;
using TerraLink.Api.Services.LoanClosures;
using TerraLink.Api.Services.CreditScoring;
using TerraLink.Api.Services.Notifications;
using TerraLink.Api.Services.Reports;
using TerraLink.Api.Services.AuditLog;

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

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(
        JwtOptions.SectionName
        )
);

builder.Services.AddScoped<IJwtService, JwtService>();

//The values are needed by bearer auth to validate icoming token.
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
var jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found in configuration.");
var jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience not found in configuration.");

builder.Services
    .AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Set clock skew to zero for immediate expiration
    };
});

//json serializer
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKycDocumentService, KycDocumentService>();
builder.Services.AddScoped<ILoanProductService, LoanProductService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IRepaymentScheduleService, RepaymentScheduleService>();
builder.Services.AddScoped<ICreditScoringService, CreditScoringService>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

// app.MapControllers();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapClientEndpoints();
app.MapKycEndpoints();
app.MapLoanProductEndpoints();
app.MapLoanApplicationEndpoints();
app.MapLoanEndpoints();
app.MapRepaymentScheduleEndpoints();
app.MapCreditScoringEndpoints();

//perform database migration on startup
app.DbMigrate();

app.Run();

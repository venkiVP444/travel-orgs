using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelOrgOS.Infrastructure.Data;
using TravelOrgOS.Infrastructure.Services;
using TravelOrgOS.Infrastructure.Services.PaymentGateways;

var builder = WebApplication.CreateBuilder(args);

// Load local appsettings file if present
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// 1. Read and Assert Database Connection Safety
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? @"Server=(localdb)\MSSQLLocalDB;Database=TravelOrgOS_Dev;Trusted_Connection=True;TrustServerCertificate=True;";

// MANDATORY SAFETY ASSERTS
DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(connectionString);

// 2. Add DbContext
builder.Services.AddDbContext<TravelOrgOSDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Register Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITravellerService, TravellerService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();

// Payment Gateway Services
builder.Services.AddScoped<IPaymentGatewayService, TravelOrgOS.Infrastructure.Services.PaymentGateways.MockPaymentGatewayService>();
builder.Services.AddScoped<IPaymentGatewayService, TravelOrgOS.Infrastructure.Services.PaymentGateways.StripePaymentGatewayService>();
builder.Services.AddScoped<IPaymentGatewayService, TravelOrgOS.Infrastructure.Services.PaymentGateways.RazorpayPaymentGatewayService>();
builder.Services.AddScoped<TravelOrgOS.Infrastructure.Services.PaymentGateways.IPaymentGatewayFactory, TravelOrgOS.Infrastructure.Services.PaymentGateways.PaymentGatewayFactory>();

// 4. Configure JWT Authentication
const string JwtBearerScheme = JwtBearerDefaults.AuthenticationScheme;

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretTravelOrgOSJwtKey2026WithMaximumSecurityLengthRequiredForHS256!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TravelOrgOS.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TravelOrgOS.Web";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerScheme;
    options.DefaultChallengeScheme = JwtBearerScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// 5. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 6. Automatic Seed Data Initialization
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TravelOrgOSDbContext>();
    await DbInitializer.SeedAsync(context);
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

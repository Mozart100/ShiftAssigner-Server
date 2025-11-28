using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Services.Validation;
using ShiftAssignerServer.Startup;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// JWT configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key") ?? throw new InvalidOperationException("Jwt:Key not configured");
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "ShiftAssignerServer";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "ShiftAssignerClients";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper registration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Authentication
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true
    };
});

// Application services
builder.Services.AddSingleton(new JwtService(jwtKey, jwtIssuer, jwtAudience));
builder.Services.AddSingleton<ITenantRepository,TenantRepository>();
builder.Services.AddSingleton<IShiftLeaderRepository,ShiftLeaderRepository>();
builder.Services.AddSingleton<IWorkerRepository,WorkerRepository>();
builder.Services.AddSingleton<IStuffBookingRepository,StuffBookingRepository>();


builder.Services.AddTransient<ITenantService,TenantService>();
builder.Services.AddTransient<IShiftLeaderService,ShiftLeaderService>();
builder.Services.AddTransient<IWorkerService,WorkerService>();
builder.Services.AddTransient<IStuffBookingService,StuffBookingService>();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register validation services
builder.Services.AddTransient<IRegistrationValidationService, RegistrationValidationService>();


// Configure Serilog for container-friendly logging (console) and optional file sink with retention.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    // Primary sink for containers: console (stdout)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handling - catch unhandled exceptions and return stable JSON payloads
app.UseGlobalErrorHandling();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

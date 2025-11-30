using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Services.Validation;
using ShiftAssignerServer.Startup;

var builder = WebApplication.CreateBuilder(args);

// ---------------- JWT configuration ----------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key") ?? throw new InvalidOperationException("Jwt:Key not configured");
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "ShiftAssignerServer";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "ShiftAssignerClients";

// ---------------- Services: MVC, Swagger ----------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------- Multi-tenancy: HttpContext + TenantProvider ----------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// ---------------- Database configuration ----------------
// If you have TenantModelCacheKeyFactory, this supports schema-per-tenant models.
// Otherwise you can remove the ReplaceService line.
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Optional but recommended for schema-per-tenant:
    // options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
});

// ---------------- AutoMapper ----------------
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// ---------------- Authentication (JWT) ----------------
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

// ---------------- Application services ----------------
builder.Services.AddSingleton(new JwtService(jwtKey, jwtIssuer, jwtAudience));

// Repositories should NOT be singletons when they depend on DbContext.
// Use Scoped so they share the request scope with ApplicationDbContext.
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IShiftLeaderRepository, ShiftLeaderRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IStuffBookingRepository, StuffBookingRepository>();

builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IShiftLeaderService, ShiftLeaderService>();
builder.Services.AddTransient<IWorkerService, WorkerService>();
builder.Services.AddTransient<IStuffBookingService, StuffBookingService>();

// ---------------- FluentValidation ----------------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Validation services
builder.Services.AddTransient<IRegistrationValidationService, RegistrationValidationService>();

// ---------------- Serilog ----------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console() // good for containers
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// ---------------- HTTP pipeline ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handling middleware (your custom extension)
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

using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;    // 👈 ADD THIS
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Extensions;
using ShiftAssignerServer.Middleware;
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

// ---------------- MVC + Swagger ----------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------- Multi-tenancy ----------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// ---------------- Database (schema-per-tenant ready) ----------------
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // 👇 IMPORTANT: enable per-tenant model cache (ApplicationDbContext + StaticTenantProvider)
    options.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
});

// ---------------- Main Database (global/master data) ----------------
builder.Services.AddDbContext<MainSchemaDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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

// ---------------- Application & Repository Layer ----------------

// Jwt Service
builder.Services.AddSingleton(new JwtService(jwtKey, jwtIssuer, jwtAudience));

// EF Repositories (tenant-aware)
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IShiftLeaderRepository, ShiftLeaderRepository>();
builder.Services.AddScoped<IBossTenantRepository, BossTenantRepository>();
builder.Services.AddScoped<IStuffBookingRepository, StuffBookingRepository>();
builder.Services.AddScoped<IMainSchemaRepository, MainSchemaRepository>();
builder.Services.AddScoped<ITenantShiftSchedulingRepository, TenantShiftSchedulingRepository>();
builder.Services.AddScoped<IShiftPeriodSchedulingRepository, ShiftPeriodSchedulingRepository>();

// Unit of Work (recommended)
builder.Services.AddScoped<ITenantUnitOfWork, TenantUnitOfWork>();

// Business Services
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IShiftLeaderService, ShiftLeaderService>();
builder.Services.AddScoped<ITeamHierarchyService, TeamHierarchyService>();
builder.Services.AddScoped<ISchemaManagerService, SchemaManagerService>();
builder.Services.AddScoped<IMainSchemaService, MainSchemaService>();
// builder.Services.AddScoped<IWorkerSchedulerService, WorkerSchedulerService>();

// ---------------- FluentValidation ----------------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<IRegistrationValidationService, RegistrationValidationService>();
builder.Services.AddTransient<IWorkersServiceValidation, WorkersServiceValidation>();

// ---------------- Serilog ----------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// ---------------- Database Migration/Creation ----------------
using (var scope = app.Services.CreateScope())
{
    // Create main/global schema first
    var mainContext = scope.ServiceProvider.GetRequiredService<MainSchemaDbContext>();
    try
    {
        Log.Information("Ensuring main database schema is created...");
        mainContext.EnsureMainSchemaCreated();
        Log.Information("Main database schema created successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error creating main database schema");
        throw;
    }
    
    // Create tenant-specific schema (for development/testing)
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        Log.Information("Ensuring tenant database schema is created...");
        context.Database.EnsureCreated();
        Log.Information("Tenant database schema created successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error creating tenant database schema");
        throw;
    }
}

// ---------------- HTTP pipeline ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception middleware
app.UseGlobalErrorHandling();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add tenant resolution middleware before authentication
app.UseTenantResolution();

// Add auto-save middleware after tenant resolution
app.UseAutoSave();

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

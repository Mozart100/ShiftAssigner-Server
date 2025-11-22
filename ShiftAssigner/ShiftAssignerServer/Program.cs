using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShiftAssignerServer.Repositories;
using ShiftAssignerServer.Services;
using ShiftAssignerServer.Startup;

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
app.Run();

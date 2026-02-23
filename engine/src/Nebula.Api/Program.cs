using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Nebula.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication — Keycloak OIDC JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("authenticated", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
    options.AddFixedWindowLimiter("anonymous", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

// OpenAPI
builder.Services.AddOpenApi();

// Health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

var app = builder.Build();

// Global exception handler — RFC 7807 ProblemDetails
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var statusCode = StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = "An unexpected error occurred",
            Type = "https://docs.nebula.com/errors/internal-error",
            Extensions = { ["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier }
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.ContentType is null or "")
    {
        response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = response.StatusCode,
            Title = response.StatusCode switch
            {
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                429 => "Too Many Requests",
                _ => "Error"
            },
            Extensions = { ["traceId"] = System.Diagnostics.Activity.Current?.Id ?? statusCodeContext.HttpContext.TraceIdentifier }
        };
        await response.WriteAsJsonAsync(problem);
    }
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// OpenAPI/Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Health endpoint
app.MapHealthChecks("/healthz").AllowAnonymous();

app.Run();

// Required for WebApplicationFactory integration tests
public partial class Program { }

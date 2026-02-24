using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using FluentValidation;
using Nebula.Infrastructure;
using Nebula.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Nebula.Application.Common;
using Nebula.Application.Services;
using Nebula.Application.Validators;
using Nebula.Api.Endpoints;
using Nebula.Api.Services;

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
        if (builder.Environment.IsDevelopment())
        {
            options.TokenValidationParameters.ValidateAudience = false;
        }
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

// Infrastructure (repositories, authorization, caching)
builder.Services.AddInfrastructure();

// Application services
builder.Services.AddScoped<BrokerService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<SubmissionService>();
builder.Services.AddScoped<RenewalService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TimelineService>();
builder.Services.AddScoped<ReferenceDataService>();

// Current user
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<BrokerCreateValidator>();

var app = builder.Build();

// Apply pending migrations and seed dev data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    if (app.Environment.IsDevelopment())
        await DevSeedData.SeedDevDataAsync(db);
}

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
            Extensions =
            {
                ["code"] = "internal_error",
                ["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier,
            }
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
        var (title, code) = response.StatusCode switch
        {
            401 => ("Unauthorized", "unauthorized"),
            403 => ("Forbidden", "forbidden"),
            404 => ("Not Found", "not_found"),
            429 => ("Too Many Requests", "rate_limited"),
            _ => ("Error", "error")
        };
        var problem = new ProblemDetails
        {
            Status = response.StatusCode,
            Title = title,
            Extensions =
            {
                ["code"] = code,
                ["traceId"] = System.Diagnostics.Activity.Current?.Id ?? statusCodeContext.HttpContext.TraceIdentifier,
            }
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

// API endpoints
app.MapBrokerEndpoints();
app.MapContactEndpoints();
app.MapReferenceDataEndpoints();
app.MapSubmissionEndpoints();
app.MapRenewalEndpoints();
app.MapDashboardEndpoints();
app.MapTaskEndpoints();
app.MapTimelineEndpoints();

app.Run();

// Required for WebApplicationFactory integration tests
public partial class Program { }

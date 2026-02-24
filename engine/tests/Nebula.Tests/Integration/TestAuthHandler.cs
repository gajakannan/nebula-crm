using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nebula.Tests.Integration;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public static string TestSubject { get; set; } = "test-user-001";
    public static string TestRole { get; set; } = "Admin";
    public static string TestDisplayName { get; set; } = "Test User";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("sub", TestSubject),
            new Claim(ClaimTypes.NameIdentifier, TestSubject),
            new Claim("name", TestDisplayName),
            new Claim(ClaimTypes.Name, TestDisplayName),
            new Claim("role", TestRole),
            new Claim(ClaimTypes.Role, TestRole),
            new Claim("regions", "West"),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

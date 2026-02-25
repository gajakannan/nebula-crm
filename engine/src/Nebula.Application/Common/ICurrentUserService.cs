namespace Nebula.Application.Common;

public interface ICurrentUserService
{
    string Subject { get; }
    string? DisplayName { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Regions { get; }
}

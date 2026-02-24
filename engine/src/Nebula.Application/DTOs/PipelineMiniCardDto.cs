namespace Nebula.Application.DTOs;

public record PipelineMiniCardDto(
    Guid EntityId,
    string EntityName,
    double? Amount,
    int DaysInStatus,
    string? AssignedUserInitials,
    string? AssignedUserDisplayName);

namespace Nebula.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    string AssignedTo,
    string? LinkedEntityType,
    Guid? LinkedEntityId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? CompletedAt);

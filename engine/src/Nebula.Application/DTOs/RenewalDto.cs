namespace Nebula.Application.DTOs;

public record RenewalDto(
    Guid Id,
    Guid AccountId,
    Guid BrokerId,
    Guid? SubmissionId,
    string CurrentStatus,
    DateTime RenewalDate,
    string? AssignedTo,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime UpdatedAt,
    string? UpdatedBy);

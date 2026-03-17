namespace Nebula.Application.DTOs;

public record RenewalDto(
    Guid Id,
    Guid AccountId,
    Guid BrokerId,
    Guid? SubmissionId,
    string? LineOfBusiness,
    string CurrentStatus,
    DateTime RenewalDate,
    Guid? AssignedToUserId,
    DateTime CreatedAt,
    Guid? CreatedByUserId,
    DateTime UpdatedAt,
    Guid? UpdatedByUserId);

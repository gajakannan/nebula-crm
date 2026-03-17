namespace Nebula.Application.DTOs;

public record RenewalUpdateDto(
    Guid? SubmissionId,
    string? LineOfBusiness,
    string? CurrentStatus,
    DateTime? RenewalDate,
    Guid? AssignedToUserId);

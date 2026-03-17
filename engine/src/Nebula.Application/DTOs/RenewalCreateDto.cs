namespace Nebula.Application.DTOs;

public record RenewalCreateDto(
    Guid AccountId,
    Guid BrokerId,
    Guid? SubmissionId,
    string? LineOfBusiness,
    string CurrentStatus,
    DateTime RenewalDate,
    Guid AssignedToUserId);

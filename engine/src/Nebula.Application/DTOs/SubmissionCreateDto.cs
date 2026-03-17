namespace Nebula.Application.DTOs;

public record SubmissionCreateDto(
    Guid AccountId,
    Guid BrokerId,
    Guid? ProgramId,
    string? LineOfBusiness,
    string CurrentStatus,
    DateTime EffectiveDate,
    decimal PremiumEstimate,
    Guid AssignedToUserId);

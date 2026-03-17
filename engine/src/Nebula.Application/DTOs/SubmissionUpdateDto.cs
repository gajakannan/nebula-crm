namespace Nebula.Application.DTOs;

public record SubmissionUpdateDto(
    Guid? ProgramId,
    string? LineOfBusiness,
    string? CurrentStatus,
    DateTime? EffectiveDate,
    decimal? PremiumEstimate,
    Guid? AssignedToUserId);

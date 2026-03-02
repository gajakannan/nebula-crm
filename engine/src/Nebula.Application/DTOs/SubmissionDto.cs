namespace Nebula.Application.DTOs;

public record SubmissionDto(
    Guid Id,
    Guid AccountId,
    Guid BrokerId,
    Guid? ProgramId,
    string CurrentStatus,
    DateTime? EffectiveDate,
    decimal? PremiumEstimate,
    Guid? AssignedToUserId,
    DateTime CreatedAt,
    Guid? CreatedByUserId,
    DateTime UpdatedAt,
    Guid? UpdatedByUserId);

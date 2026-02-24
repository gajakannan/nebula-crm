namespace Nebula.Application.DTOs;

public record SubmissionDto(
    Guid Id,
    Guid AccountId,
    Guid BrokerId,
    Guid? ProgramId,
    string CurrentStatus,
    DateTime? EffectiveDate,
    decimal? PremiumEstimate,
    string? AssignedTo,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime UpdatedAt,
    string? UpdatedBy);

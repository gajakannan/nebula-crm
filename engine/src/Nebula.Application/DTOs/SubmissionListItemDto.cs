namespace Nebula.Application.DTOs;

public record SubmissionListItemDto(
    Guid Id,
    string AccountName,
    string BrokerName,
    string? LineOfBusiness,
    string CurrentStatus,
    DateTime EffectiveDate,
    Guid AssignedToUserId,
    string? AssignedToDisplayName,
    DateTime CreatedAt,
    bool IsStale);

namespace Nebula.Application.DTOs;

public record OpportunityAgingDto(
    string EntityType,
    int PeriodDays,
    IReadOnlyList<OpportunityAgingStatusDto> Statuses);

public record OpportunityAgingStatusDto(
    string Status,
    string Label,
    string ColorGroup,
    short DisplayOrder,
    IReadOnlyList<OpportunityAgingBucketDto> Buckets,
    int Total);

public record OpportunityAgingBucketDto(
    string Key,
    string Label,
    int Count);

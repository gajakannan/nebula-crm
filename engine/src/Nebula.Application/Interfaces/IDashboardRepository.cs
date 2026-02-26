using Nebula.Application.DTOs;

namespace Nebula.Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default);
    Task<DashboardOpportunitiesDto> GetOpportunitiesAsync(CancellationToken ct = default);
    Task<OpportunityFlowDto> GetOpportunityFlowAsync(string entityType, int periodDays, CancellationToken ct = default);
    Task<OpportunityItemsDto> GetOpportunityItemsAsync(string entityType, string status, CancellationToken ct = default);
    Task<IReadOnlyList<NudgeCardDto>> GetNudgesAsync(string userSubject, CancellationToken ct = default);
}

using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;

namespace Nebula.Application.Services;

public class DashboardService(IDashboardRepository dashboardRepo)
{
    public Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default) =>
        dashboardRepo.GetKpisAsync(ct);

    public Task<DashboardOpportunitiesDto> GetOpportunitiesAsync(CancellationToken ct = default) =>
        dashboardRepo.GetOpportunitiesAsync(ct);

    public Task<OpportunityFlowDto> GetOpportunityFlowAsync(string entityType, int periodDays = 180, CancellationToken ct = default) =>
        dashboardRepo.GetOpportunityFlowAsync(entityType, periodDays, ct);

    public Task<OpportunityItemsDto> GetOpportunityItemsAsync(string entityType, string status, CancellationToken ct = default) =>
        dashboardRepo.GetOpportunityItemsAsync(entityType, status, ct);

    public async Task<NudgesResponseDto> GetNudgesAsync(string userSubject, CancellationToken ct = default)
    {
        var nudges = await dashboardRepo.GetNudgesAsync(userSubject, ct);
        return new NudgesResponseDto(nudges);
    }
}

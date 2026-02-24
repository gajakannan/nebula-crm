using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;

namespace Nebula.Application.Services;

public class DashboardService(IDashboardRepository dashboardRepo)
{
    public Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default) =>
        dashboardRepo.GetKpisAsync(ct);

    public Task<DashboardPipelineDto> GetPipelineAsync(CancellationToken ct = default) =>
        dashboardRepo.GetPipelineAsync(ct);

    public Task<PipelineItemsDto> GetPipelineItemsAsync(string entityType, string status, CancellationToken ct = default) =>
        dashboardRepo.GetPipelineItemsAsync(entityType, status, ct);

    public async Task<NudgesResponseDto> GetNudgesAsync(string userSubject, CancellationToken ct = default)
    {
        var nudges = await dashboardRepo.GetNudgesAsync(userSubject, ct);
        return new NudgesResponseDto(nudges);
    }
}

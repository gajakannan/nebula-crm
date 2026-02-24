using Nebula.Application.DTOs;

namespace Nebula.Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default);
    Task<DashboardPipelineDto> GetPipelineAsync(CancellationToken ct = default);
    Task<PipelineItemsDto> GetPipelineItemsAsync(string entityType, string status, CancellationToken ct = default);
    Task<IReadOnlyList<NudgeCardDto>> GetNudgesAsync(string userSubject, CancellationToken ct = default);
}

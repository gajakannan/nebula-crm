namespace Nebula.Application.DTOs;

public record DashboardPipelineDto(
    IReadOnlyList<PipelineStatusCountDto> Submissions,
    IReadOnlyList<PipelineStatusCountDto> Renewals);

public record PipelineStatusCountDto(
    string Status,
    int Count,
    string ColorGroup);

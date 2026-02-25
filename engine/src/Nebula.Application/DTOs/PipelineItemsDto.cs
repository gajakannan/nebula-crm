namespace Nebula.Application.DTOs;

public record PipelineItemsDto(
    IReadOnlyList<PipelineMiniCardDto> Items,
    int TotalCount);

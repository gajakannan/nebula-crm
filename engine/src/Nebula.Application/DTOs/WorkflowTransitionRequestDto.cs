namespace Nebula.Application.DTOs;

public record WorkflowTransitionRequestDto(
    string ToState,
    string? Reason);

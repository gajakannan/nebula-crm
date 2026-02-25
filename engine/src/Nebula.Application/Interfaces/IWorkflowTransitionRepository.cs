using Nebula.Domain.Entities;

namespace Nebula.Application.Interfaces;

public interface IWorkflowTransitionRepository
{
    Task<IReadOnlyList<WorkflowTransition>> ListByEntityAsync(string workflowType, Guid entityId, CancellationToken ct = default);
    Task AddAsync(WorkflowTransition transition, CancellationToken ct = default);
}

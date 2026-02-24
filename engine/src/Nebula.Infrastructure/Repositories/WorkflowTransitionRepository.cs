using Microsoft.EntityFrameworkCore;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Infrastructure.Repositories;

public class WorkflowTransitionRepository(AppDbContext db) : IWorkflowTransitionRepository
{
    public async Task<IReadOnlyList<WorkflowTransition>> ListByEntityAsync(
        string workflowType, Guid entityId, CancellationToken ct = default) =>
        await db.WorkflowTransitions
            .Where(wt => wt.WorkflowType == workflowType && wt.EntityId == entityId)
            .OrderBy(wt => wt.OccurredAt)
            .ToListAsync(ct);

    public async Task AddAsync(WorkflowTransition transition, CancellationToken ct = default)
    {
        db.WorkflowTransitions.Add(transition);
        await db.SaveChangesAsync(ct);
    }
}

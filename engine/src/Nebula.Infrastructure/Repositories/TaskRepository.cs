using Microsoft.EntityFrameworkCore;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Infrastructure.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<TaskItem> Tasks, int TotalCount)> GetMyTasksAsync(
        string assignedTo, int limit, CancellationToken ct = default)
    {
        var query = db.Tasks
            .Where(t => t.AssignedTo == assignedTo && t.Status != "Done");

        var totalCount = await query.CountAsync(ct);

        var tasks = await query
            .OrderBy(t => t.DueDate.HasValue ? 0 : 1)
            .ThenBy(t => t.DueDate)
            .Take(limit)
            .ToListAsync(ct);

        return (tasks, totalCount);
    }
}

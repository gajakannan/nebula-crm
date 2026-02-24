using Nebula.Domain.Entities;

namespace Nebula.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<TaskItem> Tasks, int TotalCount)> GetMyTasksAsync(string assignedTo, int limit, CancellationToken ct = default);
}

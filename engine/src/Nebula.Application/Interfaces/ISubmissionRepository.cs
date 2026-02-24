using Nebula.Domain.Entities;

namespace Nebula.Application.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Submission submission, CancellationToken ct = default);
}

using Nebula.Domain.Entities;

namespace Nebula.Application.Interfaces;

public interface IRenewalRepository
{
    Task<Renewal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Renewal renewal, CancellationToken ct = default);
}

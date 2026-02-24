using Microsoft.EntityFrameworkCore;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Infrastructure.Repositories;

public class RenewalRepository(AppDbContext db) : IRenewalRepository
{
    public async Task<Renewal?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Renewals.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task UpdateAsync(Renewal renewal, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}

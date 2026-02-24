using Microsoft.EntityFrameworkCore;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Infrastructure.Repositories;

public class SubmissionRepository(AppDbContext db) : ISubmissionRepository
{
    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Submissions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task UpdateAsync(Submission submission, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}

using Nebula.Application.Common;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Domain.Entities;

namespace Nebula.Application.Services;

public class BrokerService(
    IBrokerRepository brokerRepo,
    ITimelineRepository timelineRepo)
{
    public async Task<BrokerDto?> GetByIdAsync(Guid id, ICurrentUserService user, CancellationToken ct = default)
    {
        var broker = await brokerRepo.GetByIdAsync(id, ct);
        if (broker is null) return null;
        return MaskPii(MapToDto(broker));
    }

    public async Task<PaginatedResult<BrokerDto>> ListAsync(
        string? search, string? statusFilter, int page, int pageSize, CancellationToken ct = default)
    {
        var result = await brokerRepo.ListAsync(search, statusFilter, page, pageSize, ct);
        var mapped = result.Data.Select(b => MaskPii(MapToDto(b))).ToList();
        return new PaginatedResult<BrokerDto>(mapped, result.Page, result.PageSize, result.TotalCount);
    }

    public async Task<(BrokerDto? Dto, string? ErrorCode)> CreateAsync(
        BrokerCreateDto dto, ICurrentUserService user, CancellationToken ct = default)
    {
        if (await brokerRepo.ExistsByLicenseAsync(dto.LicenseNumber, ct))
            return (null, "duplicate_license");

        var now = DateTime.UtcNow;
        var broker = new Broker
        {
            LegalName = dto.LegalName,
            LicenseNumber = dto.LicenseNumber,
            State = dto.State,
            Status = "Pending",
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = user.Subject,
            UpdatedBy = user.Subject,
        };

        await brokerRepo.AddAsync(broker, ct);

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Broker",
            EntityId = broker.Id,
            EventType = "BrokerCreated",
            EventDescription = $"New broker \"{broker.LegalName}\" added",
            ActorSubject = user.Subject,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        return (MapToDto(broker), null);
    }

    public async Task<(BrokerDto? Dto, string? ErrorCode)> UpdateAsync(
        Guid id, BrokerUpdateDto dto, uint rowVersion, ICurrentUserService user, CancellationToken ct = default)
    {
        var broker = await brokerRepo.GetByIdAsync(id, ct);
        if (broker is null) return (null, "not_found");

        var oldStatus = broker.Status;
        var now = DateTime.UtcNow;

        broker.LegalName = dto.LegalName;
        broker.State = dto.State;
        broker.Status = dto.Status;
        broker.Email = dto.Email;
        broker.Phone = dto.Phone;
        broker.UpdatedAt = now;
        broker.UpdatedBy = user.Subject;
        broker.RowVersion = rowVersion;

        await brokerRepo.UpdateAsync(broker, ct);

        var eventType = oldStatus != dto.Status ? "BrokerStatusChanged" : "BrokerUpdated";
        var description = oldStatus != dto.Status
            ? $"Broker \"{broker.LegalName}\" status changed from {oldStatus} to {dto.Status}"
            : $"Broker \"{broker.LegalName}\" updated";

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Broker",
            EntityId = broker.Id,
            EventType = eventType,
            EventDescription = description,
            ActorSubject = user.Subject,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        return (MaskPii(MapToDto(broker)), null);
    }

    public async Task<string?> DeleteAsync(Guid id, ICurrentUserService user, CancellationToken ct = default)
    {
        var broker = await brokerRepo.GetByIdAsync(id, ct);
        if (broker is null) return "not_found";

        if (await brokerRepo.HasActiveSubmissionsOrRenewalsAsync(id, ct))
            return "active_submissions_exist";

        var now = DateTime.UtcNow;
        broker.IsDeleted = true;
        broker.DeletedAt = now;
        broker.DeletedBy = user.Subject;
        broker.UpdatedAt = now;
        broker.UpdatedBy = user.Subject;

        await brokerRepo.UpdateAsync(broker, ct);

        await timelineRepo.AddEventAsync(new ActivityTimelineEvent
        {
            EntityType = "Broker",
            EntityId = broker.Id,
            EventType = "BrokerDeleted",
            EventDescription = $"Broker \"{broker.LegalName}\" deleted",
            ActorSubject = user.Subject,
            ActorDisplayName = user.DisplayName,
            OccurredAt = now,
        }, ct);

        return null;
    }

    private static BrokerDto MapToDto(Broker b) => new(
        b.Id, b.LegalName, b.LicenseNumber, b.State, b.Status,
        b.Email, b.Phone, b.CreatedAt, b.UpdatedAt, b.RowVersion);

    private static BrokerDto MaskPii(BrokerDto dto) =>
        dto.Status == "Inactive" ? dto with { Email = null, Phone = null } : dto;
}

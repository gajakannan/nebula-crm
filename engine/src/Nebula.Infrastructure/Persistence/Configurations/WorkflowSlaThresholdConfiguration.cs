using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class WorkflowSlaThresholdConfiguration : IEntityTypeConfiguration<WorkflowSlaThreshold>
{
    public void Configure(EntityTypeBuilder<WorkflowSlaThreshold> builder)
    {
        builder.ToTable("WorkflowSlaThresholds", t =>
        {
            t.HasCheckConstraint("CK_WorkflowSlaThresholds_WarningLessThanTarget", "\"WarningDays\" < \"TargetDays\"");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(30);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(30);
        builder.Property(e => e.WarningDays).IsRequired();
        builder.Property(e => e.TargetDays).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        builder.HasIndex(e => new { e.EntityType, e.Status })
            .IsUnique()
            .HasDatabaseName("UX_WorkflowSlaThresholds_EntityType_Status");

        builder.HasData(BuildSeedData());
    }

    private static IReadOnlyList<WorkflowSlaThreshold> BuildSeedData()
    {
        var now = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc);

        return
        [
            // Submission thresholds
            Seed("1fef8cb4-2f9b-41e8-9329-3c1a5d22790a", "submission", "Received", 1, 2, now),
            Seed("f6969f8b-7ab9-4ab0-a6e9-26f95f57b21c", "submission", "Triaging", 1, 2, now),
            Seed("379f3ad6-68f0-4d2f-b52f-5ab9bb40f157", "submission", "WaitingOnBroker", 2, 3, now),

            // Renewal thresholds
            Seed("30efe68f-9e5c-4e7f-9191-e68ee0f8eb26", "renewal", "Created", 1, 3, now),
            Seed("0e5f31e6-af58-4e30-8ea0-f2d6f862994e", "renewal", "Early", 7, 30, now),
            Seed("f0f6f093-7e6e-45f5-ac84-76510ddfe371", "renewal", "DataReview", 2, 5, now),
            Seed("bb695667-05cf-43dd-a89c-c05e4747967c", "renewal", "OutreachStarted", 3, 7, now),
            Seed("2a620479-fc25-4a25-b0c5-1dce00a3693a", "renewal", "WaitingOnBroker", 5, 10, now),
            Seed("77ca3fa9-fddd-47ec-b4d2-84bcbf001687", "renewal", "InReview", 5, 14, now),
            Seed("f501f5dd-23d4-4250-9eab-65a70d0c08f5", "renewal", "Quoted", 7, 21, now),
            Seed("d7fe40cd-c9a5-4fd5-b09c-47f10ff0f20f", "renewal", "Negotiation", 7, 21, now),
            Seed("fdf17afe-4182-46e4-bf8b-3079e74b3579", "renewal", "BindRequested", 2, 5, now),
        ];
    }

    private static WorkflowSlaThreshold Seed(
        string id,
        string entityType,
        string status,
        int warningDays,
        int targetDays,
        DateTime timestampUtc) => new()
        {
            Id = Guid.Parse(id),
            EntityType = entityType,
            Status = status,
            WarningDays = warningDays,
            TargetDays = targetDays,
            CreatedAt = timestampUtc,
            UpdatedAt = timestampUtc,
        };
}

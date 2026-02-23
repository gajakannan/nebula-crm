using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class ReferenceSubmissionStatusConfiguration : IEntityTypeConfiguration<ReferenceSubmissionStatus>
{
    public void Configure(EntityTypeBuilder<ReferenceSubmissionStatus> builder)
    {
        builder.ToTable("ReferenceSubmissionStatuses");

        builder.HasKey(e => e.Code);

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(255);
        builder.Property(e => e.IsTerminal).IsRequired();
        builder.Property(e => e.DisplayOrder).IsRequired();
        builder.Property(e => e.ColorGroup).HasMaxLength(20);

        builder.HasIndex(e => e.DisplayOrder).IsUnique();

        builder.HasData(
            new ReferenceSubmissionStatus { Code = "Received", DisplayName = "Received", Description = "Initial state when submission is created", IsTerminal = false, DisplayOrder = 1, ColorGroup = "intake" },
            new ReferenceSubmissionStatus { Code = "Triaging", DisplayName = "Triaging", Description = "Initial triage and data validation", IsTerminal = false, DisplayOrder = 2, ColorGroup = "triage" },
            new ReferenceSubmissionStatus { Code = "WaitingOnBroker", DisplayName = "Waiting on Broker", Description = "Awaiting additional information from broker", IsTerminal = false, DisplayOrder = 3, ColorGroup = "waiting" },
            new ReferenceSubmissionStatus { Code = "ReadyForUWReview", DisplayName = "Ready for UW Review", Description = "All data received, queued for underwriter", IsTerminal = false, DisplayOrder = 4, ColorGroup = "review" },
            new ReferenceSubmissionStatus { Code = "InReview", DisplayName = "In Review", Description = "Under active underwriter review", IsTerminal = false, DisplayOrder = 5, ColorGroup = "review" },
            new ReferenceSubmissionStatus { Code = "Quoted", DisplayName = "Quoted", Description = "Quote issued, awaiting broker response", IsTerminal = false, DisplayOrder = 6, ColorGroup = "decision" },
            new ReferenceSubmissionStatus { Code = "BindRequested", DisplayName = "Bind Requested", Description = "Broker accepted quote, bind in progress", IsTerminal = false, DisplayOrder = 7, ColorGroup = "decision" },
            new ReferenceSubmissionStatus { Code = "Bound", DisplayName = "Bound", Description = "Policy bound and issued", IsTerminal = true, DisplayOrder = 8, ColorGroup = null },
            new ReferenceSubmissionStatus { Code = "Declined", DisplayName = "Declined", Description = "Submission declined by underwriter", IsTerminal = true, DisplayOrder = 9, ColorGroup = null },
            new ReferenceSubmissionStatus { Code = "Withdrawn", DisplayName = "Withdrawn", Description = "Broker withdrew submission", IsTerminal = true, DisplayOrder = 10, ColorGroup = null }
        );
    }
}

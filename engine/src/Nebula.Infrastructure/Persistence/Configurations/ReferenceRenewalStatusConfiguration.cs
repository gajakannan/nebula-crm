using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class ReferenceRenewalStatusConfiguration : IEntityTypeConfiguration<ReferenceRenewalStatus>
{
    public void Configure(EntityTypeBuilder<ReferenceRenewalStatus> builder)
    {
        builder.ToTable("ReferenceRenewalStatuses");

        builder.HasKey(e => e.Code);

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(255);
        builder.Property(e => e.IsTerminal).IsRequired();
        builder.Property(e => e.DisplayOrder).IsRequired();
        builder.Property(e => e.ColorGroup).HasMaxLength(20);

        builder.HasIndex(e => e.DisplayOrder).IsUnique();

        builder.HasData(
            new ReferenceRenewalStatus { Code = "Created", DisplayName = "Created", Description = "Renewal record created from expiring policy", IsTerminal = false, DisplayOrder = 1, ColorGroup = "intake" },
            new ReferenceRenewalStatus { Code = "Early", DisplayName = "Early", Description = "In early renewal window (90-120 days out)", IsTerminal = false, DisplayOrder = 2, ColorGroup = "intake" },
            new ReferenceRenewalStatus { Code = "OutreachStarted", DisplayName = "Outreach Started", Description = "Active broker/account outreach begun", IsTerminal = false, DisplayOrder = 3, ColorGroup = "waiting" },
            new ReferenceRenewalStatus { Code = "InReview", DisplayName = "In Review", Description = "Under underwriter review for renewal terms", IsTerminal = false, DisplayOrder = 4, ColorGroup = "review" },
            new ReferenceRenewalStatus { Code = "Quoted", DisplayName = "Quoted", Description = "Renewal quote issued", IsTerminal = false, DisplayOrder = 5, ColorGroup = "decision" },
            new ReferenceRenewalStatus { Code = "Bound", DisplayName = "Bound", Description = "Policy renewed and bound", IsTerminal = true, DisplayOrder = 6, ColorGroup = null },
            new ReferenceRenewalStatus { Code = "Lost", DisplayName = "Lost", Description = "Lost to competitor", IsTerminal = true, DisplayOrder = 7, ColorGroup = null },
            new ReferenceRenewalStatus { Code = "Lapsed", DisplayName = "Lapsed", Description = "Policy expired without renewal", IsTerminal = true, DisplayOrder = 8, ColorGroup = null }
        );
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CurrentStatus).IsRequired().HasMaxLength(30).HasDefaultValue("Received");
        builder.Property(e => e.EffectiveDate).IsRequired();
        builder.Property(e => e.PremiumEstimate).IsRequired().HasPrecision(18, 2);
        builder.Property(e => e.AssignedTo).IsRequired().HasMaxLength(255);
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.DeletedBy).HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Broker)
            .WithMany()
            .HasForeignKey(e => e.BrokerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Program)
            .WithMany()
            .HasForeignKey(e => e.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.CurrentStatus)
            .HasDatabaseName("IX_Submissions_CurrentStatus")
            .HasFilter("\"CurrentStatus\" NOT IN ('Bound', 'Declined', 'Withdrawn')");

        builder.HasIndex(e => new { e.AssignedTo, e.CurrentStatus })
            .HasDatabaseName("IX_Submissions_AssignedTo_CurrentStatus");
    }
}

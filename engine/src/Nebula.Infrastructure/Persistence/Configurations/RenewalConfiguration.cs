using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class RenewalConfiguration : IEntityTypeConfiguration<Renewal>
{
    public void Configure(EntityTypeBuilder<Renewal> builder)
    {
        builder.ToTable("Renewals");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CurrentStatus).IsRequired().HasMaxLength(30).HasDefaultValue("Created");
        builder.Property(e => e.RenewalDate).IsRequired();
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

        builder.HasOne(e => e.Submission)
            .WithMany()
            .HasForeignKey(e => e.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.CurrentStatus)
            .HasDatabaseName("IX_Renewals_CurrentStatus");

        builder.HasIndex(e => new { e.AssignedTo, e.CurrentStatus })
            .HasDatabaseName("IX_Renewals_AssignedTo_CurrentStatus");

        builder.HasIndex(e => new { e.RenewalDate, e.CurrentStatus })
            .HasDatabaseName("IX_Renewals_RenewalDate_Status");
    }
}

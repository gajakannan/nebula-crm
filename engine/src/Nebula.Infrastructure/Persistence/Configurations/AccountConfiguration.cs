using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Industry).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PrimaryState).IsRequired().HasMaxLength(2);
        builder.Property(e => e.Region).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(20);
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.DeletedBy).HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        builder.Property(e => e.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.Region)
            .HasDatabaseName("IX_Accounts_Region");
    }
}

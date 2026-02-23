using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence.Configurations;

public class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.ToTable("Programs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ProgramCode).IsRequired().HasMaxLength(50);
        builder.Property(e => e.ManagedBySubject).HasMaxLength(255);
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(255);
        builder.Property(e => e.DeletedBy).HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        builder.HasOne(e => e.Mga)
            .WithMany()
            .HasForeignKey(e => e.MgaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.ManagedBySubject)
            .HasDatabaseName("IX_Programs_ManagedBySubject");
    }
}

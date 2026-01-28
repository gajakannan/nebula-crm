# Data Modeling Guide

Focused guide for designing database schemas with EF Core.

## Entity Design Checklist

### Required Fields
- [x] Primary key (Guid Id)
- [x] Audit fields (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- [x] Soft delete (DeletedAt nullable)

### Relationships
- Define navigation properties
- Specify cascade behavior
- Index foreign keys

### Validation
- NOT NULL constraints
- UNIQUE constraints where needed
- CHECK constraints for business rules
- String length limits

## EF Core Patterns

### Entity Configuration
```csharp
public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.ToTable("Brokers");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.HasIndex(b => b.Name);
        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
```

### Migrations
```bash
# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

See `agents/templates/entity-model-template.md` for complete entity specification format.

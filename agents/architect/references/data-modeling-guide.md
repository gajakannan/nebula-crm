# Data Modeling Guide

Comprehensive guide for designing database schemas with Entity Framework Core 10, PostgreSQL, and Clean Architecture principles for the Nebula insurance CRM.

---

## 1. Entity Design Patterns

### 1.1 Base Entity Classes

All domain entities should inherit from a common base class to ensure consistency.

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Audit fields (who and when)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid UpdatedBy { get; set; }

    // Soft delete
    public DateTime? DeletedAt { get; set; }

    // Helper methods
    public bool IsDeleted => DeletedAt.HasValue;
    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    public void Restore() => DeletedAt = null;
}
```

**Benefits:**
- Consistent audit trail across all entities
- Automatic soft delete support
- Reduces boilerplate in entity classes
- Easy to add global conventions (e.g., RowVersion for concurrency)

**Convention:**
- All entities in Domain layer inherit from `BaseEntity`
- Infrastructure layer (EF Core) automatically sets audit fields via interceptors
- Global query filter applies `WHERE DeletedAt IS NULL` to all queries

---

### 1.2 Soft Delete Pattern

Soft delete marks records as deleted without removing them from the database, preserving audit trail and allowing restoration.

**Implementation:**

```csharp
// Entity with soft delete
public class Broker : BaseEntity
{
    public string Name { get; set; }
    public string LicenseNumber { get; set; }
    // ... other fields
}

// EF Core configuration
public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        // Global query filter - automatically excludes soft-deleted records
        builder.HasQueryFilter(b => b.DeletedAt == null);

        // Partial index for active records only (PostgreSQL)
        builder.HasIndex(b => b.DeletedAt)
            .HasFilter("DeletedAt IS NULL")
            .HasDatabaseName("IX_Brokers_ActiveOnly");
    }
}

// Repository pattern
public class BrokerRepository
{
    private readonly ApplicationDbContext _context;

    // Get active brokers (soft-deleted automatically excluded)
    public async Task<List<Broker>> GetAllAsync()
    {
        return await _context.Brokers.ToListAsync();
    }

    // Include soft-deleted brokers (bypass global filter)
    public async Task<List<Broker>> GetAllIncludingDeletedAsync()
    {
        return await _context.Brokers
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    // Soft delete
    public async Task DeleteAsync(Guid id)
    {
        var broker = await _context.Brokers.FindAsync(id);
        broker.SoftDelete();
        await _context.SaveChangesAsync();
    }

    // Restore soft-deleted
    public async Task RestoreAsync(Guid id)
    {
        var broker = await _context.Brokers
            .IgnoreQueryFilters()
            .FirstAsync(b => b.Id == id);
        broker.Restore();
        await _context.SaveChangesAsync();
    }
}
```

**Considerations:**
- Soft-deleted records still count against database size
- Unique constraints must account for soft-deleted records (e.g., license number can be reused after soft delete)
- Consider hard delete after retention period (e.g., 7 years)

---

### 1.3 Audit Fields (Who and When)

Every mutation should track who made the change and when.

**Automatic Audit via SaveChanges Interceptor:**

```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUser.UserId;
        var timestamp = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = timestamp;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedAt = timestamp;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = timestamp;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
```

**Benefits:**
- No manual setting of audit fields in business logic
- Consistent across all entities
- Impossible to forget

---

### 1.4 Value Objects (Owned Entities)

Value objects are immutable objects without identity, defined by their properties.

**Example: Address Value Object:**

```csharp
public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }

    private Address() { } // EF Core constructor

    public Address(string street, string city, string state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    // Value equality
    public override bool Equals(object? obj)
    {
        if (obj is not Address other) return false;
        return Street == other.Street && City == other.City &&
               State == other.State && ZipCode == other.ZipCode;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Street, City, State, ZipCode);
}

// Entity using value object
public class Broker : BaseEntity
{
    public string Name { get; set; }
    public Address PhysicalAddress { get; set; } // Owned entity
    public Address MailingAddress { get; set; }
}

// EF Core configuration
public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.OwnsOne(b => b.PhysicalAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("PhysicalStreet");
            address.Property(a => a.City).HasColumnName("PhysicalCity");
            address.Property(a => a.State).HasColumnName("PhysicalState");
            address.Property(a => a.ZipCode).HasColumnName("PhysicalZipCode");
        });

        builder.OwnsOne(b => b.MailingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("MailingStreet");
            address.Property(a => a.City).HasColumnName("MailingCity");
            address.Property(a => a.State).HasColumnName("MailingState");
            address.Property(a => a.ZipCode).HasColumnName("MailingZipCode");
        });
    }
}
```

**Result:** All address fields stored in Brokers table (not separate table), value object enforces immutability and consistency.

---

### 1.5 Enumerations (String vs Int)

**String Enums (Recommended for Insurance CRM):**

```csharp
public class SubmissionStatus
{
    public const string Received = nameof(Received);
    public const string Triaging = nameof(Triaging);
    public const string ReadyForUWReview = nameof(ReadyForUWReview);
    public const string InReview = nameof(InReview);
    public const string Quoted = nameof(Quoted);
    public const string Bound = nameof(Bound);
    public const string Declined = nameof(Declined);
    public const string Withdrawn = nameof(Withdrawn);
}

public class Submission : BaseEntity
{
    public string Status { get; set; } = SubmissionStatus.Received;
}

// EF Core configuration
builder.Property(s => s.Status)
    .IsRequired()
    .HasMaxLength(50);
```

**Int Enums with Value Converter:**

```csharp
public enum SubmissionStatusEnum
{
    Received = 1,
    Triaging = 2,
    ReadyForUWReview = 3,
    InReview = 4,
    Quoted = 5,
    Bound = 6,
    Declined = 7,
    Withdrawn = 8
}

// EF Core configuration - store as string in database
builder.Property(s => s.Status)
    .HasConversion<string>()
    .IsRequired()
    .HasMaxLength(50);
```

**Recommendation:** Use string enums for insurance CRM (self-documenting in database, easier to query, no magic numbers).

---

## 2. Relationship Patterns

### 2.1 One-to-Many Relationships

**Example: Broker → Contacts**

```csharp
public class Broker : BaseEntity
{
    public string Name { get; set; }

    // Navigation property (one broker has many contacts)
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

public class Contact : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }

    // Foreign key
    public Guid BrokerId { get; set; }

    // Navigation property (many contacts belong to one broker)
    public Broker Broker { get; set; }
}

// EF Core configuration
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasOne(c => c.Broker)
            .WithMany(b => b.Contacts)
            .HasForeignKey(c => c.BrokerId)
            .OnDelete(DeleteBehavior.Cascade); // When broker deleted, delete contacts
    }
}
```

**Cascade Behaviors:**
- `Cascade`: Delete children when parent deleted (use for owned relationships)
- `Restrict`: Prevent parent delete if children exist (use for critical relationships like Broker → Submissions)
- `SetNull`: Set FK to null when parent deleted (use for optional relationships)

---

### 2.2 Many-to-Many Relationships

**Modern Approach (EF Core 5+):** Use explicit join entity for metadata.

**Example: Broker → Programs (many brokers can sell many programs)**

```csharp
public class Broker : BaseEntity
{
    public string Name { get; set; }
    public ICollection<BrokerProgram> BrokerPrograms { get; set; } = new List<BrokerProgram>();
}

public class Program : BaseEntity
{
    public string Name { get; set; }
    public ICollection<BrokerProgram> BrokerPrograms { get; set; } = new List<BrokerProgram>();
}

// Explicit join entity with metadata
public class BrokerProgram : BaseEntity
{
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; }

    public Guid ProgramId { get; set; }
    public Program Program { get; set; }

    // Additional metadata
    public decimal CommissionRate { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsActive { get; set; }
}

// EF Core configuration
public class BrokerProgramConfiguration : IEntityTypeConfiguration<BrokerProgram>
{
    public void Configure(EntityTypeBuilder<BrokerProgram> builder)
    {
        builder.HasKey(bp => new { bp.BrokerId, bp.ProgramId }); // Composite key

        builder.HasOne(bp => bp.Broker)
            .WithMany(b => b.BrokerPrograms)
            .HasForeignKey(bp => bp.BrokerId);

        builder.HasOne(bp => bp.Program)
            .WithMany(p => p.BrokerPrograms)
            .HasForeignKey(bp => bp.ProgramId);
    }
}
```

---

### 2.3 Self-Referencing Relationships

**Example: Broker Hierarchy (MGA → Sub-Broker)**

```csharp
public class Broker : BaseEntity
{
    public string Name { get; set; }

    // Self-referencing foreign key
    public Guid? ParentBrokerId { get; set; }

    // Navigation properties
    public Broker? ParentBroker { get; set; }
    public ICollection<Broker> SubBrokers { get; set; } = new List<Broker>();
}

// EF Core configuration
public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.HasOne(b => b.ParentBroker)
            .WithMany(b => b.SubBrokers)
            .HasForeignKey(b => b.ParentBrokerId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of entire tree
    }
}
```

---

### 2.4 Optional vs Required Relationships

**Required Relationship (FK cannot be null):**

```csharp
public class Submission : BaseEntity
{
    // Required: Every submission must have a broker
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; }
}

// EF Core knows it's required (non-nullable Guid)
```

**Optional Relationship (FK can be null):**

```csharp
public class Submission : BaseEntity
{
    // Optional: Submission may not have assigned underwriter yet
    public Guid? AssignedUnderwriterId { get; set; }
    public User? AssignedUnderwriter { get; set; }
}

// EF Core knows it's optional (nullable Guid?)
```

---

### 2.5 Navigation Properties (One-Way vs Two-Way)

**Two-Way Navigation (Recommended for most cases):**

```csharp
public class Broker : BaseEntity
{
    public ICollection<Submission> Submissions { get; set; }
}

public class Submission : BaseEntity
{
    public Guid BrokerId { get; set; }
    public Broker Broker { get; set; } // Allows submission.Broker.Name
}
```

**One-Way Navigation (When reverse navigation not needed):**

```csharp
public class ActivityTimelineEvent : BaseEntity
{
    public Guid UserId { get; set; }
    // No navigation property - don't need event.User typically
}
```

---

## 3. Migration Strategies

### 3.1 Code-First Migrations

**Create Migration:**

```bash
# Navigate to Infrastructure project
cd src/Nebula.Infrastructure

# Add migration
dotnet ef migrations add AddBrokerEntity --startup-project ../Nebula.API

# Review generated migration in Migrations/ folder
# Verify Up() and Down() methods

# Apply migration to database
dotnet ef database update --startup-project ../Nebula.API
```

**Generated Migration Example:**

```csharp
public partial class AddBrokerEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Brokers",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 255, nullable: false),
                LicenseNumber = table.Column<string>(maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false),
                // ... other columns
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Brokers", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Brokers_LicenseNumber",
            table: "Brokers",
            column: "LicenseNumber",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("Brokers");
    }
}
```

---

### 3.2 Seed Data Patterns

**Deterministic Seeding (Production Reference Data):**

```csharp
public class SeedDataConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        // Seed all 50 US states with deterministic GUIDs
        builder.HasData(
            new State { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Code = "AL", Name = "Alabama" },
            new State { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Code = "AK", Name = "Alaska" },
            // ... all 50 states
        );
    }
}
```

**Idempotent Seeding (Development/Test Data):**

```csharp
public static class DatabaseSeeder
{
    public static async Task SeedDevelopmentDataAsync(ApplicationDbContext context)
    {
        // Seed only if no data exists
        if (await context.Brokers.AnyAsync())
            return;

        var brokers = new[]
        {
            new Broker { Id = Guid.NewGuid(), Name = "Acme Insurance", LicenseNumber = "CA-12345" },
            new Broker { Id = Guid.NewGuid(), Name = "Best Brokers", LicenseNumber = "NY-67890" }
        };

        await context.Brokers.AddRangeAsync(brokers);
        await context.SaveChangesAsync();
    }
}
```

---

### 3.3 Data Migration vs Schema Migration

**Schema Migration:** Changes table structure (add column, drop table, rename field).

**Data Migration:** Populates or transforms existing data.

**Example: Adding Status Field with Default Value**

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Add column (nullable initially)
    migrationBuilder.AddColumn<string>(
        name: "Status",
        table: "Brokers",
        maxLength: 20,
        nullable: true);

    // 2. Set default value for existing rows (data migration)
    migrationBuilder.Sql(
        "UPDATE Brokers SET Status = 'Active' WHERE Status IS NULL");

    // 3. Make column required
    migrationBuilder.AlterColumn<string>(
        name: "Status",
        table: "Brokers",
        maxLength: 20,
        nullable: false);
}
```

---

### 3.4 Handling Breaking Changes

**Renaming Column (with backward compatibility):**

```csharp
// Step 1: Add new column
migrationBuilder.AddColumn<string>(
    name: "BrokerName",
    table: "Brokers");

// Step 2: Copy data from old column
migrationBuilder.Sql(
    "UPDATE Brokers SET BrokerName = Name");

// Step 3: Drop old column (in next migration, after app updated)
migrationBuilder.DropColumn(
    name: "Name",
    table: "Brokers");
```

---

### 3.5 Production Migration Checklist

- [ ] Test migration on development database
- [ ] Review generated SQL (`dotnet ef migrations script`)
- [ ] Backup production database before migration
- [ ] Run migration during maintenance window (if schema changes are breaking)
- [ ] Verify data integrity after migration
- [ ] Have rollback plan (migration's Down() method tested)

---

## 4. Query Optimization

### 4.1 Eager Loading (.Include, .ThenInclude)

**Problem: N+1 Queries**

```csharp
// BAD: N+1 queries (1 for brokers + N for each broker's contacts)
var brokers = await context.Brokers.ToListAsync(); // 1 query
foreach (var broker in brokers)
{
    var contacts = await context.Contacts.Where(c => c.BrokerId == broker.Id).ToListAsync(); // N queries
}
```

**Solution: Eager Loading**

```csharp
// GOOD: Single query with JOIN
var brokers = await context.Brokers
    .Include(b => b.Contacts)
    .ToListAsync();

// Multiple levels
var submissions = await context.Submissions
    .Include(s => s.Broker)
        .ThenInclude(b => b.ParentBroker)
    .Include(s => s.Account)
        .ThenInclude(a => a.Policies)
    .ToListAsync();
```

---

### 4.2 Projection (Select to DTOs)

**Problem: Loading Entire Entities**

```csharp
// BAD: Loads all 20 fields of Broker entity
var brokers = await context.Brokers.ToListAsync();
return brokers.Select(b => new BrokerListItem { Id = b.Id, Name = b.Name });
```

**Solution: Project to DTO**

```csharp
// GOOD: SQL only selects Id, Name columns
var brokers = await context.Brokers
    .Select(b => new BrokerListItem
    {
        Id = b.Id,
        Name = b.Name,
        LicenseNumber = b.LicenseNumber
    })
    .ToListAsync();
```

---

### 4.3 Pagination (Skip/Take)

**Offset Pagination:**

```csharp
public async Task<PagedResult<Broker>> GetBrokersAsync(int page, int pageSize)
{
    var query = context.Brokers.AsQueryable();

    var totalCount = await query.CountAsync();

    var brokers = await query
        .OrderBy(b => b.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Broker>
    {
        Data = brokers,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
```

**Keyset Pagination (for large datasets):**

```csharp
// More efficient than OFFSET for large datasets
var brokers = await context.Brokers
    .Where(b => b.Name.CompareTo(lastSeenName) > 0)
    .OrderBy(b => b.Name)
    .Take(pageSize)
    .ToListAsync();
```

---

### 4.4 Filtering Best Practices

**Use Indexed Columns in WHERE:**

```csharp
// GOOD: Status is indexed
var activeBrokers = await context.Brokers
    .Where(b => b.Status == "Active")
    .ToListAsync();

// AVOID: String operations prevent index usage
var brokers = await context.Brokers
    .Where(b => b.Name.ToLower().Contains("acme")) // Can't use index
    .ToListAsync();

// BETTER: Use case-insensitive collation in database
var brokers = await context.Brokers
    .Where(b => EF.Functions.ILike(b.Name, "%acme%")) // Uses index with PostgreSQL ILIKE
    .ToListAsync();
```

---

### 4.5 Avoiding N+1 Queries (Batching)

**Use .Include() or Explicit Loading:**

```csharp
// Explicit loading (load related data separately)
var broker = await context.Brokers.FindAsync(brokerId);
await context.Entry(broker)
    .Collection(b => b.Contacts)
    .LoadAsync();
```

---

## 5. PostgreSQL-Specific Patterns

### 5.1 JSONB Columns for Flexible Data

**Use Case:** User preferences, audit event payloads, flexible metadata.

```csharp
public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }

    // Store preferences as JSONB
    public Dictionary<string, object> Preferences { get; set; } = new();
}

// EF Core configuration
builder.Property(u => u.Preferences)
    .HasColumnType("jsonb");

// Query JSONB
var users = await context.UserProfiles
    .Where(u => EF.Functions.JsonContains(u.Preferences, @"{""theme"": ""dark""}"))
    .ToListAsync();
```

---

### 5.2 Full-Text Search

**Use tsvector and GIN Index:**

```csharp
// Migration: Add full-text search column
migrationBuilder.Sql(@"
    ALTER TABLE Brokers ADD COLUMN SearchVector tsvector
    GENERATED ALWAYS AS (to_tsvector('english', coalesce(Name, '') || ' ' || coalesce(LicenseNumber, ''))) STORED;

    CREATE INDEX IX_Brokers_SearchVector ON Brokers USING GIN(SearchVector);
");

// Query
var brokers = await context.Brokers
    .Where(b => EF.Functions.ToTsVector("english", b.Name + " " + b.LicenseNumber)
        .Matches(EF.Functions.ToTsQuery("english", searchTerm)))
    .ToListAsync();
```

---

### 5.3 Array Columns

```csharp
public class Submission : BaseEntity
{
    public string[] Tags { get; set; } // Stored as PostgreSQL array
}

// EF Core configuration
builder.Property(s => s.Tags)
    .HasColumnType("text[]");

// Query
var submissions = await context.Submissions
    .Where(s => s.Tags.Contains("urgent"))
    .ToListAsync();
```

---

### 5.4 GUID vs Serial Primary Keys

**Recommendation:** Use GUIDs for distributed systems, ease of merging data, security (non-sequential).

```csharp
// UUID v7 (time-ordered, better index performance than random UUIDs)
public Guid Id { get; set; } = Guid.CreateVersion7();
```

---

### 5.5 Index Types

- **B-tree** (default): General purpose, good for equality and range queries
- **GIN** (Generalized Inverted Index): Full-text search, JSONB, arrays
- **GiST** (Generalized Search Tree): Spatial data, full-text search
- **Hash**: Equality only (rarely needed)

---

## 6. Audit Trail Implementation

### 6.1 ActivityTimelineEvent Table

**Append-only audit log for all entity mutations.**

```csharp
public class ActivityTimelineEvent : BaseEntity
{
    public string EntityType { get; set; } // "Broker", "Submission", etc.
    public Guid EntityId { get; set; } // No FK constraint (entity may be deleted)
    public string EventType { get; set; } // "BrokerCreated", "BrokerUpdated", etc.
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Payload { get; set; } // JSONB for flexibility
}

// EF Core configuration
public class ActivityTimelineEventConfiguration : IEntityTypeConfiguration<ActivityTimelineEvent>
{
    public void Configure(EntityTypeBuilder<ActivityTimelineEvent> builder)
    {
        builder.ToTable("ActivityTimelineEvents");

        builder.Property(e => e.Payload)
            .HasColumnType("jsonb");

        // Indexes for common queries
        builder.HasIndex(e => new { e.EntityType, e.EntityId });
        builder.HasIndex(e => e.Timestamp);

        // Prevent updates/deletes (append-only)
        // Note: This is convention-based; enforce in application layer
    }
}
```

---

### 6.2 WorkflowTransition Table

**Immutable log of all workflow state changes.**

```csharp
public class WorkflowTransition : BaseEntity
{
    public Guid SubmissionId { get; set; } // Or other entity with workflow
    public string FromStatus { get; set; }
    public string ToStatus { get; set; }
    public DateTime TransitionedAt { get; set; }
    public Guid TransitionedBy { get; set; }
    public string? Reason { get; set; } // Required for Declined/Withdrawn
}

// EF Core configuration
public class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransition>
{
    public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
    {
        builder.ToTable("WorkflowTransitions");

        builder.HasIndex(w => w.SubmissionId);
        builder.HasIndex(w => w.TransitionedAt);
    }
}
```

---

### 6.3 Event Sourcing for Audit

**Rebuild entity state from event stream (advanced pattern).**

```csharp
// Not recommended for MVP, but consider for Phase 2 if needed
public async Task<Broker> RebuildBrokerFromEventsAsync(Guid brokerId)
{
    var events = await context.ActivityTimelineEvents
        .Where(e => e.EntityType == "Broker" && e.EntityId == brokerId)
        .OrderBy(e => e.Timestamp)
        .ToListAsync();

    var broker = new Broker();
    foreach (var evt in events)
    {
        switch (evt.EventType)
        {
            case "BrokerCreated":
                broker.Id = evt.EntityId;
                broker.Name = evt.Payload["name"]?.ToString();
                // ... apply event
                break;
            case "BrokerUpdated":
                // ... apply update
                break;
        }
    }

    return broker;
}
```

---

## Version History

**Version 2.0** - 2026-01-31 - Comprehensive data modeling guide with EF Core 10 and PostgreSQL patterns (350 lines)
**Version 1.0** - 2026-01-26 - Initial data modeling guide (54 lines)

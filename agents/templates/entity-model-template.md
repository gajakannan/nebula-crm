---
template: entity-model
version: 1.0
applies_to: architect
---

# Entity Model Template

Use this template to specify database entities for EF Core implementation.

## Entity: [EntityName]

**Table Name:** `[TableName]` (plural, PascalCase)

**Description:** [Brief description of what this entity represents]

**Module:** [Module name this entity belongs to]

---

## Fields

| Field | Type | Constraints | Default | Description |
|-------|------|-------------|---------|-------------|
| Id | Guid | PK, NOT NULL | NewGuid() | Unique identifier |
| [FieldName] | [Type] | [Constraints] | [Default] | [Description] |

### Field Types Reference

**Common Types:**
- `Guid` - Primary keys, foreign keys
- `string(N)` - Varchar with max length (e.g., string(255))
- `string(MAX)` - Text field (unlimited)
- `int` - Integer
- `decimal(P,S)` - Decimal with precision and scale (e.g., decimal(18,2))
- `bool` - Boolean
- `DateTime` - Date and time (stored as UTC)
- `DateOnly` - Date without time
- `byte[]` - Binary data

**Common Constraints:**
- `PK` - Primary key
- `FK → Table` - Foreign key to another table
- `NOT NULL` - Cannot be null
- `NULL` - Nullable (default)
- `UNIQUE` - Unique constraint
- `INDEX` - Indexed for performance
- `CHECK` - Check constraint (specify condition)
- `DEFAULT` - Default value

### Example Fields

```markdown
| Field | Type | Constraints | Default | Description |
|-------|------|-------------|---------|-------------|
| Id | Guid | PK, NOT NULL | NewGuid() | Unique identifier |
| Name | string(255) | NOT NULL, INDEX | - | Entity name |
| Description | string(MAX) | NULL | - | Optional description |
| Status | string(20) | NOT NULL | 'Active' | Active, Inactive, Suspended |
| Amount | decimal(18,2) | NOT NULL, CHECK > 0 | - | Monetary amount |
| IsActive | bool | NOT NULL | true | Active flag |
| ExternalId | string(100) | NULL, UNIQUE | - | External system ID |
| ParentId | Guid | NULL, FK → Entities | - | Self-referencing hierarchy |
| CreatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp of creation |
| CreatedBy | Guid | NOT NULL, FK → Users | - | User who created |
| UpdatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp of last update |
| UpdatedBy | Guid | NOT NULL, FK → Users | - | User who last updated |
| DeletedAt | DateTime | NULL | - | Soft delete timestamp |
```

---

## Relationships

Define relationships with other entities using EF Core navigation properties.

### One-to-Many Relationships

**Format:**
```markdown
- **One-to-Many:** [ParentEntity] → [ChildEntity] ([description])
  - Navigation: [ParentEntity].[ChildCollectionProperty] (ICollection<[ChildEntity]>)
  - Foreign Key: [ChildEntity].[ParentEntityId]
  - Cascade: [Cascade | Restrict | SetNull]
  - Description: [What this relationship represents]
```

**Example:**
```markdown
- **One-to-Many:** Broker → Contacts (one broker has many contacts)
  - Navigation: Broker.Contacts (ICollection<Contact>)
  - Foreign Key: Contact.BrokerId
  - Cascade: Cascade (delete broker → delete contacts)
  - Description: Each broker can have multiple contact persons
```

### Many-to-One Relationships

**Format:**
```markdown
- **Many-to-One:** [ChildEntity] → [ParentEntity] ([description])
  - Navigation: [ChildEntity].[ParentProperty] ([ParentEntity])
  - Foreign Key: [ChildEntity].[ParentEntityId]
  - Required: [Yes | No]
```

**Example:**
```markdown
- **Many-to-One:** Submission → Broker (many submissions from one broker)
  - Navigation: Submission.Broker (Broker)
  - Foreign Key: Submission.BrokerId
  - Required: Yes
```

### Many-to-Many Relationships

**Format:**
```markdown
- **Many-to-Many:** [Entity1] ↔ [Entity2] ([description])
  - Join Table: [JoinTableName]
  - Navigation: [Entity1].[Entity2CollectionProperty], [Entity2].[Entity1CollectionProperty]
  - Description: [What this relationship represents]
```

**Example:**
```markdown
- **Many-to-Many:** Submission ↔ Documents (submissions can have multiple documents, documents can belong to multiple submissions)
  - Join Table: SubmissionDocuments
  - Navigation: Submission.Documents, Document.Submissions
  - Description: Link documents to submissions for versioning
```

### Self-Referencing Relationships

**Format:**
```markdown
- **Self-Referencing:** [Entity] → [Entity] ([description])
  - Navigation: [Entity].[ChildrenProperty], [Entity].[ParentProperty]
  - Foreign Key: [Entity].ParentId
  - Description: [Hierarchy structure]
```

**Example:**
```markdown
- **Self-Referencing:** Broker → Broker (broker hierarchy)
  - Navigation: Broker.SubBrokers (ICollection<Broker>), Broker.ParentBroker (Broker)
  - Foreign Key: Broker.ParentBrokerId
  - Description: Sub-brokers report to parent brokers
```

---

## Indexes

Define indexes for query performance optimization.

**Format:**
```markdown
- `IX_[TableName]_[Column(s)]` ([Type]) - [Description]
```

**Index Types:**
- `UNIQUE` - Unique index (enforces uniqueness)
- `CLUSTERED` - Clustered index (typically primary key)
- `NONCLUSTERED` - Non-clustered index (default)
- `COMPOSITE` - Multi-column index

**Examples:**
```markdown
### Indexes

- `IX_Brokers_LicenseNumber` (UNIQUE) - Ensure license number uniqueness
- `IX_Brokers_Name` - Fast broker name lookups
- `IX_Brokers_State` - Filter brokers by state
- `IX_Brokers_Status` - Filter brokers by status
- `IX_Submissions_BrokerId` - Foreign key index for joins
- `IX_Submissions_Status_CreatedAt` (COMPOSITE) - Filter by status and sort by date
```

---

## Validation Rules

Specify validation rules that should be enforced at the application layer (in addition to database constraints).

**Format:**
```markdown
- **[FieldName]:**
  - [Rule]: [Description]
```

**Examples:**
```markdown
### Validation Rules

- **Name:**
  - Required: Cannot be null or empty
  - Length: 1-255 characters
  - Pattern: Alphanumeric with spaces and common punctuation

- **Email:**
  - Format: Valid email address format (RFC 5322)
  - Unique: No two brokers can have same email (if email is unique)

- **LicenseNumber:**
  - Required: Cannot be null or empty
  - Format: State code (2 letters) + hyphen + digits (e.g., "CA-12345")
  - Unique: No two brokers can have same license number

- **Status:**
  - Enum: Must be one of [Active, Inactive, Suspended]
  - Default: Active

- **Amount:**
  - Range: Must be greater than 0
  - Precision: Max 18 digits, 2 decimal places
```

---

## Audit Requirements

Specify how this entity is audited.

### Standard Audit Fields

**All entities must have:**
```markdown
- CreatedAt (DateTime, NOT NULL)
- CreatedBy (Guid, NOT NULL, FK → Users)
- UpdatedAt (DateTime, NOT NULL)
- UpdatedBy (Guid, NOT NULL, FK → Users)
- DeletedAt (DateTime, NULL) - for soft delete
```

### Timeline Events

**Specify which mutations create timeline events:**

```markdown
### Timeline Events

**Event Types:**
- `[Entity]Created` - When entity is first created
- `[Entity]Updated` - When entity is modified
- `[Entity]Deleted` - When entity is soft-deleted
- `[Entity]StatusChanged` - When status field changes
- `[Entity]Assigned` - When ownership/assignment changes

**Event Data:**
Each timeline event includes:
- EventType: Event type string
- EntityId: ID of this entity
- EntityType: "[EntityName]"
- Timestamp: UTC timestamp
- UserId: User who performed action
- Description: Human-readable description
- Changes: JSON object with old/new values for updates

**Example:**
When Broker name is updated from "Acme" to "Acme Insurance":
```json
{
  "eventType": "BrokerUpdated",
  "entityId": "123e4567-e89b-12d3-a456-426614174000",
  "entityType": "Broker",
  "timestamp": "2024-01-15T10:30:00Z",
  "userId": "987e6543-e21b-43a1-b789-123456789abc",
  "description": "Broker name updated",
  "changes": {
    "name": {
      "oldValue": "Acme",
      "newValue": "Acme Insurance"
    }
  }
}
```
```

---

## Seed Data Strategy

Specify whether this entity requires seed data and how it should be seeded.

**Options:**
- **No Seed Data** - All records are user-created
- **Reference Data** - Static lookup data (seeded via migrations)
- **Sample Data** - Example data for development/testing

**Examples:**

```markdown
### Seed Data Strategy

**Type:** Reference Data

**Seed Data Required:**
- Yes, this entity contains reference data that must exist for system to function

**Seed Data:**
```csharp
// US States reference data
new State { Code = "AL", Name = "Alabama" },
new State { Code = "AK", Name = "Alaska" },
new State { Code = "AZ", Name = "Arizona" },
// ... all 50 states
```

**Migration Strategy:**
- Seed data added in initial migration
- Updates via new migrations (not manual inserts)
- Never delete reference data (mark as inactive if needed)
```

OR

```markdown
### Seed Data Strategy

**Type:** No Seed Data

All brokers are created by users. No seed data needed.
```

---

## Soft Delete Behavior

Specify how soft deletes are handled for this entity.

```markdown
### Soft Delete Behavior

**Enabled:** Yes

**Implementation:**
- DeletedAt field is set to current UTC timestamp
- Deleted records are excluded from queries using global query filter
- Deleted records remain in database for audit purposes

**Cascade Behavior:**
- When this entity is soft-deleted:
  - Related [ChildEntity] records: [Cascade soft delete | Leave untouched | Prevent delete]
  - Example: When Broker is deleted, all Contacts are cascade soft-deleted

**Restore Capability:**
- Admin users can restore soft-deleted records
- Restore sets DeletedAt to NULL
```

---

## Business Rules

Specify any business rules that affect this entity's behavior.

```markdown
### Business Rules

**Creation Rules:**
- [Rule]: [Description]
- Example: A broker can only be created by users with "CreateBroker" permission

**Update Rules:**
- [Rule]: [Description]
- Example: LicenseNumber cannot be changed after creation (immutable)

**Delete Rules:**
- [Rule]: [Description]
- Example: Cannot delete broker if they have active submissions

**Status Transition Rules:**
- Active → Inactive: Allowed, requires reason
- Inactive → Active: Allowed, no restrictions
- Active → Suspended: Allowed, requires reason
- Suspended → Active: Allowed, requires admin approval
```

---

## EF Core Configuration

Specify EF Core-specific configuration details.

```markdown
### EF Core Configuration

**Entity Configuration Class:** `[EntityName]Configuration`

**Configuration Details:**

```csharp
public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        // Table
        builder.ToTable("Brokers");

        // Primary Key
        builder.HasKey(b => b.Id);

        // Properties
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Active");

        // Indexes
        builder.HasIndex(b => b.LicenseNumber)
            .IsUnique();

        builder.HasIndex(b => b.Name);

        // Relationships
        builder.HasMany(b => b.Contacts)
            .WithOne(c => c.Broker)
            .HasForeignKey(c => c.BrokerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global Query Filter (soft delete)
        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
```
```

---

## Related Entities

List entities that have relationships with this entity.

```markdown
### Related Entities

**Parent Entities:**
- [ParentEntity]: [Relationship description]

**Child Entities:**
- [ChildEntity]: [Relationship description]

**Peer Entities:**
- [PeerEntity]: [Relationship description]

**Example:**
- **Parent:** None (Broker is a root entity)
- **Children:**
  - Contact: Broker has many contacts
  - Submission: Broker submits many submissions
- **Peers:**
  - MGA: Brokers work with MGAs (many-to-many)
```

---

## Migration Notes

Any special notes for database migrations.

```markdown
### Migration Notes

**Initial Migration:**
- Create table with all fields
- Add indexes
- Add foreign key constraints
- Seed reference data (if applicable)

**Future Considerations:**
- Column additions should be nullable or have defaults
- Never drop columns (use soft deprecation)
- Data migrations may be needed for complex changes

**Backward Compatibility:**
- API contracts must remain compatible during migrations
- Use expand-contract pattern for breaking changes
```

---

## Example Usage

See `agents/architect/references/data-modeling-guide.md` for comprehensive examples and EF Core patterns.

---

## Version History

**Version 1.0** - 2026-01-26 - Initial entity model template

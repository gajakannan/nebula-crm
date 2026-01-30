# Architecture Examples

Real-world examples from Nebula showing complete specifications.

## Example 1: Broker Entity Specification

**Table Name:** `Brokers`

### Fields

| Field | Type | Constraints | Default | Description |
|-------|------|-------------|---------|-------------|
| Id | Guid | PK, NOT NULL | NewGuid() | Unique identifier |
| Name | string(255) | NOT NULL, INDEX | - | Broker legal name |
| LicenseNumber | string(50) | NOT NULL, UNIQUE | - | State license number |
| State | string(2) | NOT NULL, FK → States | - | Licensed state |
| Email | string(255) | NULL | - | Primary contact email |
| Phone | string(20) | NULL | - | Primary contact phone |
| Status | string(20) | NOT NULL | 'Active' | Active, Inactive, Suspended |
| CreatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp |
| CreatedBy | Guid | NOT NULL, FK → Users | - | User who created |
| UpdatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp |
| UpdatedBy | Guid | NOT NULL, FK → Users | - | User who last updated |
| DeletedAt | DateTime | NULL | - | Soft delete timestamp |

### Relationships

- **One-to-Many:** Broker → Contacts (cascade delete)
- **One-to-Many:** Broker → Submissions (restrict delete)
- **Self-Referencing:** Broker → ParentBroker (hierarchy)

### Indexes

- `IX_Brokers_LicenseNumber` (UNIQUE)
- `IX_Brokers_Name`
- `IX_Brokers_State`

---

## Example 2: Submission Workflow

### States

- Received → Triaging → ReadyForUWReview → InReview → Quoted → BindRequested → Bound
- Terminal: Bound, Declined, Withdrawn

### Transition: Triaging → ReadyForUWReview

**Prerequisites:**
- InsuredName populated
- CoverageType selected
- Program assigned
- Broker assigned

**Authorization:**
- Roles: Distribution, Admin
- Permission: TransitionSubmission

**Side Effects:**
- WorkflowTransition event created
- ActivityTimelineEvent logged
- Email to underwriters

**Error (Missing Fields):**
```json
{
  "code": "MISSING_REQUIRED_FIELDS",
  "message": "Cannot transition to ReadyForUWReview",
  "details": [
    {"field": "program", "message": "Program must be selected"}
  ]
}
```

---

## Example 3: API Contract - Create Broker

```yaml
paths:
  /api/brokers:
    post:
      summary: Create a new broker
      requestBody:
        required: true
        content:
          application/json:
            schema:
              required: [name, licenseNumber, state]
              properties:
                name: {type: string, maxLength: 255}
                licenseNumber: {type: string, maxLength: 50}
                state: {type: string, pattern: "^[A-Z]{2}$"}
      responses:
        '201':
          description: Broker created successfully
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/BrokerResponse'
        '400':
          description: Validation error
        '409':
          description: Duplicate license number
```

---

## Example 4: Authorization Policy

**Policy:** Distribution users can create/read/update brokers

```csv
p, Distribution, Broker, Create, allow
p, Distribution, Broker, Read, allow
p, Distribution, Broker, Update, allow
```

**Policy:** Underwriters can only read brokers

```csv
p, Underwriter, Broker, Read, allow
```

**Policy:** Underwriters can update submissions assigned to them

```csv
p, Underwriter, Submission, Update, allow, sub.userId == res.assignedUnderwriter
```

---

## Version History

**Version 1.0** - 2026-01-26 - Initial architecture examples

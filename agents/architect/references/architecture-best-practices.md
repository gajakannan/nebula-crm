# Architecture Best Practices

Comprehensive guide for designing robust, maintainable architecture for BrokerHub and similar enterprise applications.

## Table of Contents

1. [Clean Architecture Principles](#clean-architecture-principles)
2. [SOLID Principles](#solid-principles)
3. [Domain-Driven Design (DDD)](#domain-driven-design)
4. [Layering and Dependencies](#layering-and-dependencies)
5. [API Design Principles](#api-design-principles)
6. [Data Modeling Principles](#data-modeling-principles)
7. [Security Architecture](#security-architecture)
8. [Performance and Scalability](#performance-and-scalability)
9. [Observability](#observability)
10. [Common Anti-Patterns](#common-anti-patterns)

---

## Clean Architecture Principles

### Core Concepts

**Goal:** Create systems that are:
- Independent of frameworks
- Testable
- Independent of UI
- Independent of Database
- Independent of external agencies

### Layer Structure

```
┌─────────────────────────────────────┐
│         API / Presentation          │  ← Controllers, DTOs, Middleware
│    (ASP.NET Core, React)            │
└──────────────┬──────────────────────┘
               │ depends on ↓
┌──────────────┴──────────────────────┐
│          Application Layer          │  ← Use Cases, Interfaces, Application Services
│    (Business Workflows)             │
└──────────────┬──────────────────────┘
               │ depends on ↓
┌──────────────┴──────────────────────┐
│           Domain Layer              │  ← Entities, Value Objects, Domain Events
│    (Business Logic, Core)           │
└─────────────────────────────────────┘
               ↑ depended on by
┌──────────────┴──────────────────────┐
│       Infrastructure Layer          │  ← EF Core, External Services, File System
│    (Implementation Details)         │
└─────────────────────────────────────┘
```

**Key Rule:** Dependencies flow inward. Inner layers know nothing about outer layers.

### Domain Layer (Core)

**Contains:**
- Entities (Broker, Submission, Renewal)
- Value Objects (Money, Address, EmailAddress)
- Domain Events (BrokerCreated, SubmissionBound)
- Domain Services (complex business logic that doesn't belong to a single entity)
- Repository Interfaces (but NOT implementations)

**Does NOT Contain:**
- Database code (no EF Core)
- External service calls
- HTTP/API concerns
- UI logic

**Example Entity:**
```csharp
public class Broker
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public BrokerStatus Status { get; private set; }

    // Domain logic
    public void Activate()
    {
        if (Status == BrokerStatus.Suspended)
            throw new DomainException("Cannot activate a suspended broker");

        Status = BrokerStatus.Active;
        AddDomainEvent(new BrokerActivated(Id));
    }

    // Factory method
    public static Broker Create(string name, string licenseNumber)
    {
        // Validation
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name is required");

        var broker = new Broker
        {
            Id = Guid.NewGuid(),
            Name = name,
            LicenseNumber = licenseNumber,
            Status = BrokerStatus.Active
        };

        broker.AddDomainEvent(new BrokerCreated(broker.Id, broker.Name));
        return broker;
    }
}
```

### Application Layer

**Contains:**
- Use Cases / Application Services (CreateBrokerUseCase, GetBrokerQuery)
- Commands and Queries (CQRS pattern)
- DTOs / Request/Response Models
- Application Interfaces (IEmailService, INotificationService)
- Validators
- Mappers (Entity → DTO)

**Does NOT Contain:**
- Database implementation details
- HTTP concerns (that's API layer)
- Domain logic (that's Domain layer)

**Example Use Case:**
```csharp
public class CreateBrokerUseCase
{
    private readonly IBrokerRepository _repository;
    private readonly IAuthorizationService _authz;
    private readonly ITimelineService _timeline;

    public async Task<BrokerDto> Execute(CreateBrokerCommand command, User user)
    {
        // Authorization check
        await _authz.CheckPermission(user, "CreateBroker");

        // Create domain entity
        var broker = Broker.Create(
            command.Name,
            command.LicenseNumber
        );

        // Persist
        await _repository.Add(broker);
        await _repository.SaveChanges();

        // Timeline event
        await _timeline.LogEvent(new BrokerCreatedEvent(broker.Id, user.Id));

        // Return DTO
        return MapToDto(broker);
    }
}
```

### Infrastructure Layer

**Contains:**
- EF Core DbContext and Configurations
- Repository Implementations
- External Service Implementations (Email, SMS, etc.)
- File System Access
- Caching Implementation

**Example Repository:**
```csharp
public class BrokerRepository : IBrokerRepository
{
    private readonly BrokerHubDbContext _context;

    public async Task<Broker> GetById(Guid id)
    {
        return await _context.Brokers
            .Include(b => b.Contacts)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task Add(Broker broker)
    {
        await _context.Brokers.AddAsync(broker);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
```

### API Layer

**Contains:**
- Controllers
- API DTOs (distinct from Application DTOs if needed)
- Middleware (auth, error handling, logging)
- API Versioning
- Swagger/OpenAPI configuration

**Example Controller:**
```csharp
[ApiController]
[Route("api/brokers")]
[Authorize]
public class BrokersController : ControllerBase
{
    private readonly CreateBrokerUseCase _createBroker;

    [HttpPost]
    [ProducesResponseType(typeof(BrokerResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Create([FromBody] CreateBrokerRequest request)
    {
        var command = MapToCommand(request);
        var result = await _createBroker.Execute(command, User);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result
        );
    }
}
```

---

## SOLID Principles

### Single Responsibility Principle (SRP)

**Definition:** A class should have only one reason to change.

**Bad Example:**
```csharp
public class BrokerService
{
    public void CreateBroker() { /* creates broker */ }
    public void SendEmail() { /* sends email */ }
    public void LogActivity() { /* logs to database */ }
    public void ValidateData() { /* validates */ }
}
// This class has 4 reasons to change!
```

**Good Example:**
```csharp
public class BrokerService
{
    private readonly IBrokerRepository _repository;
    private readonly IEmailService _email;
    private readonly ITimelineService _timeline;
    private readonly IValidator<Broker> _validator;

    public void CreateBroker()
    {
        // Orchestrates, delegates to specialized services
    }
}
```

### Open/Closed Principle (OCP)

**Definition:** Software entities should be open for extension, closed for modification.

**Use Strategy Pattern for Varying Behavior:**
```csharp
public interface IQuoteCalculator
{
    decimal Calculate(Submission submission);
}

public class RestaurantQuoteCalculator : IQuoteCalculator
{
    public decimal Calculate(Submission submission) { /* restaurant logic */ }
}

public class RetailQuoteCalculator : IQuoteCalculator
{
    public decimal Calculate(Submission submission) { /* retail logic */ }
}

// Add new calculators without modifying existing code
```

### Liskov Substitution Principle (LSP)

**Definition:** Subtypes must be substitutable for their base types.

**Avoid violating LSP:**
```csharp
// Bad: Square violates LSP if it inherits from Rectangle
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
}

public class Square : Rectangle
{
    // Violates LSP: SetWidth changes Height too
    public override int Width
    {
        set { base.Width = base.Height = value; }
    }
}

// Good: Use composition or separate hierarchies
public interface IShape
{
    int Area();
}

public class Rectangle : IShape { }
public class Square : IShape { }
```

### Interface Segregation Principle (ISP)

**Definition:** Clients should not be forced to depend on interfaces they don't use.

**Bad Example:**
```csharp
public interface IBrokerRepository
{
    Task<Broker> GetById(Guid id);
    Task Add(Broker broker);
    Task Update(Broker broker);
    Task Delete(Guid id);
    Task<List<Broker>> Search(string term);
    Task<BrokerStatistics> GetStatistics(Guid id);
    Task<List<Submission>> GetSubmissions(Guid id);
}
// Too many responsibilities!
```

**Good Example:**
```csharp
public interface IBrokerRepository
{
    Task<Broker> GetById(Guid id);
    Task Add(Broker broker);
    Task SaveChanges();
}

public interface IBrokerQueryService
{
    Task<List<Broker>> Search(string term);
    Task<BrokerStatistics> GetStatistics(Guid id);
}

public interface IBrokerSubmissionsService
{
    Task<List<Submission>> GetSubmissions(Guid id);
}
```

### Dependency Inversion Principle (DIP)

**Definition:** High-level modules should not depend on low-level modules. Both should depend on abstractions.

**Bad Example:**
```csharp
public class BrokerService
{
    private SqlBrokerRepository _repository; // Concrete dependency!

    public BrokerService()
    {
        _repository = new SqlBrokerRepository(); // Tightly coupled!
    }
}
```

**Good Example:**
```csharp
public class BrokerService
{
    private readonly IBrokerRepository _repository; // Abstract dependency

    public BrokerService(IBrokerRepository repository) // Injected
    {
        _repository = repository;
    }
}

// Configured in Startup.cs:
services.AddScoped<IBrokerRepository, SqlBrokerRepository>();
```

---

## Domain-Driven Design (DDD)

### Bounded Contexts

**Definition:** Explicit boundaries within which a domain model is defined.

**BrokerHub Bounded Contexts:**
- **Broker Management Context:** Broker, Contact, BrokerHierarchy
- **Submission Context:** Submission, Quote, Underwriting
- **Renewal Context:** Renewal, RenewalTerms
- **Identity Context:** User, Role, Permission

**Context Mapping:**
- Contexts communicate through well-defined interfaces
- Shared Kernel: Common types (Money, Address)
- Anti-Corruption Layer: Translate between contexts

### Aggregates

**Definition:** Cluster of domain objects treated as a single unit.

**Rules:**
- One entity is the Aggregate Root (e.g., Broker)
- External objects can only reference the root
- Aggregates are transactional boundaries

**Example:**
```csharp
public class Broker // Aggregate Root
{
    private readonly List<Contact> _contacts; // Part of aggregate
    public IReadOnlyList<Contact> Contacts => _contacts.AsReadOnly();

    public void AddContact(Contact contact)
    {
        // Business rule enforced at aggregate level
        if (_contacts.Count >= 10)
            throw new DomainException("Cannot exceed 10 contacts per broker");

        _contacts.Add(contact);
    }
}

// Contact is only accessible through Broker
```

### Value Objects

**Definition:** Objects defined by their attributes, not identity.

**Characteristics:**
- Immutable
- No identity
- Replaceable

**Example:**
```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }

    // Value objects are compared by value, not reference
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    // Immutable operations return new instances
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return new Money(Amount + other.Amount, Currency);
    }
}
```

### Domain Events

**Definition:** Something that happened in the domain that domain experts care about.

**Example:**
```csharp
public class BrokerCreated : DomainEvent
{
    public Guid BrokerId { get; }
    public string BrokerName { get; }
    public DateTime OccurredAt { get; }

    public BrokerCreated(Guid brokerId, string brokerName)
    {
        BrokerId = brokerId;
        BrokerName = brokerName;
        OccurredAt = DateTime.UtcNow;
    }
}

// Entity raises events
public class Broker
{
    private List<DomainEvent> _domainEvents = new();
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

// Events are dispatched after SaveChanges
public async Task SaveChanges()
{
    await _context.SaveChangesAsync();
    await DispatchDomainEvents();
}
```

---

## Layering and Dependencies

### Dependency Flow Rules

```
API Layer
   ↓ (depends on)
Application Layer
   ↓ (depends on)
Domain Layer
   ↑ (implemented by)
Infrastructure Layer
```

**Key Rules:**
1. Domain Layer has NO dependencies (pure C#)
2. Application Layer depends ONLY on Domain
3. Infrastructure implements interfaces from Domain/Application
4. API Layer orchestrates, delegates to Application

### Dependency Injection

**Configure in Program.cs / Startup.cs:**
```csharp
// Domain Services
services.AddScoped<IBrokerDomainService, BrokerDomainService>();

// Application Services
services.AddScoped<CreateBrokerUseCase>();
services.AddScoped<GetBrokerQuery>();

// Infrastructure
services.AddScoped<IBrokerRepository, BrokerRepository>();
services.AddDbContext<BrokerHubDbContext>(options =>
    options.UseNpgsql(connectionString));

// External Services
services.AddScoped<IEmailService, SendGridEmailService>();
services.AddScoped<ITimelineService, TimelineService>();
```

---

## API Design Principles

### RESTful Conventions

- Use nouns for resources (`/api/brokers`, not `/api/getBrokers`)
- Use HTTP verbs correctly (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 400, 404, 500)
- Use plural nouns (`/brokers`, not `/broker`)
- Use hierarchical URLs for relationships (`/brokers/{id}/contacts`)

### Versioning

**URL Versioning (Recommended):**
```
/api/v1/brokers
/api/v2/brokers
```

**Header Versioning (Alternative):**
```
GET /api/brokers
Accept: application/vnd.brokerhub.v1+json
```

### Error Handling

**Consistent Error Contract:**
```json
{
  "code": "VALIDATION_ERROR",
  "message": "Invalid request data",
  "details": [
    {
      "field": "name",
      "message": "Name is required"
    }
  ],
  "traceId": "550e8400-e29b-41d4-a716-446655440000"
}
```

See `agents/architect/references/api-design-guide.md` for comprehensive API design patterns.

---

## Data Modeling Principles

### Normalization

- **1NF:** Atomic values, no repeating groups
- **2NF:** No partial dependencies
- **3NF:** No transitive dependencies

**When to Denormalize:**
- Read-heavy queries with complex joins
- Performance requirements dictate it
- Always document why

### Indexes

**When to Add Index:**
- Foreign keys (EF Core doesn't auto-index FKs!)
- Columns used in WHERE clauses
- Columns used in ORDER BY
- Columns used in JOIN conditions

**When NOT to Add Index:**
- Small tables (< 1000 rows)
- Columns rarely queried
- Columns with low selectivity (e.g., boolean flags)

### Audit Fields

**Every entity should have:**
```csharp
public DateTime CreatedAt { get; set; }
public Guid CreatedBy { get; set; }
public DateTime UpdatedAt { get; set; }
public Guid UpdatedBy { get; set; }
public DateTime? DeletedAt { get; set; } // Soft delete
```

See `agents/architect/references/data-modeling-guide.md` for comprehensive data modeling patterns.

---

## Security Architecture

### Defense in Depth

**Multiple layers of security:**
1. **Network:** HTTPS, firewall
2. **Authentication:** Keycloak (OIDC/JWT)
3. **Authorization:** Casbin (ABAC)
4. **Application:** Input validation, SQL injection prevention
5. **Data:** Encryption at rest, TDE
6. **Audit:** Comprehensive logging

### Authorization Layers

```
┌─────────────────────────┐
│   API Controller        │ ← [Authorize] attribute
└──────────┬──────────────┘
           │
           ↓
┌──────────┴──────────────┐
│   Authorization Service │ ← Casbin ABAC check
└──────────┬──────────────┘
           │
           ↓
┌──────────┴──────────────┐
│   Application Service   │ ← Business rule validation
└──────────┬──────────────┘
           │
           ↓
┌──────────┴──────────────┐
│   Domain Entity         │ ← Invariant enforcement
└─────────────────────────┘
```

### Principle of Least Privilege

- Users get ONLY permissions they need
- Default deny (explicit allow required)
- Separate read and write permissions
- Row-level security where applicable

See `agents/architect/references/authorization-patterns.md` for comprehensive authorization patterns.

---

## Performance and Scalability

### Database Performance

**Avoid N+1 Queries:**
```csharp
// Bad: N+1
var brokers = await _context.Brokers.ToListAsync();
foreach (var broker in brokers)
{
    var contacts = await _context.Contacts.Where(c => c.BrokerId == broker.Id).ToListAsync();
}

// Good: Eager loading
var brokers = await _context.Brokers
    .Include(b => b.Contacts)
    .ToListAsync();
```

**Use Projections:**
```csharp
// Bad: Load entire entity
var brokers = await _context.Brokers.ToListAsync();

// Good: Project to DTO
var brokers = await _context.Brokers
    .Select(b => new BrokerListDto
    {
        Id = b.Id,
        Name = b.Name,
        Status = b.Status
    })
    .ToListAsync();
```

### Caching Strategy

**Cache Layers:**
1. **Memory Cache:** Short-lived, high-speed (reference data)
2. **Distributed Cache:** Redis for shared state
3. **CDN:** Static assets

**What to Cache:**
- Reference data (states, programs)
- Infrequently changing data (broker details)
- Expensive calculations (broker statistics)

**What NOT to Cache:**
- Sensitive data
- Rapidly changing data
- Large objects

### Pagination

**Always paginate list endpoints:**
```csharp
public async Task<PagedResult<BrokerDto>> GetBrokers(int page, int pageSize)
{
    var query = _context.Brokers.AsQueryable();

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<BrokerDto>
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
```

---

## Observability

### Logging Levels

- **Trace:** Very detailed, disabled in production
- **Debug:** Diagnostic info, disabled in production
- **Information:** General flow (requests, key events)
- **Warning:** Unexpected but recoverable (retry logic, degraded performance)
- **Error:** Errors that require attention
- **Critical:** Application crash, data loss

### Structured Logging

```csharp
_logger.LogInformation(
    "Broker {BrokerId} created by user {UserId}",
    brokerId,
    userId
);

// JSON output:
// {
//   "timestamp": "2024-01-15T10:30:00Z",
//   "level": "Information",
//   "message": "Broker 123... created by user 456...",
//   "brokerId": "123e4567-e89b-12d3-a456-426614174000",
//   "userId": "987e6543-e21b-43a1-b789-123456789abc"
// }
```

### Correlation IDs

**Track requests across services:**
```csharp
public class CorrelationIdMiddleware
{
    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add("X-Correlation-ID", correlationId);

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }
}
```

---

## Common Anti-Patterns

### God Objects

**Anti-Pattern:** One class does everything
```csharp
public class BrokerManager
{
    public void CreateBroker() { }
    public void UpdateBroker() { }
    public void DeleteBroker() { }
    public void SendEmail() { }
    public void GenerateReport() { }
    public void CalculateStatistics() { }
    // ... 50 more methods
}
```

**Solution:** Split into focused classes with single responsibilities

### Anemic Domain Model

**Anti-Pattern:** Entities with no behavior, all logic in services
```csharp
public class Broker
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    // Just properties, no behavior
}

public class BrokerService
{
    public void ActivateBroker(Broker broker)
    {
        broker.Status = "Active"; // Logic outside entity
    }
}
```

**Solution:** Move behavior into entities
```csharp
public class Broker
{
    public void Activate()
    {
        if (Status == BrokerStatus.Suspended)
            throw new DomainException("Cannot activate suspended broker");

        Status = BrokerStatus.Active;
    }
}
```

### Leaky Abstractions

**Anti-Pattern:** Implementation details leak through interfaces
```csharp
public interface IBrokerRepository
{
    Task<IQueryable<Broker>> GetQueryable(); // Leaks EF Core IQueryable!
}
```

**Solution:** Hide implementation details
```csharp
public interface IBrokerRepository
{
    Task<List<Broker>> Search(BrokerSearchCriteria criteria);
}
```

### Magic Strings/Numbers

**Anti-Pattern:**
```csharp
if (broker.Status == "Active") // Magic string
{
    // ...
}
```

**Solution:** Use enums or constants
```csharp
public enum BrokerStatus
{
    Active,
    Inactive,
    Suspended
}

if (broker.Status == BrokerStatus.Active)
{
    // ...
}
```

---

## Further Reading

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Implementing Domain-Driven Design by Vaughn Vernon
- Building Microservices by Sam Newman
- Microsoft .NET Architecture Guides: https://learn.microsoft.com/dotnet/architecture/

---

## Version History

**Version 1.0** - 2026-01-26 - Initial architecture best practices guide

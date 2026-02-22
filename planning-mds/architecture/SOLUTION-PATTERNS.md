# Nebula CRM - Solution-Specific Patterns

**Purpose:** This document captures architectural decisions, design patterns, and coding conventions specific to the Nebula CRM solution. These patterns ensure consistency across all implementations and serve as institutional memory.

**Last Updated:** 2026-02-17

---

## How to Use This Document

### During Planning (Architect)
- Read patterns before designing architecture
- Apply patterns to new designs
- Reference patterns in ADRs
- Update patterns when new conventions emerge

### During Implementation (Developers)
- Read relevant patterns before coding
- Follow established conventions
- Ask if pattern doesn't fit your use case
- Propose pattern updates via ADR

### During Review (Code Reviewer, Security)
- Validate implementations against patterns
- Check for pattern violations
- Suggest pattern improvements
- Approve pattern updates

---

## 1. Authorization Patterns

### Pattern: Use Casbin ABAC
**Decision:** All authorization uses Casbin with Attribute-Based Access Control (ABAC)
**Rationale:** Need fine-grained, attribute-based permissions beyond simple role-based access
**Applied in:** All API endpoints, domain services
**Enforcement:** Server-side only (never trust client)

**Example:**
```csharp
// Check permission before operation
if (!await _authorizationService.Authorize(user, "broker", "read", broker))
{
    return Forbid();
}
```

### Pattern: JWT Token Authentication
**Decision:** Use JWT tokens issued by Keycloak for authentication
**Rationale:** Standard, stateless, integrates with existing identity provider
**Applied in:** All API requests
**Token Location:** Authorization header (Bearer token)

### Pattern: Per-Endpoint Authorization
**Decision:** Every API endpoint must explicitly check authorization
**Rationale:** Defense in depth, fail-secure
**Applied in:** All controllers/endpoints

**Example:**
```csharp
[HttpGet("{id}")]
[Authorize] // Authentication required
public async Task<IActionResult> GetBroker(Guid id)
{
    // Explicit authorization check
    var broker = await _brokerService.GetAsync(id);
    if (!await _authz.CanRead(User, broker))
        return Forbid();

    return Ok(broker);
}
```

---

## 2. Audit & Timeline Patterns

### Pattern: All Mutations Create Timeline Events
**Decision:** Every data mutation creates an immutable ActivityTimelineEvent
**Rationale:** Compliance requires complete audit trail of all changes
**Applied in:** All services that modify data

**Example:**
```csharp
public async Task<Broker> UpdateBrokerAsync(Guid id, UpdateBrokerDto dto)
{
    var broker = await _repo.GetAsync(id);
    broker.Update(dto);

    // MUST create timeline event
    await _timelineService.CreateEventAsync(new ActivityTimelineEvent
    {
        EntityType = "Broker",
        EntityId = id,
        EventType = "BrokerUpdated",
        Description = $"Broker {broker.Name} updated",
        PerformedBy = _currentUser.Id,
        PerformedAt = DateTime.UtcNow,
        Metadata = JsonSerializer.Serialize(dto)
    });

    await _repo.SaveAsync();
    return broker;
}
```

### Pattern: Workflow Transitions are Append-Only
**Decision:** WorkflowTransition table is append-only (immutable history)
**Rationale:** Audit compliance, state machine integrity
**Applied in:** Submission workflow, Renewal workflow

**Example:**
```csharp
// Never update or delete - always append
var transition = new WorkflowTransition
{
    SubmissionId = submission.Id,
    FromState = submission.Status,
    ToState = newStatus,
    TransitionedBy = _currentUser.Id,
    TransitionedAt = DateTime.UtcNow,
    Reason = reason
};

await _transitionRepo.AddAsync(transition); // Append only!
submission.Status = newStatus;
```

---

## 3. API Design Patterns

### Pattern: REST Resource Conventions
**Decision:** API endpoints follow `/api/{resource}` and `/api/{resource}/{id}` pattern
**Rationale:** Simplicity, RESTful conventions, no versioning complexity for MVP
**Applied in:** All REST endpoints

**Examples:**
```
GET    /api/brokers           - List brokers
POST   /api/brokers           - Create broker
GET    /api/brokers/{id}      - Get broker detail
PUT    /api/brokers/{id}      - Update broker
DELETE /api/brokers/{id}      - Delete broker
```

**Sub-resource vs query-parameter convention:**
Use flat collection endpoints with query-parameter filters (e.g. `GET /api/contacts?brokerId=...`)
when the child entity may be queried across parents or independently. Use sub-resource URLs
(e.g. `GET /api/brokers/{id}/submissions`) only when the child is always accessed in the context
of a single parent. For Nebula MVP, contacts use the query-parameter pattern; submissions and
renewals use the sub-resource pattern.

### Pattern: RFC 7807 ProblemDetails for Errors
**Decision:** All API errors return RFC 7807 ProblemDetails format
**Rationale:** Standard, machine-readable error format
**Applied in:** All API error responses

**Example:**
```csharp
return Problem(
    title: "Broker not found",
    detail: $"Broker with ID {id} does not exist",
    statusCode: StatusCodes.Status404NotFound,
    type: "https://docs.nebula.com/errors/broker-not-found"
);
```

### Pattern: Pagination for List Endpoints
**Decision:** All list endpoints support pagination with query parameters
**Rationale:** Performance, avoid returning huge datasets
**Applied in:** All GET collection endpoints

**Example:**
```
GET /api/brokers?page=1&pageSize=20
GET /api/submissions?page=2&pageSize=50
```

---

## 4. Clean Architecture Patterns

### Pattern: Four-Layer Architecture
**Decision:** Domain → Application → Infrastructure → API layers
**Rationale:** Clean Architecture, separation of concerns, testability
**Applied in:** Entire backend codebase

**Layer Rules:**
```
Domain Layer (innermost)
  - Entities, Value Objects, Domain Events
  - No dependencies on outer layers
  - Pure business logic

Application Layer
  - Services, Use Cases, Interfaces
  - Depends only on Domain
  - Defines repository interfaces

Infrastructure Layer
  - EF Core, Repositories, External Services
  - Implements Application interfaces
  - Depends on Application and Domain

API Layer (outermost)
  - Controllers, DTOs, Middleware
  - Depends on Application
  - Thin layer, delegates to Application services
```

### Pattern: Repository Pattern
**Decision:** Use repository pattern for data access
**Rationale:** Abstraction, testability, clean separation
**Applied in:** All data access

**Example:**
```csharp
// Application layer defines interface
public interface IBrokerRepository
{
    Task<Broker> GetAsync(Guid id);
    Task<IEnumerable<Broker>> ListAsync();
    Task AddAsync(Broker broker);
}

// Infrastructure layer implements with EF Core
public class BrokerRepository : IBrokerRepository
{
    private readonly AppDbContext _context;
    // Implementation...
}
```

---

## 5. Frontend Patterns

### Pattern: React Hook Form for Manual Forms
**Decision:** Manual forms (static, custom layouts) use React Hook Form
**Rationale:** Performance, minimal re-renders, great DX, full control over UI
**Applied in:** User-facing forms with custom designs and branding

**Example:**
```tsx
const { register, handleSubmit, formState: { errors } } = useForm<BrokerFormData>({
  resolver: ajvResolver(brokerSchema) // JSON Schema validation
});

<form onSubmit={handleSubmit(onSubmit)}>
  <input {...register("name")} />
  {errors.name && <span className="text-red-500">{errors.name.message}</span>}
</form>
```

### Pattern: RJSF for Dynamic Forms
**Decision:** Dynamic/admin forms use RJSF (React JSON Schema Form)
**Rationale:** Auto-generates forms from JSON Schema, great for admin panels, configurable forms, rapid development
**Applied in:** Admin interfaces, schema-driven forms, configurable workflows
**When to use:** Forms that need to adapt to changing schemas, or when speed > custom design

**Example:**
```tsx
import Form from '@rjsf/core';
import validator from '@rjsf/validator-ajv8';

// Schema loaded from shared location
const schema = {
  type: 'object',
  properties: {
    name: { type: 'string', title: 'Broker Name' },
    email: { type: 'string', format: 'email' },
  },
  required: ['name', 'email']
};

<Form
  schema={schema}
  validator={validator}
  onSubmit={({ formData }) => handleSubmit(formData)}
/>
```

**Custom Widgets:**
```tsx
// Use shadcn/ui components as RJSF widgets
const CustomTextWidget = (props) => (
  <Input
    value={props.value}
    onChange={(e) => props.onChange(e.target.value)}
  />
);

const widgets = { TextWidget: CustomTextWidget };

<Form schema={schema} validator={validator} widgets={widgets} />
```

### Pattern: JSON Schema + AJV for Validation
**Decision:** Use JSON Schema for validation with AJV validator
**Rationale:** Language-agnostic schemas shared between frontend (TypeScript) and backend (C#), consistent validation rules across tiers
**Applied in:** All forms with validation, API request/response validation
**Schema Location:** `planning-mds/schemas/` or `experience/src/schemas/` (shared schemas in planning-mds)

**Why JSON Schema + AJV for this architecture:**
- Backend C# can validate using same schema (NJsonSchema or similar)
- Single source of truth for validation rules
- TypeScript types can be generated from schemas
- Industry standard, works with OpenAPI

**Example:**
```tsx
import { ajvResolver } from '@hookform/resolvers/ajv';
import Ajv from 'ajv';
import addErrors from 'ajv-errors';
import type { JSONSchemaType } from 'ajv';

// Define schema (can be loaded from shared JSON file)
interface BrokerFormData {
  name: string;
  email: string;
  state: string;
}

const brokerSchema: JSONSchemaType<BrokerFormData> = {
  type: 'object',
  properties: {
    name: {
      type: 'string',
      minLength: 1,
      errorMessage: 'Name is required'
    },
    email: {
      type: 'string',
      format: 'email',
      errorMessage: 'Invalid email address'
    },
    state: {
      type: 'string',
      minLength: 2,
      maxLength: 2,
      errorMessage: 'State must be 2 characters'
    }
  },
  required: ['name', 'email', 'state'],
  additionalProperties: false
};

// Use with React Hook Form
const ajv = new Ajv({ allErrors: true });
addErrors(ajv);

const { register, handleSubmit } = useForm<BrokerFormData>({
  resolver: ajvResolver(brokerSchema)
});
```

**Alternative: Load from shared schema file**
```tsx
// Load schema from planning-mds/schemas/broker.schema.json
import brokerSchema from '@/schemas/broker.schema.json';
import { ajvResolver } from '@hookform/resolvers/ajv';

const { register, handleSubmit } = useForm({
  resolver: ajvResolver(brokerSchema)
});
```

### Pattern: TanStack Query for API Calls
**Decision:** All API calls use TanStack Query (React Query)
**Rationale:** Caching, loading states, error handling, refetching
**Applied in:** All data fetching

**Example:**
```tsx
const { data: brokers, isLoading, error } = useQuery({
  queryKey: ['brokers'],
  queryFn: () => api.brokers.list()
});
```

### Pattern: Tailwind + shadcn/ui for Styling
**Decision:** Use Tailwind CSS utility classes + shadcn/ui components
**Rationale:** Consistency, accessibility, customizable, good DX
**Applied in:** All UI components

**Example:**
```tsx
<Button variant="primary" size="lg" className="mt-4">
  Create Broker
</Button>
```

---

## 6. Data Modeling Patterns

### Pattern: Audit Fields on All Entities
**Decision:** All entities include audit fields
**Rationale:** Track who created/modified, when
**Applied in:** All domain entities

**Required Fields:**
```csharp
public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!; // Keycloak subject (sub claim)
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; } // Keycloak subject (sub claim)
}
```

### Pattern: Soft Delete Support
**Decision:** Entities support soft delete (not hard delete)
**Rationale:** Data recovery, audit compliance, referential integrity
**Applied in:** All deletable entities

**Additional Fields:**
```csharp
public bool IsDeleted { get; set; }
public DateTime? DeletedAt { get; set; }
public string? DeletedBy { get; set; } // Keycloak subject (sub claim)
```

### Pattern: Use GUIDs for Primary Keys
**Decision:** All primary keys are GUIDs (not integers)
**Rationale:** Distributed systems, no conflicts, security (non-sequential)
**Applied in:** All entities

---

## 7. Testing Patterns

**Reference:** See `planning-mds/architecture/TESTING-STRATEGY.md` for comprehensive testing strategy across all tiers.

### Pattern: Test Pyramid (70-20-10 Rule)
**Decision:** 70% unit tests, 20% integration tests, 10% E2E tests
**Rationale:** Fast feedback (unit), contract verification (integration), critical flow validation (E2E)
**Applied in:** All layers (Frontend, Backend, AI/Neuron)

**Distribution:**
- **Unit Tests:** Test business logic in isolation (fast, cheap)
- **Integration Tests:** Test API contracts, database access (medium speed/cost)
- **E2E Tests:** Test critical user flows (slow, expensive)

### Pattern: Testing Tools by Layer

**Frontend (experience/):**
- **Unit/Component:** Vitest + React Testing Library
- **Integration:** Vitest + MSW (Mock Service Worker)
- **E2E:** Playwright (with MCP automation)
- **Accessibility:** @axe-core/playwright, jest-axe
- **Performance:** Lighthouse CI, Web Vitals

**Backend (engine/):**
- **Unit:** xUnit + FluentAssertions
- **Integration:** xUnit + WebApplicationFactory
- **API Collections:** Bruno CLI or curl scripts
- **Database:** xUnit + Testcontainers (real PostgreSQL)
- **Contract:** Pact.NET (consumer-driven contracts)
- **Load:** k6
- **Coverage:** Coverlet + ReportGenerator

**AI/Neuron (neuron/):**
- **Unit:** pytest
- **LLM:** pytest + mocking (fast, no API calls)
- **Evaluation:** pytest + custom metrics (accuracy, hallucination, cost)
- **Integration:** pytest + real API (use sparingly)
- **MCP Server:** pytest + FastAPI TestClient
- **Performance:** pytest-benchmark

### Pattern: ≥80% Unit Test Coverage for Business Logic
**Decision:** Minimum 80% code coverage for domain and application layers
**Rationale:** Quality assurance, regression prevention
**Applied in:** All domain services, application services
**Measurement:** Coverlet (backend), Vitest (frontend), pytest-cov (AI)

### Pattern: Integration Tests for All API Endpoints
**Decision:** Every API endpoint has integration tests
**Rationale:** Verify end-to-end functionality, catch integration issues
**Applied in:** All API controllers
**Tool:** WebApplicationFactory + xUnit (backend), MSW (frontend mocks)

**Example:**
```csharp
[Fact]
public async Task CreateBroker_WithValidData_ReturnsCreated()
{
    // Arrange
    var client = _factory.CreateClient();
    var dto = new CreateBrokerDto { Name = "Test", Email = "test@example.com" };

    // Act
    var response = await client.PostAsJsonAsync("/api/brokers", dto);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

### Pattern: E2E Tests for Critical Workflows Only
**Decision:** All critical user workflows have E2E tests (not every feature)
**Rationale:** Verify complete user journeys, but minimize slow/expensive tests
**Applied in:** Submission workflow, Renewal workflow, Broker CRUD, Login flow
**Tool:** Playwright (cross-browser: Chrome, Firefox, Safari)

### Pattern: Real Database for Integration Tests (Testcontainers)
**Decision:** Use Testcontainers (real PostgreSQL) for database integration tests, not in-memory
**Rationale:** PostgreSQL-specific features (JSONB, triggers, full-text search), production-like environment
**Applied in:** Repository tests, migration tests
**Tool:** Testcontainers.PostgreSql

**Example:**
```csharp
private PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
    .WithImage("postgres:15")
    .Build();

await _postgresContainer.StartAsync();
var connectionString = _postgresContainer.GetConnectionString();
```

### Pattern: Contract Testing (Frontend ↔ Backend)
**Decision:** Use Pact for consumer-driven contract testing
**Rationale:** Verify frontend expectations match backend implementation
**Applied in:** All API integrations
**Tool:** Pact.NET (backend), Pact JS (frontend)

### Pattern: AI Agent Evaluation Metrics
**Decision:** Measure agent quality with custom metrics
**Metrics:**
- **Accuracy:** Compare output to golden dataset (≥85% threshold)
- **Hallucination Rate:** Detect invented information (<5%)
- **Latency:** p95 response time (<2 seconds)
- **Cost:** Token usage per request
- **Success Rate:** % of tasks completed (≥90%)

**Tool:** pytest with custom evaluation framework

### Pattern: Accessibility Testing (WCAG 2.1 AA)
**Decision:** All screens must pass WCAG 2.1 AA compliance
**Rationale:** Legal requirement, inclusive design
**Applied in:** All frontend components
**Tool:** @axe-core/playwright (E2E), jest-axe (component tests)

**Example:**
```typescript
test('broker form has no a11y violations', async ({ page }) => {
  await page.goto('/brokers/new');
  const results = await new AxeBuilder({ page }).analyze();
  expect(results.violations).toEqual([]);
});
```

### Pattern: Performance Testing Thresholds
**Decision:** Define and enforce performance thresholds
**Thresholds:**
- **Frontend:** LCP < 2.5s, FID < 100ms, CLS < 0.1
- **Backend:** p95 response time < 500ms, error rate < 1%
- **Load:** Support 100 concurrent users with <500ms p95 response

**Tools:** Lighthouse CI (frontend), k6 (backend load testing)

---

## 8. Workflow Patterns

### Pattern: Temporal.io for Workflow Engine
**Decision:** Use Temporal for long-running workflows
**Rationale:** Durable execution, retry logic, workflow visibility
**Applied in:** Submission workflow, Renewal workflow

### Pattern: State Machines for Business Workflows
**Decision:** Model workflows as explicit state machines
**Rationale:** Clear transitions, validation, audit trail
**Applied in:** Submission (8 states), Renewal (6 states)

**Example:**
```csharp
public enum SubmissionStatus
{
    Received,
    Triaging,
    WaitingOnBroker,
    ReadyForUWReview,
    InReview,
    Quoted,
    BindRequested,
    Bound
}

// Valid transitions defined
private static readonly Dictionary<SubmissionStatus, List<SubmissionStatus>> ValidTransitions = new()
{
    { SubmissionStatus.Received, new() { SubmissionStatus.Triaging } },
    { SubmissionStatus.Triaging, new() { SubmissionStatus.WaitingOnBroker, SubmissionStatus.ReadyForUWReview } },
    // etc.
};
```

---

## 9. Cross-Cutting Concerns

### Pattern: Structured Logging with Serilog
**Decision:** Use Serilog for structured logging
**Rationale:** Queryable logs, context preservation
**Applied in:** All services, controllers

**Example:**
```csharp
_logger.LogInformation(
    "Broker {BrokerId} updated by {UserId}",
    broker.Id,
    currentUser.Id
);
```

### Pattern: Global Exception Handler
**Decision:** Use global exception handling middleware
**Rationale:** Consistent error responses, avoid scattered try-catch
**Applied in:** API layer

### Pattern: In-Memory-First Caching (Cache-Aside Default)
**Decision:** Use in-memory caching (MemoryCache) for small, low-churn datasets; use external cache (Redis) only when cross-instance consistency or scale requires it.
**Rationale:** Lower operational complexity during early implementation while preserving an upgrade path for scale.
**Applied in:** Reference data, dashboard aggregates, request-scoped ABAC resolution.

**When to use in-memory cache:**
- Data is small and changes infrequently (reference tables).
- Cache is per-process or per-request (ABAC scope resolution).
- Cross-instance consistency is not required.

**When to use external cache (Redis):**
- Multiple app instances must share cache state.
- Cache entry size or volume exceeds in-memory thresholds.
- Cache invalidation must be centralized.

**Pattern choice:**
- **Cache-aside (default):** Read from cache; on miss, load from DB and populate.
- **Write-through (selective):** Admin-managed reference data updates that require immediate consistency.
- **Write-behind:** Not used in MVP.

**Security notes:**
- Avoid caching secrets or raw PII without explicit approval.
- Include tenant/subject identifiers in cache keys for scoped data.

### Pattern: OWASP Top 10 Compliance
**Decision:** Follow OWASP Top 10 security practices
**Rationale:** Industry standard security baseline
**Applied in:** All code
**Checks:** Input validation, SQL injection prevention, XSS prevention, CSRF protection, etc.

---

## 10. DevOps Patterns

### Pattern: Docker for All Services
**Decision:** All services run in Docker containers
**Rationale:** Consistency, reproducibility, deployment simplicity
**Applied in:** Backend, Frontend, Database, Keycloak

### Pattern: docker-compose for Local Development
**Decision:** Use docker-compose for local dev environment
**Rationale:** Easy setup, matches production, all services together
**Applied in:** Development workflow

### Pattern: Environment Variables for Configuration
**Decision:** All configuration via environment variables (12-factor app)
**Rationale:** Security (no secrets in code), environment-specific config
**Applied in:** All services

**Example:**
```bash
DATABASE_CONNECTION_STRING=...
KEYCLOAK_URL=...
JWT_SECRET=...
```

---

## Pattern Update Process

When new patterns emerge or existing patterns need updating:

1. **Identify Pattern** - Notice repeated design decisions or conventions
2. **Document Proposal** - Write ADR explaining the pattern and rationale
3. **Review** - Architect reviews and discusses with team
4. **Approve** - Add to this document with clear rationale
5. **Apply** - Use in all new code
6. **Refactor** - Optionally update existing code to match

---

## Pattern Categories

### Must Follow (Critical)
- Authorization patterns (security-critical)
- Audit patterns (compliance-critical)
- Error handling patterns (user experience)
- Testing patterns (quality-critical)

### Should Follow (Recommended)
- API design patterns (consistency)
- Frontend patterns (maintainability)
- Data modeling patterns (integrity)
- Clean architecture patterns (maintainability)

### May Follow (Guideline)
- Code organization conventions
- Naming conventions
- Comment styles

---

## Related Documents

- `planning-mds/architecture/decisions/*.md` - ADRs explaining why patterns were chosen
- `planning-mds/INCEPTION.md` Section 4 - Architecture specifications
- `agents/*/references/*.md` - Generic best practices (not project-specific)

---

**Version:** 1.0
**Next Review:** After Phase C completion or when significant patterns emerge

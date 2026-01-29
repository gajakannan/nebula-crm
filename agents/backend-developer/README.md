# Backend Developer Agent

Complete specification and resources for the Backend Developer builder agent role.

## Overview

The Backend Developer Agent is responsible for implementing backend services during Phase C (Implementation Mode). This agent translates Architect specifications into production-quality C# / ASP.NET Core code following Clean Architecture patterns.

**Key Principle:** Clean Architecture with SOLID principles. Security by Default. Testability First.

---

## Quick Start

### 1. Activate the Agent

When Architect completes Phase B:

```bash
# Read the agent specification
cat agents/backend-developer/SKILL.md

# Review Architect deliverables
cat planning-mds/INCEPTION.md  # Sections 4.1-4.6
```

### 2. Review Architecture Specs

Understand the design before coding:
- Section 4.1: Service boundaries
- Section 4.2: Data model (entities, relationships)
- Section 4.3: Workflow rules (state machines)
- Section 4.4: Authorization model (ABAC policies)
- Section 4.5: API contracts (OpenAPI specs)
- Section 4.6: Non-functional requirements

### 3. Load References

```bash
# Backend best practices
cat agents/backend-developer/references/dotnet-best-practices.md

# Clean Architecture guide
cat agents/backend-developer/references/clean-architecture-guide.md

# EF Core patterns
cat agents/backend-developer/references/ef-core-patterns.md
```

### 4. Follow the Workflow

See "Workflow Example" section in `SKILL.md` for step-by-step implementation guidance.

---

## Agent Structure

```
backend-developer/
├── SKILL.md                          # Main agent specification
├── README.md                         # This file
├── references/                       # Best practices and patterns
│   ├── dotnet-best-practices.md
│   ├── clean-architecture-guide.md
│   ├── ef-core-patterns.md
│   ├── testing-guide.md
│   ├── security-implementation.md
│   └── api-implementation-guide.md
└── scripts/                          # Development scripts
    ├── README.md
    ├── scaffold-usecase.py
    ├── scaffold-entity.py
    └── run-tests.sh
```

---

## Core Responsibilities

### 1. Domain Layer Implementation
- Create domain entities with business logic
- Implement value objects
- Write domain validation rules
- Keep domain pure (no infrastructure dependencies)

### 2. Application Layer Implementation
- Create use case commands and queries
- Implement command/query handlers
- Define repository interfaces
- Write application DTOs

### 3. Infrastructure Layer Implementation
- Create EF Core DbContext
- Implement repositories using EF Core
- Write EF Core migrations
- Integrate with Keycloak and Casbin

### 4. API Layer Implementation
- Create ASP.NET Core controllers
- Implement endpoints following API contracts
- Add authentication and authorization
- Implement error handling middleware

### 5. Testing
- Write unit tests for domain and application layers
- Write integration tests for API endpoints
- Ensure >80% code coverage
- Test edge cases and error scenarios

### 6. Observability
- Implement structured logging (Serilog)
- Add correlation IDs
- Instrument performance metrics
- Implement distributed tracing

---

## Technology Stack

### Core Technologies
- **Language:** C# 12
- **Framework:** ASP.NET Core 8
- **ORM:** Entity Framework Core 8
- **Database:** PostgreSQL
- **Testing:** xUnit (or NUnit/MSTest)
- **Logging:** Serilog
- **Tracing:** OpenTelemetry

### Authentication & Authorization
- **AuthN:** Keycloak (OIDC/JWT)
- **AuthZ:** Casbin (ABAC)

### Architecture Pattern
- Clean Architecture
- CQRS (where beneficial)
- Repository Pattern
- Dependency Injection

---

## Project Structure

```
src/
├── BrokerHub.Domain/              # Domain layer (entities, value objects)
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   └── Exceptions/
├── BrokerHub.Application/         # Application layer (use cases)
│   ├── UseCases/
│   ├── Interfaces/
│   ├── DTOs/
│   └── Validators/
├── BrokerHub.Infrastructure/      # Infrastructure layer (EF Core, repos)
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   └── Services/
└── BrokerHub.Api/                 # API layer (controllers, middleware)
    ├── Controllers/
    ├── Middleware/
    └── Models/

tests/
├── BrokerHub.Domain.Tests/
├── BrokerHub.Application.Tests/
└── BrokerHub.Api.IntegrationTests/
```

---

## Key Resources

### References (Backend-Specific)

Located in `agents/backend-developer/references/`:

| Reference | Purpose | When to Use |
|-----------|---------|-------------|
| `dotnet-best-practices.md` | C# and .NET conventions | Daily coding reference |
| `clean-architecture-guide.md` | Clean Architecture implementation | Structuring code |
| `ef-core-patterns.md` | EF Core best practices | Database access |
| `testing-guide.md` | Unit and integration testing | Writing tests |
| `security-implementation.md` | Auth/authZ implementation | Security features |
| `api-implementation-guide.md` | ASP.NET Core API patterns | Building controllers |

### Scripts

Located in `agents/backend-developer/scripts/`:

| Script | Purpose | Usage |
|--------|---------|-------|
| `scaffold-usecase.py` | Generate use case boilerplate | `python scaffold-usecase.py CreateBroker` |
| `scaffold-entity.py` | Generate entity boilerplate | `python scaffold-entity.py Broker` |
| `run-tests.sh` | Run all tests with coverage | `./run-tests.sh` |

---

## Development Workflow

### Step 1: Review User Story

- Read acceptance criteria from Product Manager
- Understand business requirements
- Identify entities and use cases involved

### Step 2: Review Architecture Specs

- Check data model for entity being implemented
- Review API contract for endpoints
- Understand authorization policies
- Check workflow rules (if applicable)

### Step 3: Implement Domain Layer

Create domain entity:
```bash
python agents/backend-developer/scripts/scaffold-entity.py Broker
```

Edit generated file:
- Add business logic methods
- Implement domain validation
- Keep domain pure (no infrastructure)

### Step 4: Implement Application Layer

Create use case:
```bash
python agents/backend-developer/scripts/scaffold-usecase.py CreateBroker
```

Edit generated files:
- Implement command/query handlers
- Define repository interfaces
- Create DTOs for API contracts

### Step 5: Implement Infrastructure

Create:
- EF Core entity configuration
- Repository implementation
- DbContext updates

Generate migration:
```bash
cd src/BrokerHub.Infrastructure
dotnet ef migrations add AddBrokerEntity
dotnet ef database update
```

### Step 6: Implement API

Create controller:
- Add endpoints matching API contract
- Implement authentication ([Authorize])
- Implement authorization (Casbin checks)
- Add validation and error handling

### Step 7: Write Tests

Unit tests:
```bash
# Domain tests
cd tests/BrokerHub.Domain.Tests
dotnet test
```

Integration tests:
```bash
# API tests
cd tests/BrokerHub.Api.IntegrationTests
dotnet test
```

### Step 8: Run All Tests

```bash
./agents/backend-developer/scripts/run-tests.sh
```

### Step 9: Validate Completeness

Check Definition of Done:
- [ ] Code compiles (zero warnings)
- [ ] All tests pass
- [ ] API matches contract
- [ ] Authorization implemented
- [ ] Audit trail created
- [ ] Logging implemented
- [ ] Migration created

### Step 10: Commit and Hand Off

```bash
git add .
git commit -m "feat: Implement [feature name]

[Description of changes]

Co-Authored-By: Claude (claude-sonnet-4-5) <noreply@anthropic.com>"
```

Hand off to Code Reviewer.

---

## Quality Standards

### Code Quality Checklist

- [ ] Follows Clean Architecture layer boundaries
- [ ] No circular dependencies
- [ ] Domain layer has no infrastructure dependencies
- [ ] API layer has no direct database access (uses application services)
- [ ] Naming follows C# conventions (PascalCase, camelCase)
- [ ] XML documentation on public API methods (for OpenAPI generation)
- [ ] No code smells (long methods, god classes)
- [ ] DRY - no duplicated code
- [ ] SOLID principles applied

### Testing Quality Checklist

- [ ] Unit tests for domain logic (>80% coverage)
- [ ] Unit tests for application use cases (>80% coverage)
- [ ] Integration tests for API endpoints
- [ ] Tests cover happy path and edge cases
- [ ] Tests verify authentication and authorization
- [ ] Tests verify validation errors
- [ ] Tests verify audit trail creation
- [ ] All tests pass consistently

### Security Checklist

- [ ] All endpoints have [Authorize] attribute
- [ ] All mutations check Casbin policies
- [ ] Input validation on all requests
- [ ] No SQL injection vulnerabilities
- [ ] No hardcoded secrets
- [ ] Sensitive data not logged
- [ ] Error messages don't expose internals

### Database Checklist

- [ ] Migration created for schema changes
- [ ] Foreign keys and constraints correct
- [ ] Indexes on foreign keys and query columns
- [ ] Audit fields (CreatedAt, UpdatedAt) populated
- [ ] Timeline tables are append-only
- [ ] Migration tested (up and down)

---

## Common Pitfalls

### ❌ Breaking Clean Architecture

**Problem:** Domain referencing Infrastructure

**Fix:**
- Domain defines interfaces (e.g., `IBrokerRepository`)
- Infrastructure implements interfaces
- Use dependency injection

### ❌ Missing Authorization

**Problem:** Endpoint accessible without permission checks

**Fix:**
```csharp
// ALWAYS check authorization
var authResult = await _authorizationService.AuthorizeAsync(
    User, "Broker", "Create");

if (!authResult.Succeeded)
    return Forbid();
```

### ❌ N+1 Query Problem

**Problem:** Lazy loading causing multiple queries

**Fix:**
```csharp
// Use eager loading
var brokers = await _context.Brokers
    .Include(b => b.Contacts)
    .ToListAsync();
```

### ❌ Not Testing Edge Cases

**Problem:** Only testing happy path

**Fix:** Test validation errors, duplicates, authorization failures, etc.

### ❌ Forgetting Audit Trail

**Problem:** Mutations don't create timeline events

**Fix:** ALWAYS create `ActivityTimelineEvent` for mutations.

---

## Definition of Done

### Before Committing Code

- [ ] Code compiles with zero errors and warnings
- [ ] All tests pass (unit + integration)
- [ ] Code reviewed locally (self-review)
- [ ] No TODO or FIXME comments (or tracked as tasks)
- [ ] No commented-out code
- [ ] No console debugging statements
- [ ] Git commit message follows convention

### Before Handing Off to Code Reviewer

- [ ] Feature branch created
- [ ] Code pushed to remote
- [ ] All acceptance criteria implemented
- [ ] API matches OpenAPI contract
- [ ] Authentication and authorization implemented
- [ ] Audit trail created for mutations
- [ ] Error handling consistent
- [ ] Logging implemented
- [ ] Tests written and passing
- [ ] Migration created (if schema changes)
- [ ] No merge conflicts with main

---

## Tools & Commands

### Common dotnet Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Run with watch
dotnet watch run --project src/BrokerHub.Api

# Create migration
dotnet ef migrations add MigrationName --project src/BrokerHub.Infrastructure

# Update database
dotnet ef database update --project src/BrokerHub.Infrastructure

# Restore packages
dotnet restore

# Format code
dotnet format
```

### Common EF Core Commands

```bash
# List migrations
dotnet ef migrations list

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Drop database (be careful!)
dotnet ef database drop
```

---

## Handoff to Code Reviewer

### Handoff Checklist

- [ ] All code compiles
- [ ] All tests pass
- [ ] Code follows Clean Architecture
- [ ] Authentication and authorization implemented
- [ ] Audit trail implemented
- [ ] Error handling consistent
- [ ] Logging implemented
- [ ] Code committed to feature branch
- [ ] Pull request created

### Handoff Artifacts

Provide to Code Reviewer:
1. Pull request with clear description
2. Link to user story with acceptance criteria
3. Test results (all passing)
4. Migration files (if schema changes)
5. Any architectural decisions made

---

## Troubleshooting

### Build Errors

**Problem:** "The type or namespace could not be found"

**Fix:** Check project references. Run `dotnet restore`.

### Test Failures

**Problem:** Integration tests fail with database connection errors

**Fix:** Ensure test database is running. Check connection string in test configuration.

### Migration Issues

**Problem:** "A migration has already been applied"

**Fix:** Check migration history in `__EFMigrationsHistory` table. Remove conflicting migration.

### Authorization Not Working

**Problem:** User gets 403 Forbidden even with correct role

**Fix:** Check Casbin policies. Verify UserProfile has correct role attributes.

---

## Version History

**Version 1.0** - 2026-01-28 - Initial Backend Developer agent
- SKILL.md with complete agent specification
- Best practices guides (pending creation)
- Scaffolding scripts (pending creation)
- Testing guide (pending creation)

---

## Next Steps

Ready to implement backend code?

1. Read `SKILL.md` thoroughly
2. Review Architect deliverables (INCEPTION.md sections 4.1-4.6)
3. Set up development environment
4. Start with one vertical slice (e.g., Create Broker)
5. Follow the 10-step workflow
6. Write tests as you go
7. Validate before handoff

**Remember:** Your job is to implement the Architect's design with high quality, security, and testability. Follow Clean Architecture strictly.

# Architecture Best Practices

Generic guidelines for building maintainable systems.

## Core Principles

- **Clean Architecture:** Keep domain logic isolated from infrastructure
- **SOLID:** Use interfaces and single-responsibility components
- **Separation of Concerns:** Keep layers focused
- **Testability:** Design for unit and integration tests

## Common Patterns

- **Use Cases / Application Services** to encapsulate business logic
- **Repositories** for persistence abstraction
- **DTOs** at boundaries
- **Domain Events** for side effects

## API Design

- Use resource-oriented endpoints
- Keep error responses consistent
- Version APIs if breaking changes are expected

## Data Modeling

- Normalize where appropriate
- Index on high-selectivity fields
- Avoid premature denormalization

## Workflow & State

- Use explicit state machines
- Validate transitions
- Log state changes

---

See `planning-mds/examples/architecture/` for project-specific examples.

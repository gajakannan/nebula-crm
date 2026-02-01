# Data Modeling Guide

Generic guidance for designing relational data models.

## Core Principles

- Normalize where appropriate
- Use clear primary keys (UUIDs or sequences)
- Prefer explicit foreign keys
- Keep audit fields consistent

## Common Patterns

- One-to-many relationships via FK
- Many-to-many relationships via join table
- Soft delete with `deletedAt` field (if needed)

## Indexing

- Index fields used for lookups and filtering
- Avoid over-indexing early

## Migrations

- Use incremental migrations
- Keep schema changes backward compatible when possible

---

See `planning-mds/examples/` for project-specific data models.

# Vertical Slicing Guide

Generic guide to breaking features into thin, end-to-end slices.

## What is a Vertical Slice?

A slice delivers user value across layers (UI → API → DB) and is testable end-to-end.

## Why It Matters

- Delivers value incrementally
- Reduces integration risk
- Enables parallel work
- Improves estimation accuracy

## Example (Domain-Neutral)

**Feature:** Record Management

**Slice 1:** View record list (UI list → API list → DB query)

**Slice 2:** Create record (UI form → API create → DB insert)

**Slice 3:** View record detail (UI detail → API get by id → DB read)

**Slice 4:** Update record (UI edit → API update → DB update)

**Slice 5:** Delete record (UI confirm → API delete → DB soft delete)

## Common Anti-Patterns

- Building all UI first, then all APIs
- Building all database schema before any user-visible value

## Tips

- Start with a small, valuable happy path
- Add validation and edge cases in later slices
- Prefer slices that are independently shippable

---

See `planning-mds/examples/` for project-specific slicing examples.

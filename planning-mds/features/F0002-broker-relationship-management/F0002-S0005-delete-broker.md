# F0002-S0005: Delete Broker

**Story ID:** F0002-S0005
**Feature:** F0002 — Broker & MGA Relationship Management
**Title:** Deactivate (soft delete) a broker
**Priority:** Medium
**Phase:** MVP

## User Story

**As a** Distribution User
**I want** to deactivate a broker record
**So that** inactive or erroneous records are removed from active workflows

## Context & Background

Brokers occasionally need to be removed from active use due to errors, inactivity, or compliance issues. Deactivation should be reversible (soft delete) and fully audited.

## Acceptance Criteria

- **Given** I have `broker:delete` permission
- **When** I confirm broker deletion
- **Then** the broker is soft deleted and no longer appears in active broker lists or search results

- **Given** a broker has been soft deleted
- **When** I attempt to access Broker 360
- **Then** I see a "Broker not found" message

- **Given** I am not authorized to delete brokers
- **When** I attempt to delete
- **Then** access is denied with a 403 response

- **Given** a broker is deleted successfully
- **When** deletion completes
- **Then** an audit timeline event is stored with actor, timestamp, and broker id

- **Given** the broker has active submissions or renewals
- **When** I attempt to delete the broker
- **Then** the delete is rejected with a deterministic conflict error and the broker remains active

- Edge case: deleting a broker that does not exist → return not found

## Data Requirements

**Required Fields:**
- BrokerId: identifier for the broker to delete

**Validation Rules:**
- Delete is soft delete (record retained, flagged as inactive/deleted)
- Broker cannot be deleted while active submissions or renewals exist

## Role-Based Visibility

**Roles that can delete brokers:**
- DistributionUser — scoped delete
- DistributionManager — region-scoped delete
- Admin — unscoped delete
  
**Explicitly not allowed in MVP:**
- RelationshipManager — no delete permission

**Data Visibility:**
- InternalOnly content only; no external access in MVP

## Non-Functional Expectations

- Security: server-side authorization required for delete paths
- Reliability: delete is idempotent and audit logged

## Dependencies

**Depends On:**
- F0002-S0001 - Create Broker
- F0002-S0003 - Read Broker (Broker 360 View)

**Related Stories:**
- F0002-S0007 - View Broker Activity Timeline

## Out of Scope

- Hard delete (permanent removal)
- Bulk deactivation

## Questions & Assumptions

**Assumptions (to be validated):**
- Soft delete hides brokers from list/search but retains audit history

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Edge cases handled
- [ ] Permissions enforced
- [ ] Audit/timeline logged for deletion
- [ ] Tests pass

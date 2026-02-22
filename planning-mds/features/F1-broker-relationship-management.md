# Feature: Broker & MGA Relationship Management

**Feature ID:** F1
**Feature Name:** Broker & MGA Relationship Management
**Priority:** Critical
**Phase:** MVP

## Feature Statement

**As a** Distribution and Relationship team member
**I want** a unified broker/MGA management workspace
**So that** I can manage records, contacts, and activity context without fragmented tools

## Business Objective

- **Goal:** Establish a reliable broker relationship source of truth for intake and collaboration workflows.
- **Metric:** Time to create and locate broker records; data completeness for broker profiles.
- **Baseline:** Spreadsheet-based tracking with inconsistent structure and low traceability.
- **Target:** Create/search broker workflows completed within target latency and with audit traces.

## Problem Statement

- **Current State:** Broker and MGA relationship information is spread across spreadsheets and messages.
- **Desired State:** Structured broker profiles, contact data, and timeline history in one system.
- **Impact:** Improves intake speed, handoff quality, and operational accountability.

## Scope & Boundaries

**In Scope:**
- Broker creation, retrieval, update, and delete lifecycle (soft delete where applicable).
- Contact association and broker search by name/license.
- Timeline events for all broker mutations.

**Out of Scope:**
- External broker portal access.
- Advanced analytics and scoring models.

## Success Criteria

- Broker create/search workflows are available with role-aware access control.
- Every broker mutation produces immutable timeline records.
- Core broker data quality checks (required fields, uniqueness, format validation) are enforced.

## Risks & Assumptions

- **Risk:** Inconsistent legacy broker data quality during migration/adoption.
- **Assumption:** License number uniqueness is sufficient for MVP deduplication.
- **Mitigation:** Enforce strong validation and deterministic conflict responses.

## Dependencies

- Keycloak authentication and Casbin ABAC enforcement.
- Broker, Contact, and timeline data model baseline.

## Related User Stories

- S1 - Create Broker
- S2 - Search Brokers
- S3 - Read Broker (Broker 360 View)
- S4 - Update Broker
- S5 - Delete Broker
- S6 - Manage Broker Contacts
- S7 - View Broker Activity Timeline

## Rollout & Enablement

- Internal team onboarding for broker workflow usage.
- Runbook updates for support and access troubleshooting.

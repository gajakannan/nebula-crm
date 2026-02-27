---
template: feature
version: 1.1
applies_to: product-manager
---

# F0004: Task Center UI + Manager Assignment

**Feature ID:** F0004  
**Feature Name:** Task Center UI + Manager Assignment  
**Priority:** Medium  
**Phase:** Phase 1

## Feature Statement

**As a** Distribution Manager or Admin  
**I want** a Task Center UI with the ability to assign tasks to other users  
**So that** I can coordinate work across my team with clear ownership

## Business Objective

- **Goal:** Enable team-level task management and visibility.  
- **Metric:** Tasks assigned across users and completion rates by team.  
- **Baseline:** Task creation is self-assigned only (MVP).  
- **Target:** Managers can assign tasks to team members in-app.

## Problem Statement

- **Current State:** Tasks can be created only for self via API; there is no UI to manage tasks.  
- **Desired State:** Managers can assign tasks to others and track progress in a dedicated Task Center.  
- **Impact:** Reduced coordination gaps and improved follow-through.

## Scope & Boundaries

**In Scope:**
- Task Center UI (list, filters, detail view)
- Assign tasks to other internal users (manager/admin only)
- Cross-user task visibility with ABAC scope
- Bulk actions (optional if time permits)

**Out of Scope:**
- External user task visibility
- Automated task creation rules engine
- Notification/alert system

## Success Criteria

- Managers can assign tasks to others within their authorized scope.
- Task Center UI supports search/filter and status updates.
- Audit timeline captures assignment changes.

## Risks & Assumptions

- **Risk:** Cross-user visibility could weaken ABAC if scoping is unclear.  
- **Assumption:** Team/region/program scoping is defined before implementation.  
- **Mitigation:** Require explicit scope rules before enabling assignment.

## Dependencies

- ABAC scope model (regions/programs/ownership)
- Task CRUD endpoints (F0003) in place
- User directory lookup (Keycloak subject + profile)

## Related User Stories

- (To be defined)

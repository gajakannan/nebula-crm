# Authorization Review

Status: Final
Last Updated: 2026-02-17
Owner: Security + Architect

## Objective

Define the authorization model evidence required before implementation and confirm the baseline ABAC policy scope.

## Authorization Model Summary

- Model: Casbin ABAC with deny-by-default policies.
- Enforcement points: API endpoints, workflow transitions, and data access queries.
- Decision context: subject attributes + resource attributes + action.

## Resource-Action Matrix (Baseline)

| Resource | Actions | Subject Scope | Notes |
|---|---|---|---|
| Broker | create/read/update/delete | Internal roles only | Restrict by region/department where applicable |
| Account | create/read/update/delete | Internal roles only | Account ownership and broker scope apply |
| Contact | create/read/update/delete | Internal roles only | Must be tied to authorized broker/account |
| Submission | read/transition | Underwriter + internal roles | Transition actions limited by workflow state |
| Renewal | read/transition | Underwriter + internal roles | Transition actions limited by workflow state |
| Task | read/update | Assigned user | No cross-user access |
| TimelineEvent | read | Authorized subjects | Filtered by ABAC scope on entity |
| Dashboard | read | Authenticated internal users | ABAC scope applied per widget |

## Policy Test Catalog (Minimum)

Allow cases:
- Internal role with broker:create can create a broker.
- Underwriter can transition a submission from InReview -> Quoted.
- User can read tasks assigned to their subject id.

Deny cases:
- User without broker:create cannot create a broker (403).
- User cannot read broker outside their ABAC scope (403).
- Underwriter cannot transition a submission to invalid next state (409).

## Story Cross-Reference Requirements

- Each story with protected resources must define at least one authorization acceptance criterion.
- Each protected API endpoint must map to a policy test case.

## Implementation Artifacts Required

- Casbin model and policy files in application runtime (model.conf, policy.csv or equivalent).
- Automated policy tests covering allow/deny for all protected endpoints.
- Runtime enforcement in middleware and repository-level scope filters.

## Sign-Off

Security Reviewer: Pending
Architect: Pending
Date: Pending

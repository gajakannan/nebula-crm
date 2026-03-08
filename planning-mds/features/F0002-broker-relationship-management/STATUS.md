# F0002 — Broker & MGA Relationship Management — Status

**Overall Status:** In Progress
**Last Updated:** 2026-03-08

## Story Checklist

| Story | Title | Status | Notes |
|-------|-------|--------|-------|
| F0002-S0001 | Create Broker | Done | Casbin `broker:create` check enforced. Core flow, duplicate license, timeline event complete. |
| F0002-S0002 | Search Brokers | Done | Casbin `broker:search` check enforced. Pagination, filters, empty state complete. |
| F0002-S0003 | Read Broker (Broker 360 View) | Done | Casbin `broker:read` check enforced. Contacts tab now consumes paginated envelope. Timeline tab now paginated. |
| F0002-S0004 | Update Broker | Done | Casbin `broker:update` check enforced. Optimistic concurrency complete. |
| F0002-S0005 | Delete Broker (Deactivate) | Done | Casbin `broker:delete` check enforced. Deactivation now sets `Status=Inactive` alongside `IsDeleted=true`. PII masking works correctly on deactivated brokers. |
| F0002-S0006 | Manage Broker Contacts | Done | Casbin `contact:create|read|update|delete` checks enforced. `ContactDto` now exposes `RowVersion`. Frontend hook consumes paginated envelope; update flow sends `If-Match`. |
| F0002-S0007 | View Broker Activity Timeline | Done | Casbin `timeline_event:read` check enforced. Paginated response (`page`, `pageSize`, `totalCount`, `totalPages`) implemented in backend and consumed by Broker 360 Timeline tab. "Unknown User" actor fallback applied via `MapToDto`. |
| F0002-S0008 | Reactivate Broker | Done | Casbin `broker:reactivate` check enforced. OpenAPI path `/brokers/{brokerId}/reactivate` added to spec. Integration tests added. |
| F0002-S0009 | Adopt Native Casbin Enforcer | Planned | Current authorization uses hand-rolled policy parser/evaluator. Replace with native Casbin enforcer per ADR-008. |

## Resolved Gaps (2026-03-08)

1. **Casbin policy enforcement** — All broker/contact/timeline endpoints now call `HasAccessAsync` with the correct resource+action per `policy.csv`. BrokerUser paths bypass Casbin (scope-isolated by F0009 logic).
2. **Deactivation sets Status=Inactive** — `BrokerService.DeleteAsync` now sets `broker.Status = "Inactive"` alongside `IsDeleted = true`. PII masking (`MaskPii`) now triggers correctly on deactivation.
3. **Contact API/UI contract** — `ContactDto` now includes `RowVersion uint`. `useBrokerContacts` hook types response as `PaginatedResponse<ContactDto>`. `BrokerContactsTab` extracts `.data`. `ContactFormModal` passes `rowVersion` to update mutation.
4. **Timeline pagination** — `ITimelineRepository` has new `ListEventsPagedAsync`. `TimelineService` has `ListEventsPagedAsync`. `TimelineEndpoints` returns `{ data, page, pageSize, totalCount, totalPages }`. `useBrokerTimeline` and `BrokerTimelineTab` support page navigation.
5. **OpenAPI reactivate path** — `POST /brokers/{brokerId}/reactivate` path added to `nebula-api.yaml` with correct responses (200, 403, 404, 409).
6. **Tests** — Added: `BrokerAuthorizationTests` (10 Casbin 403 tests), `TimelineEndpointTests` (3 pagination tests), reactivation tests in `BrokerEndpointTests` (3 tests), contact paginated envelope + RowVersion tests in `ContactEndpointTests` (2 tests).

## Open Items / Follow-ups

- UI-level action hiding (edit/deactivate/delete buttons hidden for unauthorized roles) deferred — requires frontend auth context integration; backend 403 responses prevent unauthorized mutations regardless.
- Cross-broker ownership validation in contact service (contacts created with mismatched brokerId) deferred — existing validator checks broker existence but not requester scope boundary; scoped to future hardening sprint.
- Integration test WSL environment limitation — `WebApplicationFactory` path resolution fails in `/mnt/c/` WSL paths; tests must be run from Windows or in a container. No C# compiler errors in test code.
- Native Casbin adoption remains pending — current implementation still uses custom parser/evaluator logic in `PolicyAuthorizationService`; see F0002-S0009 and ADR-008.

## Resolved (F0009 Complete)

- BrokerUser tenant isolation and field-boundary filtering are implemented in F0009-S0004 (scope resolution + audience-specific DTOs).
- BrokerUser timeline event filtering to approved event types with `BrokerDescription` is implemented.

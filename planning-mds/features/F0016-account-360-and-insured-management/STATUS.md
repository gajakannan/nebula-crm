# F0016 — Account 360 & Insured Management — Status

**Overall Status:** In Refinement
**Last Updated:** 2026-04-13

## Story Checklist

| Story | Title | Status |
|-------|-------|--------|
| F0016-S0001 | Account list with search and filtering | Draft |
| F0016-S0002 | Create account (manual and from submission / policy) | Draft |
| F0016-S0003 | Account detail and profile edit | Draft |
| F0016-S0004 | Account 360 composed workspace | Draft |
| F0016-S0005 | Account-scoped contacts management | Draft |
| F0016-S0006 | Account relationships (broker / producer / territory) | Draft |
| F0016-S0007 | Account lifecycle (deactivate / reactivate / delete) | Draft |
| F0016-S0008 | Account merge and duplicate handling | Draft |
| F0016-S0009 | Deleted / merged account fallback contract | Draft |
| F0016-S0010 | Account activity timeline and audit trail | Draft |
| F0016-S0011 | Account summary projection | Draft |

## Refinement Guardrails

- F0016 owns the Deleted / Merged Account Fallback Contract (descoped from F0006 at archive time). Every dependent feature (F0006, F0007, F0018 when it lands) must adopt it.
- Any F0016 account lifecycle story must define how linked submissions, policies, renewals, activity timelines, and search results render when the account is deactivated, merged, or deleted.
- Merge is synchronous and size-gated in MVP (≤ 500 linked records). Async / Temporal-backed merge for larger accounts is a deliberate Future follow-up.
- Unmerge / undelete recovery flows are explicitly out of MVP.
- Policy data today is the F0007-seeded Policy stub. F0016 does not block on F0018.

## Required Signoff Roles (Set in Planning)

| Role | Required | Why Required | Set By | Date |
|------|----------|--------------|--------|------|
| Quality Engineer | Yes | Workflow transition matrix, merge semantics, fallback-contract integration tests, 360 rail isolation, ABAC test coverage. | Architect | TBD (Phase B) |
| Code Reviewer | Yes | Entity modeling, Account 360 composition, merge transaction atomicity, fallback-contract adoption across dependent modules. | Architect | TBD (Phase B) |
| Security Reviewer | Yes | Cross-role visibility on a hub entity, new Casbin `account:*` actions, merge and delete authority, tombstone-forward access semantics. | Architect | TBD (Phase B) |
| DevOps | Yes | New Accounts / AccountContacts / AccountRelationshipHistory migrations, index changes, denormalized-column backfill on existing submissions / renewals, rollback paths. | Architect | TBD (Phase B) |
| Architect | Yes | ADR-015 (Proposed) — Account merge, tombstone semantics, and dependent-view fallback; cross-module contract authority; 360 composition patterns. | Architect | TBD (Phase B) |

## Story Signoff Provenance

| Story | Role | Reviewer | Verdict | Evidence | Date | Notes |
|-------|------|----------|---------|----------|------|-------|
| F0016-S0001 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0001 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0001 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0001 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0001 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0002 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0002 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0002 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0002 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0002 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0003 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0003 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0003 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0003 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0003 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0004 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0004 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0004 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0004 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0004 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0005 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0005 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0005 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0005 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0005 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0006 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0006 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0006 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0006 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0006 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0007 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0007 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0007 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0007 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0007 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0008 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0008 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0008 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0008 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0008 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0009 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0009 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0009 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0009 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0009 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0010 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0010 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0010 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0010 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0010 | Architect | - | N/A | - | - | Populate during implementation. |
| F0016-S0011 | Quality Engineer | - | N/A | - | - | Populate during implementation. |
| F0016-S0011 | Code Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0011 | Security Reviewer | - | N/A | - | - | Populate during implementation. |
| F0016-S0011 | DevOps | - | N/A | - | - | Populate during implementation. |
| F0016-S0011 | Architect | - | N/A | - | - | Populate during implementation. |

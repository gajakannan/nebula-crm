# F0016 — Account 360 & Insured Management — Getting Started

## Prerequisites

- [ ] Read the release framing in [COMMERCIAL-PC-CRM-RELEASE-PLAN.md](../COMMERCIAL-PC-CRM-RELEASE-PLAN.md)
- [ ] Read [F0016 PRD](./PRD.md), including the Deleted / Merged Account Fallback Contract section
- [ ] Review current Account references in the blueprint, data model, and F0006 / F0007 archived STATUS notes
- [ ] Review F0002 Broker contract (broker-of-record FK target) and F0007-seeded Policy stub contract
- [ ] Confirm Casbin ABAC patterns used by F0002 / F0007 before extending with `account:*` rules
- [ ] Refresh the KG coverage with `python3 scripts/kg/lookup.py F0016` after ontology mappings land

## Refinement Walkthrough

1. Read the PRD top-to-bottom; pay close attention to:
   - The lifecycle state machine (Active, Inactive, Merged, Deleted)
   - The Deleted / Merged Account Fallback Contract — this is the owning contract descoped from F0006
   - Scope boundary table (what F0016 owns vs what it reads from F0002, F0006, F0007, F0018, F0020)
2. Read all eleven stories (`F0016-S0001` … `F0016-S0011`) before making any implementation decisions — they interlock (e.g., S0007 emits events that S0010 consumes; S0008 relies on S0009 for rendering).
3. Phase B (Architect) will add:
   - `feature-assembly-plan.md` (implementation sequencing across engine + experience + tests + migration)
   - OpenAPI fragments for every new endpoint
   - JSON Schemas for new DTOs
   - ADR-015 (Proposed) — Account merge, tombstone semantics, and dependent-view fallback contract

## How to Verify (Refinement Exit Criteria)

1. Confirm Account is modeled as a first-class aggregate with a state machine including `Merged` and `Deleted` as terminal-for-writes states.
2. Confirm the minimum related records on Account 360 (submissions, policies, renewals, contacts, activity) are enumerated and each has its own paginated rail.
3. Confirm MVP lifecycle semantics: Active ↔ Inactive, Active/Inactive → Merged (with survivor), Active/Inactive → Deleted (with reason). No unmerge / undelete in MVP.
4. Confirm the fallback contract defines, for every dependent view: (a) read-model behavior, (b) UI fallback label, (c) API semantics (410 for deleted, tombstone-forward for merged), (d) regression coverage.
5. Validate tracker sync after refinement with the PM scripts:
   - `python3 agents/product-manager/scripts/validate-stories.py planning-mds/features/F0016-account-360-and-insured-management`
   - `python3 agents/product-manager/scripts/generate-story-index.py planning-mds/features/`
   - `python3 agents/product-manager/scripts/validate-trackers.py`

## Open Follow-ups (Deferred, Tracked)

- Async / Temporal-backed merge for accounts with > 500 linked records
- Unmerge / undelete admin recovery flows
- Account summary materialized projection (if counts become a hotspot)
- Cross-account / global Contact module (when F0021 generalizes contacts)
- Territory hierarchy + rule-based auto-assignment (F0017)
- Documents rail wiring to F0020 once F0020 lands

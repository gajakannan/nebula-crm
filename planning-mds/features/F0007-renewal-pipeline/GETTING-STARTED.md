# F0007 — Renewal Pipeline — Getting Started

## Prerequisites

- [ ] Read the [PRD](./PRD.md) — scope, personas, workflow states, screens, and business rules
- [ ] Read the [COMMERCIAL-PC-CRM-RELEASE-PLAN](../COMMERCIAL-PC-CRM-RELEASE-PLAN.md) — F0007 sequencing and release context
- [ ] Review [ADR-009](../../architecture/decisions/ADR-009-lob-classification-and-sla-configuration.md) — WorkflowSlaThreshold pattern for renewal timing windows
- [ ] Review [ADR-011](../../architecture/decisions/ADR-011-crm-workflow-state-machines-and-transition-history.md) — State machine and transition history pattern
- [ ] Confirm F0018 (Policy Lifecycle) is implemented — F0007 reads policy data for renewal creation and display

## Dependencies (must be available before implementation)

| Dependency | Feature | What F0007 Needs |
|------------|---------|------------------|
| Policy entity with expiration dates | F0018 | PolicyId, ExpirationDate, EffectiveDate, Carrier, LOB, AccountId, BrokerId |
| Account entity | F0016 | AccountId, Name, Industry, PrimaryState |
| Broker entity | F0002 (done) | BrokerId, LegalName, LicenseNumber, State |
| User search API | F0004-S0002 (done) | Assignee picker for renewal ownership |
| Task linked entity | F0003/F0004 (done) | LinkedEntityType=Renewal on Task entity |
| WorkflowSlaThreshold table | ADR-009 (done) | Renewal-specific timing threshold seed data |

## Key Domain Concepts

- **Renewal workflow:** Identified → Outreach → InReview → Quoted → Completed / Lost
- **Overdue:** `current_date > (PolicyExpirationDate - LOB outreach target days)` AND `CurrentStatus = Identified`
- **Ownership handoff:** Distribution owns Identified/Outreach; underwriting owns InReview/Quoted
- **One active renewal per policy:** Enforced by unique constraint (excludes terminal + deleted)

## Seed Data Required

- WorkflowSlaThreshold entries for EntityType="renewal", Status="Identified", per-LOB outreach target and warning days
- ReferenceRenewalStatus entries for: Identified, Outreach, InReview, Quoted, Completed, Lost
- Sample policies with future expiration dates for development testing

## How to Verify

1. Create a renewal from an expiring policy → confirm status = Identified, fields inherited from policy
2. Transition through the full workflow → confirm each transition is validated and recorded
3. Assign/reassign ownership → confirm timeline records the change
4. Create a renewal past its outreach target date → confirm overdue flag appears in pipeline list
5. View dashboard → confirm renewal nudge card shows overdue/approaching counts
6. Attempt invalid transition → confirm HTTP 409 with appropriate error code
7. Attempt duplicate renewal for same policy → confirm HTTP 409
8. Run tracker validation → `python3 agents/product-manager/scripts/validate-trackers.py`

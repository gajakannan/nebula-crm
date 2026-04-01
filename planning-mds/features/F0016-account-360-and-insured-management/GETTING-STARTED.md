# F0016 — Account 360 & Insured Management — Getting Started

## Prerequisites

- [ ] Read the current release framing in [COMMERCIAL-PC-CRM-RELEASE-PLAN.md](../COMMERCIAL-PC-CRM-RELEASE-PLAN.md)
- [ ] Review current account references in the blueprint and data model
- [ ] Read F0006 closeout notes and capture that deleted-account fallback on submission detail moved here if it is descoped from F0006
- [ ] Refine this feature into stories and an implementation contract before coding

## How to Verify

1. Confirm the account is treated as a first-class insured record with a 360 view.
2. Define the minimum related records that must appear in the first release.
3. Define account lifecycle actions for MVP, including whether delete means deactivate, soft delete, merge, or reactivation-capable lifecycle behavior.
4. Verify dependent submission, policy, renewal, and timeline views have an explicit deleted/merged-account fallback contract instead of assuming a live account row always exists.
5. Validate tracker sync after refinement.

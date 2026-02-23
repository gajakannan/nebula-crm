# Nebula CRM — Error Codes (MVP)

**Purpose:** Single source of truth for ProblemDetails `code` values used in MVP.
**Scope:** F0 (Dashboard) and F1 (Broker Relationship Management).

## Usage

- Returned in RFC 7807 ProblemDetails `code` field.
- Status codes are included here for clarity; the HTTP response is authoritative.

## Codes

| Code | HTTP Status | Description | Source |
|---|---|---|---|
| `invalid_transition` | 409 | Workflow transition pair is not allowed. | `planning-mds/INCEPTION.md` |
| `missing_transition_prerequisite` | 409 | Required checklist/data missing for a transition. | `planning-mds/INCEPTION.md` |
| `active_submissions_exist` | 409 | Broker delete blocked due to active submissions/renewals. | `planning-mds/architecture/feature-assembly-plan.md` |
| `region_mismatch` | 400 | Account.Region not in BrokerRegion set on submission/renewal creation. | `planning-mds/INCEPTION.md` |
| `concurrency_conflict` | 409 | Resource was modified by another user since last read. Client should refresh and retry. | `planning-mds/architecture/SOLUTION-PATTERNS.md` |
| `duplicate_license` | 409 | Broker with the given license number already exists. Record must not be created. | F1-S1 edge case |
| `validation_error` | 400 | Request payload failed schema validation. Response includes `errors` map. | `planning-mds/architecture/SOLUTION-PATTERNS.md` §3 |

## Notes

- Add new codes here when new stories introduce deterministic error cases.
- Keep codes stable once released to avoid breaking client-side error handling.

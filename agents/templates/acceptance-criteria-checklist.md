---
template: acceptance-criteria-checklist
version: 1.1
applies_to: product-manager
---

# Acceptance Criteria Checklist

Use this checklist to validate that acceptance criteria are clear, testable, and domain-neutral.

## 1) Clarity & Testability

- [ ] Each criterion is specific and measurable
- [ ] No vague terms ("properly", "fast", "user-friendly")
- [ ] Pass/fail is unambiguous

**Bad:** The system should handle record creation properly
**Good:** Given valid input, when I submit, then a new record appears in the list

## 2) Coverage

- [ ] Happy path covered
- [ ] At least one error/edge case covered
- [ ] Permission/authorization behavior specified (if relevant)

## 3) Data Validation

- [ ] Required fields enforced
- [ ] Formats and constraints specified
- [ ] Duplicates/conflicts handled (if applicable)

## 4) Error Handling

- [ ] Error messages are actionable
- [ ] System errors have a user-safe message

## 5) Navigation & Feedback

- [ ] Post-action navigation specified
- [ ] Success/failure feedback specified

## 6) Non-Functional Criteria (If Applicable)

- [ ] Performance expectations
- [ ] Security expectations
- [ ] Reliability expectations

## 7) Audit & Timeline (If Applicable)

- [ ] Mutations specify audit/timeline requirements

## 8) Out of Scope

- [ ] Explicit non-goals listed

---

## Example Library

See `planning-mds/examples/` for project-specific acceptance criteria examples.

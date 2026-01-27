# Inception Requirements

What makes a complete Phase A (Product Manager Mode) specification.

## Purpose

This document defines the completion criteria for Phase A work. Use this as a checklist before handing off to the Architect Agent for Phase B.

---

## Phase A Deliverables

Phase A work is documented in `planning-mds/INCEPTION.md` sections 3.1 through 3.5.

### Section 3.1: Vision & Non-Goals

**Required Elements:**

✅ **Vision Statement**
- Clear, concise statement of what BrokerHub achieves
- Focuses on business outcome, not technical capabilities
- 2-3 sentences maximum
- Example: "BrokerHub enables surplus lines carriers to manage broker relationships, submissions, and renewals in a single platform, replacing spreadsheets and email-based workflows with structured data and automated reminders."

✅ **Success Criteria**
- 3-5 measurable outcomes that define success
- Examples:
  - Reduce submission processing time by 50%
  - Increase quote-to-bind ratio from 40% to 60%
  - Eliminate lost submissions (100% tracked in system)
  - Improve broker satisfaction score from 3.5 to 4.5/5

✅ **Explicit Non-Goals**
- List of 5-10 things explicitly excluded from scope
- Each with brief reason (deferred to Phase 1, not needed, etc.)
- Examples:
  - ❌ External broker/MGA portal (Phase 1)
  - ❌ Document versioning (Phase 1)
  - ❌ Advanced analytics dashboards (Future)
  - ❌ Mobile app (Future)

---

### Section 3.2: Personas

**Required: Minimum 3 Personas**

For each persona, define:

✅ **Persona Header**
- Name (e.g., "Sarah - Distribution Manager")
- Role/Title
- Priority (Primary | Secondary | Future)

✅ **Demographics & Background**
- Experience level (years in role/industry)
- Daily responsibilities (3-5 bullet points)

✅ **Goals & Motivations**
- Primary goals (what they want to achieve)
- Success metrics (how they measure success)

✅ **Pain Points**
- Current pain points (3-5 specific frustrations)
- Impact and frequency of each pain point

✅ **Jobs-to-be-Done**
- 3-5 jobs in format: "When [situation], I want to [action], so I can [outcome]"

**Minimum Personas for BrokerHub MVP:**
1. Distribution & Marketing Manager (Primary)
2. Underwriter (Primary)
3. Relationship Manager (Secondary)

**Optional Personas:**
4. Program Manager
5. Admin

---

### Section 3.3: Epics

**Required: All Core Epics Defined**

For each epic:

✅ **Epic Header**
- Epic ID (E1, E2, etc.)
- Epic Name
- Priority

✅ **Epic Statement**
- As a [persona]
- I want [high-level capability]
- So that [business value]

✅ **Business Objective**
- Clear statement of business goal
- Success metrics (2-3 measurable outcomes)

✅ **Scope & Boundaries**
- In scope: High-level capabilities (3-5 bullet points)
- Out of scope: Explicit exclusions with reasons

**Minimum Epics for BrokerHub MVP:**
1. E1: Broker & MGA Relationship Management
2. E2: Account 360 & Activity Timeline
3. E3: Submission Intake Workflow
4. E4: Renewal Pipeline
5. E5: Task Center + Reminders (optional MVP)

---

### Section 3.4: MVP User Stories

**Required: One Complete Vertical Slice (Minimum)**

For MVP, define at least one complete vertical slice with all CRUD operations.

**Minimum Requirement: Broker Vertical Slice**
- S1: Create broker with basic information ✅
- S2: View broker list with search/filtering ✅
- S3: View broker 360 detail screen ✅
- S4: Update broker information ✅
- S5: Delete broker (soft delete) ✅
- S6: View broker timeline events ✅

**Each Story Must Include:**

✅ **Story Header**
- Story ID, Epic, Title, Priority, Phase

✅ **User Story**
- As a [specific persona]
- I want [capability]
- So that [business value]

✅ **Acceptance Criteria**
- Happy path (Given/When/Then format)
- Edge cases and error scenarios
- Permission checks
- Audit trail requirements (if mutation)

✅ **Data Requirements**
- Required fields
- Optional fields
- Validation rules

✅ **Out of Scope**
- Explicit exclusions for this story

✅ **Definition of Done**
- Testable completion criteria

**Quality Bar:**
- Stories follow INVEST criteria
- Acceptance criteria are testable and complete
- No invented business rules (all rules are validated)

---

### Section 3.5: Screen Specifications

**Required: Minimum Screens for MVP**

For each screen:

✅ **Screen Header**
- Screen ID, Name, Type, Route

✅ **Purpose & Context**
- What is this screen for?
- Which personas use it?

✅ **Data Fields**
- Fields displayed on screen
- Field formats and validation

✅ **User Actions**
- Primary and secondary actions
- Permission requirements for each action

✅ **Navigation & Flow**
- Entry points (how users get here)
- Exit points (where they go from here)

**Minimum Screens for BrokerHub MVP:**
1. Navigation Shell (top nav, side nav) ✅
2. Broker List (searchable table) ✅
3. Broker 360 (detail view with timeline) ✅
4. Create/Edit Broker Form ✅
5. Dashboard (optional MVP)

---

## Phase A Completion Checklist

Before handing off to Architect (Phase B), verify:

### Documentation Completeness

- [ ] All sections 3.1-3.5 in INCEPTION.md are complete
- [ ] No TODO or TBD placeholders remain
- [ ] All personas have detailed profiles
- [ ] All epics have clear business objectives
- [ ] At least one vertical slice of stories is complete
- [ ] All MVP screens are specified

### Quality Standards

- [ ] Vision statement is clear and business-focused
- [ ] Non-goals are explicit (at least 5 items)
- [ ] Personas are based on real user needs (not fictional)
- [ ] Stories follow INVEST criteria
- [ ] Acceptance criteria are testable
- [ ] No invented business rules (all validated with stakeholder)
- [ ] Audit trail requirements are documented for mutations
- [ ] Permission requirements are specified for all actions

### Validation

- [ ] Product Owner has reviewed and approved
- [ ] Domain expert has validated business rules
- [ ] No open questions remain (all answered or explicitly marked)
- [ ] Stakeholders agree on MVP scope

### Handoff Preparation

- [ ] Architect has been notified that Phase A is complete
- [ ] Summary of deliverables has been provided
- [ ] Product Manager is available for clarification questions
- [ ] INCEPTION.md is committed to git repository

---

## Common Completion Pitfalls

### ❌ Incomplete Acceptance Criteria
**Problem:** Stories have acceptance criteria but miss edge cases, errors, or permissions
**Fix:** Use `agents/templates/acceptance-criteria-checklist.md` to validate

### ❌ Technical Solutions in Stories
**Problem:** Stories specify implementation details (APIs, database, tech stack)
**Fix:** Focus on user-observable behavior, defer technical decisions to Architect

### ❌ Invented Business Rules
**Problem:** PM assumed business rules without validation
**Fix:** Review all stories for assumptions, validate with stakeholders

### ❌ Vague Screen Specifications
**Problem:** Screens described in general terms without specific fields or actions
**Fix:** Use `agents/templates/screen-spec-template.md` for detailed specifications

### ❌ Missing Non-Goals
**Problem:** Only documented what's included, not what's excluded
**Fix:** Explicitly list 5-10 non-goals with reasons

---

## Phase A to Phase B Handoff

### Handoff Meeting Agenda

1. **Vision Review** (5 min)
   - Present vision statement and success criteria
   - Confirm alignment with business strategy

2. **Persona Walkthrough** (10 min)
   - Review each primary persona
   - Highlight key pain points and jobs-to-be-done

3. **Epic Overview** (10 min)
   - Present each epic with business objectives
   - Confirm MVP vs Phase 1 prioritization

4. **Story Deep Dive** (20 min)
   - Walk through the complete vertical slice (Broker CRUD)
   - Highlight audit trail and permission requirements
   - Clarify any complex acceptance criteria

5. **Screen Specifications** (10 min)
   - Review key screens (Broker List, Broker 360)
   - Discuss navigation and user workflows

6. **Q&A** (15 min)
   - Answer Architect's clarification questions
   - Document any open items for follow-up

### Handoff Artifacts

Provide Architect with:
- ✅ `planning-mds/INCEPTION.md` (sections 3.1-3.5 complete)
- ✅ Link to all templates used (story, persona, epic, screen)
- ✅ Domain glossary (`agents/product-manager/references/insurance-domain-glossary.md`)
- ✅ Contact info for follow-up questions

---

## Measuring Phase A Quality

### Quality Metrics

**Coverage:**
- Number of personas defined: Target ≥ 3
- Number of epics defined: Target ≥ 5
- Number of MVP stories: Target ≥ 20
- Number of screens specified: Target ≥ 5

**Completeness:**
- Stories with complete acceptance criteria: Target 100%
- Stories with audit trail requirements (if applicable): Target 100%
- Stories with permission checks: Target 100%
- Screens with data fields specified: Target 100%

**Quality:**
- Stories passing INVEST criteria: Target ≥ 90%
- Stories with no invented business rules: Target 100%
- Stories with edge cases documented: Target ≥ 80%

### Quality Review

Run validation before handoff:
```bash
# Validate all stories
for story in planning-mds/stories/*.md; do
    python agents/product-manager/scripts/validate-stories.py "$story"
done

# Generate index
python agents/product-manager/scripts/generate-story-index.py planning-mds/stories/
```

---

## Version History

**Version 1.0** - 2026-01-26 - Initial inception requirements

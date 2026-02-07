# Validation Enhancements Added

**Date:** 2026-02-01
**Based on:** GitHub Spec Kit learnings

---

## ✅ 1. Clarification Step Added to plan.md

**Location:** `agents/actions/plan.md` - Step 1.5 (new step between PM and Approval)

**Purpose:** Catch underspecified requirements BEFORE architecture design

### What It Does:

1. **Reviews Phase A for vagueness:**
   - Ambiguous language ("should", "might", "easy", "fast")
   - Missing edge cases
   - Undefined dependencies
   - Unstated assumptions

2. **Creates clarification questions:**
   ```
   Story: "Customer search should be fast"
   Question: How fast? (< 200ms? < 1s? < 5s?)

   Story: "Users can upload documents"
   Questions:
   - What file types? (PDF, images, Office?)
   - Max file size? (1MB? 10MB?)
   - What if upload fails?
   ```

3. **Updates requirements with specifics:**
   - Quantified acceptance criteria
   - Explicit edge cases
   - Documented assumptions

4. **Validates testability:**
   - All criteria measurable
   - No ambiguous words
   - Error scenarios specified

### Anti-Patterns Caught:

**Banned Words:**
- ❌ "should", "might", "probably", "usually"
- ❌ "easy", "simple", "intuitive", "user-friendly"
- ❌ "fast", "quick", "slow", "performant"
- ❌ "secure", "safe" (without specifics)
- ❌ "scalable", "flexible", "robust" (without metrics)

**Replacements:**
- ✅ "must" (requirement), "may" (optional)
- ✅ "< 200ms p95" instead of "fast"
- ✅ "JWT tokens, HTTPS, 30min timeout" instead of "secure"
- ✅ "support 10,000 users" instead of "scalable"

---

## ✅ 2. Enhanced Validation Checklists (validate.md)

**Location:** `agents/actions/validate.md` - Enhanced Agent Responsibilities sections

### Product Manager Validation - Enhanced

**Added Testability Checklist (per story):**
```markdown
- [ ] Has "As a / I want / So that" structure
- [ ] Acceptance criteria specific and measurable
- [ ] No banned words: "should", "might", "easy", "fast"
- [ ] Performance criteria quantified (< Xms, not "fast")
- [ ] Error scenarios specified ("if X fails, then Y")
- [ ] Edge cases identified (empty lists, max values, nulls)
- [ ] Dependencies documented
```

**Added Anti-Pattern Detection:**

Examples of issues to flag:
```
❌ "System should be fast"
✅ "API responses < 200ms p95"

❌ "Users can upload files"
✅ "Users can upload PDF/PNG (max 10MB)"

❌ "Secure authentication"
✅ "JWT tokens, HTTPS only, session timeout 30min"

❌ "Easy to use interface"
✅ "3-click maximum to create customer"

❌ "Dashboard is intuitive"
✅ "Dashboard shows: revenue chart, top 5 customers, recent submissions"
```

### Architect Validation - Enhanced

**Added Cross-Mapping Checks:**
```markdown
- [ ] Every user story has required API endpoints
- [ ] Every endpoint supports user stories
- [ ] Request/response schemas defined
- [ ] Error responses documented
```

**Added Endpoint Completeness:**
```markdown
- [ ] CRUD operations complete (where needed)
- [ ] Search/filter endpoints specified
- [ ] Pagination parameters defined
- [ ] Sorting options documented
```

**Added NFR Quantification Check:**
```markdown
- [ ] Performance: specific targets (< 200ms p95, not "fast")
- [ ] Scalability: numbers (10,000 users, not "many")
- [ ] Availability: percentage (99.9%, not "always up")
- [ ] Security: standards (OWASP Top 10, not "secure")
- [ ] Compliance: regulations (GDPR, SOC2, not "compliant")
```

**Added SOLUTION-PATTERNS.md Compliance:**
```markdown
- [ ] Authorization follows Casbin ABAC pattern
- [ ] All mutations create ActivityTimelineEvent
- [ ] API endpoints follow /api/{resource}/{id} pattern
- [ ] Errors use ProblemDetails (RFC 7807)
- [ ] Clean architecture layers respected
- [ ] Audit fields on all entities
```

### Implementation Validation - Enhanced

**Added Cross-Section Consistency Matrix:**

```markdown
Section 3.2 (Personas) ↔ Section 3.4 (User Stories)
- [ ] Every user story references a persona
- [ ] Every persona has at least one story

Section 3.3 (Features) ↔ Section 3.4 (Stories)
- [ ] Every feature has stories
- [ ] Every story maps to a feature

Section 3.4 (Stories) ↔ Section 3.5 (Screens)
- [ ] Every story is supported by screens
- [ ] Every screen supports stories

Section 3.4 (Stories) ↔ Section 4.3 (API Contracts)
- [ ] Every story has required API endpoints
- [ ] Every endpoint supports stories

Section 4.2 (Data Model) ↔ Section 3.3 (Features)
- [ ] Every feature has required entities
- [ ] No orphaned entities

Section 4.5 (Authorization) ↔ Section 4.2 (Entities)
- [ ] Every entity has authorization rules
- [ ] All CRUD operations covered

Section 4.3 (APIs) ↔ Code (Controllers)
- [ ] All endpoints implemented
- [ ] Signatures match contracts

Section 4.2 (Data Model) ↔ Code (Database)
- [ ] All entities exist as tables
- [ ] Relationships match
```

---

## How to Use These Enhancements

### When Running plan Action:

```
Step 1: Product Manager generates requirements
  ↓
Step 1.5: CLARIFICATION GATE (NEW!)
  - Agent identifies vague criteria
  - Asks specific clarification questions
  - Updates requirements with quantified specs
  - Validates testability
  ↓
Step 2: User approves requirements
  ↓
Step 3: Architect designs architecture
```

### When Running validate Action:

```
Agent runtime runs enhanced validation checklists:

PM Validation:
  - Testability check (measurable criteria?)
  - Anti-pattern detection (banned words?)
  - Completeness check

Architect Validation:
  - Cross-mapping (stories ↔ endpoints?)
  - NFR quantification (numbers not adjectives?)
  - SOLUTION-PATTERNS compliance

Implementation Validation:
  - Cross-section consistency matrix
  - Code ↔ architecture alignment
```

---

## Benefits

### From Clarification Step:
✅ Catches vagueness early (before architecture)
✅ Prevents architect assumptions
✅ Ensures testable acceptance criteria
✅ Documents edge cases and errors upfront
✅ Reduces rework

### From Enhanced Validation:
✅ Specific checklists (not just concepts)
✅ Anti-pattern detection (automated quality)
✅ Cross-section consistency (no orphans)
✅ Quantified NFRs (measurable, not subjective)
✅ Pattern compliance (SOLUTION-PATTERNS.md)

---

## Example: Before vs After

### Before Clarification:

**Story:** "Customer search should be fast and intuitive"

**Acceptance Criteria:**
- System should return results quickly
- Interface should be easy to use
- Search should be accurate

### After Clarification:

**Story:** "Customer search must return results within performance targets and support common search patterns"

**Acceptance Criteria:**
- API response < 200ms p95 for searches with < 1000 results
- Search supports: name (partial match), state (exact), status (exact)
- Empty search returns all customers (paginated, 20 per page)
- No results shows: "No customers found. Try different search terms."
- Invalid search shows: "Invalid search. Name must be 2+ characters."
- Pagination: default 20/page, max 100/page
- Sorting: by name (asc/desc), by created date (asc/desc)

---

## Implementation Status

- ✅ Clarification step added to `plan.md`
- ✅ Validation enhancements documented (this file)
- ⏳ validate.md enhancements (manual update needed)

To complete: Manually update `agents/actions/validate.md` with the enhanced checklists from this document.

---

**Next:** Run plan action on a feature and test the clarification step!

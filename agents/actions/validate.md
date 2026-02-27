# Action: Validate

## User Intent

Validate that requirements, architecture, and implementation are aligned and that all artifacts are complete and consistent.

## Agent Flow

```
Architect (architecture validation) + Product Manager (requirements validation)
```

**Flow Type:** Parallel (both validations can run simultaneously)

## Prerequisites

- [ ] `planning-mds/BLUEPRINT.md` exists with planning content
- [ ] Architecture artifacts exist (data model, API contracts, workflows)
- [ ] Optional: Implementation code exists (for implementation validation)

## Inputs

### From Planning
- `planning-mds/BLUEPRINT.md` (all sections)
- `planning-mds/architecture/` (architecture artifacts)
- `planning-mds/examples/` (personas, features, stories, screens)
- `planning-mds/domain/glossary.md`

### From Codebase (Optional)
- Database schema (migrations)
- API endpoints (controllers)
- Domain entities (C# classes)
- Frontend components

### From User
- Validation scope (requirements, architecture, implementation, or all)
- Specific areas of concern

## Outputs

### Requirements Validation Report (Product Manager)
- Completeness check (all sections filled, no TODOs)
- Consistency check (requirements don't contradict)
- Traceability check (features → stories → acceptance criteria)
- Clarity check (no ambiguous requirements)
- Non-goals check (scope boundaries clear)
- Issues found with severity and recommendations

### Architecture Validation Report (Architect)
- Architecture completeness check
- Requirements alignment check (architecture satisfies requirements)
- Data model validation (entities match requirements)
- API contract validation (endpoints match stories)
- Workflow validation (state machines match requirements)
- Authorization model validation (roles and permissions defined)
- NFR validation (measurable and achievable)
- Issues found with severity and recommendations

### Implementation Validation Report (Architect + Product Manager)
- Code-to-architecture alignment
- Database schema matches data model
- API endpoints match contracts
- Features implement acceptance criteria
- Missing requirements or features
- Implementation drift from architecture

## Agent Responsibilities


### Product Manager (Requirements Validation)

1. **Check BLUEPRINT.md Section 3 completeness**
   - [ ] All subsections filled (3.1-3.5)
   - [ ] No TODO or placeholder text

2. **Validate vision and non-goals clarity**
   - [ ] Vision is 1-2 sentences, clear outcome
   - [ ] Non-goals are explicit (what we're NOT building)
   - [ ] Success metrics defined

3. **Check personas are well-defined**
   - [ ] Each persona has: name, role, goals, pain points
   - [ ] Personas represent actual target users
   - [ ] Primary vs secondary personas identified

4. **Validate features trace to user needs**
   - [ ] Every feature maps to a persona need
   - [ ] No features without clear user value
   - [ ] Features prioritized (MVP vs future)

5. **Check user stories have testable acceptance criteria**

   **Testability Checklist (per story):**
   - [ ] Has "As a / I want / So that" structure
   - [ ] Acceptance criteria are specific and measurable
   - [ ] No banned words: "should", "might", "easy", "fast", "secure" (without specifics)
   - [ ] Performance criteria quantified (< Xms, not "fast")
   - [ ] Error scenarios specified ("if X fails, then Y")
   - [ ] Edge cases identified (empty lists, max values, nulls, etc.)
   - [ ] Dependencies documented

   **Anti-Pattern Detection:**

   Flag these as issues:
   - ❌ "System should be fast" → ✅ "API responses < 200ms p95"
   - ❌ "Users can upload files" → ✅ "Users can upload PDF/PNG (max 10MB)"
   - ❌ "Secure authentication" → ✅ "JWT tokens, HTTPS only, session timeout 30min"
   - ❌ "Easy to use interface" → ✅ "3-click maximum to create customer"
   - ❌ "Dashboard is intuitive" → ✅ "Dashboard shows: revenue chart, top 5 customers, recent orders"

6. **Validate screen specs are complete**
   - [ ] Each screen has: purpose, layout, key elements
   - [ ] Screens support user stories
   - [ ] Navigation between screens defined

7. **Check for contradictions or ambiguities**
   - [ ] No conflicting requirements
   - [ ] Consistent terminology (use glossary)
   - [ ] Clear priorities (no "all are critical")

8. **Verify no invented business rules**
   - [ ] All rules trace to user needs or constraints
   - [ ] Assumptions documented explicitly
   - [ ] Questions raised for unknowns (not invented)

9. **Provide validation report with issues**










### Architect (Architecture Validation)
1. Check `BLUEPRINT.md` section 4 completeness
2. Validate service boundaries are clear
3. Check data model completeness and consistency
4. Validate API contracts align with stories
5. Check authorization model covers all resources
6. Validate workflows match requirements
7. Check NFRs are measurable
8. Verify ADRs exist for key decisions
9. Validate architecture satisfies all requirements
10. Provide validation report with issues

### Both (Implementation Validation)
1. Compare database schema to data model
2. Compare API endpoints to API contracts
3. Check implemented features vs planned features
4. Validate acceptance criteria implementation
5. Identify implementation drift
6. Provide alignment report

## Validation Criteria

### Requirements Valid
- [ ] All BLUEPRINT.md section 3 subsections complete
- [ ] No TODOs or placeholders remain
- [ ] Vision and non-goals clear
- [ ] All features trace to user needs
- [ ] All stories have acceptance criteria
- [ ] No contradictions found
- [ ] Scope boundaries explicit

### Architecture Valid
- [ ] All BLUEPRINT.md section 4 subsections complete
- [ ] Service boundaries defined
- [ ] Data model complete with relationships
- [ ] API contracts match user stories
- [ ] Authorization model comprehensive
- [ ] Workflows specified
- [ ] NFRs measurable
- [ ] Architecture satisfies requirements

### Implementation Valid
- [ ] Database schema matches data model
- [ ] API endpoints match contracts
- [ ] Features implement acceptance criteria
- [ ] No missing planned features
- [ ] No architectural drift
- [ ] Code follows architecture patterns

## Example Usage

### Scenario 1: Pre-Build Validation
```
User: "Validate architecture before we start building"

Validate Action:
  ↓
Product Manager (parallel):
  - Validates requirements completeness
  - Checks for ambiguities
  - Report: "Requirements valid, ready to build"

Architect (parallel):
  - Validates architecture completeness
  - Checks requirements alignment
  - Report: "Architecture valid, data model complete"
  ↓
Result: READY TO BUILD
```

### Scenario 2: Implementation Validation
```
User: "Validate that our implementation matches the plan"

Validate Action:
  ↓
Product Manager (parallel):
  - Compares implemented features to planned stories
  - Checks acceptance criteria coverage
  - Report: "3 stories implemented, 2 pending, all AC met"

Architect (parallel):
  - Compares database schema to data model
  - Checks API endpoints vs contracts
  - Report: "Schema aligned, 2 endpoints missing authorization"
  ↓
Result: ISSUES FOUND (missing authorization)
Recommendation: Fix authorization before deployment
```

### Scenario 3: Full Validation
```
User: "Validate entire project from requirements to implementation"

Validate Action:
  ↓
Product Manager (parallel):
  - Validates all requirements
  - Validates implementation completeness
  - Report: "Requirements complete, 80% features implemented"

Architect (parallel):
  - Validates architecture
  - Validates code alignment
  - Report: "Architecture sound, minor drift in error handling"
  ↓
Result: MOSTLY VALID with recommendations
```

## Validation Severity Levels

### Critical
- Missing required sections
- Major contradictions
- Fundamental architecture flaws
- Security gaps

### High
- Incomplete specifications
- Ambiguous requirements
- Missing API contracts
- Unmet acceptance criteria

### Medium
- Minor inconsistencies
- Optimization opportunities
- Documentation gaps

### Low
- Style suggestions
- Nice-to-have improvements

## Post-Validation Next Steps

### If Validation Passes
1. Proceed with confidence to next phase
2. Optionally address low-priority suggestions
3. Ready to run **[build action](./build.md)** or continue implementation

### If Validation Fails
1. Address critical and high issues
2. Re-run validate action
3. Repeat until validation passes

## Related Actions

- **Before Building:** Run validate after [plan action](./plan.md)
- **During Building:** Run validate to check alignment
- **After Building:** Run validate before deployment
- **Continuous:** Run validate regularly to catch drift

## Notes

- Validate action is non-destructive (read-only)
- Can be run at any phase of the project
- Use validation scripts in `agents/*/scripts/` if available
- Validation reports should be saved to `planning-mds/validation/`
- Validation can be automated in CI/CD pipeline
- Re-run after fixing issues to confirm resolution

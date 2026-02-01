# Refactoring Inventory Summary

**Date:** 2026-02-01
**Purpose:** Document all files that need to be moved or modified

---

## Files to Move to planning-mds/

### Product Manager (9 files with Nebula content)
- [x] agents/product-manager/references/insurance-domain-glossary.md → planning-mds/domain/insurance-glossary.md
- [x] agents/product-manager/references/crm-competitive-analysis.md → planning-mds/domain/crm-competitive-analysis.md
- [x] agents/product-manager/references/persona-examples.md → planning-mds/examples/personas/nebula-personas.md
- [x] agents/product-manager/references/feature-examples.md → planning-mds/examples/features/nebula-features.md
- [x] agents/product-manager/references/story-examples.md → planning-mds/examples/stories/nebula-stories.md
- [x] agents/product-manager/references/screen-spec-examples.md → planning-mds/examples/screens/nebula-screens.md
- [ ] **REVIEW:** pm-best-practices.md (check for embedded Nebula examples)
- [ ] **REVIEW:** vertical-slicing-guide.md (check for embedded Nebula examples)
- [ ] **REVIEW:** inception-requirements.md (check if generic or Nebula-specific)

### Architect (2+ files confirmed Nebula-specific)
- [x] agents/architect/references/insurance-crm-architecture-patterns.md → planning-mds/domain/crm-architecture-patterns.md
- [x] agents/architect/references/architecture-examples.md → planning-mds/examples/architecture/nebula-architecture.md
- [ ] **REVIEW:** api-design-guide.md (has MANY Broker/Submission code examples - need to replace with generic)
- [ ] **REVIEW:** architecture-best-practices.md (has MANY Broker/Submission code examples - need to replace with generic)

### Frontend Developer (1 file confirmed Nebula-specific)
- [x] agents/frontend-developer/references/design-tokens-nebula.md → planning-mds/frontend/design-tokens-nebula.md

---

## Files to Create (Generic Replacements)

### Product Manager
- [ ] agents/product-manager/references/persona-examples.md (NEW - B2B SaaS, e-commerce, healthcare)
- [ ] agents/product-manager/references/feature-examples.md (NEW - task mgmt, e-commerce, scheduling)
- [ ] agents/product-manager/references/story-examples.md (NEW - generic stories)
- [ ] agents/product-manager/references/screen-spec-examples.md (NEW - generic screen specs)

### Architect
- [ ] agents/architect/references/architecture-examples.md (NEW - e-commerce, CMS, SaaS)
- [ ] **CLEAN:** api-design-guide.md (replace Broker/Submission examples with Product/Order/Customer examples)
- [ ] **CLEAN:** architecture-best-practices.md (replace Broker/Submission examples with generic examples)

### Frontend Developer
- [ ] agents/frontend-developer/references/design-tokens.md (NEW - generic design tokens guide)
OR keep design-tokens-nebula.md but document it's solution-specific

---

## Templates to Review (7 templates have Nebula content!)

- [ ] agents/templates/acceptance-criteria-checklist.md - Check for Nebula examples
- [ ] agents/templates/adr-template.md - Check for Nebula examples
- [ ] agents/templates/entity-model-template.md - Check for Nebula examples
- [ ] agents/templates/feature-template.md - Check for Nebula examples
- [ ] agents/templates/screen-spec-template.md - Check for Nebula examples
- [ ] agents/templates/story-template.md - Check for Nebula examples
- [ ] agents/templates/workflow-spec-template.md - Check for Nebula examples

**Action:** Replace any Nebula examples with generic placeholders like `[EntityName]`, `[UserName]`, etc.

---

## Scripts to Review

- [ ] agents/product-manager/scripts/generate-story-index.py - Generic or solution-specific?
- [ ] agents/product-manager/scripts/validate-stories.py - Generic or solution-specific?

---

## Documentation to Create

### planning-mds/ Documentation
- [ ] planning-mds/README.md
- [ ] planning-mds/BOUNDARY-POLICY.md
- [ ] planning-mds/domain/README.md
- [ ] planning-mds/examples/README.md
- [ ] planning-mds/examples/personas/README.md
- [ ] planning-mds/examples/features/README.md
- [ ] planning-mds/examples/stories/README.md
- [ ] planning-mds/examples/screens/README.md
- [ ] planning-mds/examples/architecture/README.md
- [ ] planning-mds/examples/architecture/adrs/README.md
- [ ] planning-mds/frontend/README.md (for design tokens)

### agents/ Documentation
- [ ] agents/README.md (reusability guide)

---

## ADRs to Create (Example ADRs for planning-mds/)

- [ ] planning-mds/examples/architecture/adrs/ADR-001-modular-monolith.md
- [ ] planning-mds/examples/architecture/adrs/ADR-002-ef-core.md
- [ ] planning-mds/examples/architecture/adrs/ADR-003-casbin-abac.md

---

## Files to Update

### Agent SKILL.md and README.md
- [ ] agents/product-manager/SKILL.md - Add generic/solution resource separation
- [ ] agents/product-manager/README.md - Add generic/solution resource separation
- [ ] agents/architect/SKILL.md - Add generic/solution resource separation
- [ ] agents/architect/README.md - Add generic/solution resource separation
- [ ] agents/frontend-developer/SKILL.md - Add note about design tokens
- [ ] agents/frontend-developer/README.md - Add note about design tokens

### Status and Planning
- [ ] agents/AGENT-STATUS.md - Update status
- [ ] planning-mds/INCEPTION.md - Add reference documentation section

---

## Other Agents to Check

The scan found Nebula references in multiple agents. Need to review:

- [ ] agents/devops/ - Check for Nebula-specific content
- [ ] agents/backend-developer/ - Check for Nebula-specific code examples
- [ ] agents/code-reviewer/ - Check for Nebula-specific examples
- [ ] agents/security/ - Check for Nebula-specific threat models
- [ ] agents/quality-engineer/ - Check for Nebula-specific test examples

---

## Scan Results Summary

**Total files with Nebula content:** 55+

**Key findings:**
1. **Product Manager:** ALL 9 reference files have Nebula content
2. **Architect:** At least 4 files have heavy Nebula content (api-design-guide, architecture-best-practices)
3. **Frontend Developer:** 1 file clearly Nebula-specific (design-tokens-nebula.md)
4. **Templates:** 7 templates have Nebula content
5. **Other agents:** Need to review for Nebula-specific examples

---

## Priority Order

### Phase 1: High Priority (Core Examples)
1. Product Manager persona, feature, story, screen examples
2. Architect architecture examples
3. Templates (remove Nebula examples)

### Phase 2: Medium Priority (Embedded Examples in Guides)
1. api-design-guide.md (replace Broker/Submission with Product/Order)
2. architecture-best-practices.md (replace Broker/Submission with generic)
3. Frontend design tokens

### Phase 3: Low Priority (Review Other Agents)
1. Review backend-developer, code-reviewer, security, quality-engineer
2. Clean up any Nebula-specific examples found

---

## Next Steps

1. ✅ Create directory structure in planning-mds/
2. ✅ Move high-priority files to planning-mds/
3. ✅ Create generic replacements
4. ✅ Clean templates
5. ✅ Clean embedded examples in guides
6. ✅ Update documentation
7. ✅ Validate and test

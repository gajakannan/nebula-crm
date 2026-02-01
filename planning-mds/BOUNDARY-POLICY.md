# Boundary Policy: Generic Agents vs Solution-Specific Content

**Date:** 2026-02-01
**Status:** Active
**Owner:** Architecture Team

---

## Purpose

This policy defines the boundary between generic, reusable agent roles (`agents/`) and solution-specific content (`planning-mds/`).

---

## Policy Rules

### Rule 1: agents/ is Generic and Reusable

**Principle:** Everything in `agents/` must be applicable to **any** software project, regardless of domain.

**What belongs in agents/:**
- ✅ Agent role definitions (SKILL.md, README.md)
- ✅ Generic best practices (SOLID, DDD, INVEST, vertical slicing, etc.)
- ✅ Generic examples from multiple domains (B2B SaaS, e-commerce, healthcare, etc.)
- ✅ Generic templates (story, feature, persona, ADR, API contract, etc.)
- ✅ Generic scripts and tools (validation, linting, formatting)

**What does NOT belong in agents/:**
- ❌ Domain-specific terminology (insurance, underwriting, claims, etc.)
- ❌ Competitive analysis for specific markets
- ❌ Solution-specific examples (personas, features, stories referencing project entities)
- ❌ Solution-specific architecture patterns
- ❌ Project-specific business rules or workflows

---

### Rule 2: planning-mds/ is Solution-Specific

**Principle:** Everything in `planning-mds/` is specific to the current project and would be replaced for a new project.

**What belongs in planning-mds/:**
- ✅ Project master specification (INCEPTION.md)
- ✅ Domain knowledge (glossary, competitive analysis, domain-specific patterns)
- ✅ Project-specific examples (personas, features, stories, architecture)
- ✅ Actual project requirements (features/, stories/, architecture/)
- ✅ Project-specific ADRs and design decisions

**What does NOT belong in planning-mds/:**
- ❌ Generic best practices (those go in agents/)
- ❌ Generic examples from other domains
- ❌ Reusable templates (those go in agents/templates/)

---

### Rule 3: Agents Must Not Invent Requirements

**Principle:** Agent roles consume requirements from `planning-mds/`; they do not create or embed solution requirements.

**Implementation:**
- Agents read from `planning-mds/INCEPTION.md` and `planning-mds/domain/` to understand project context
- Agents reference `planning-mds/examples/` to see how generic patterns apply to this project
- Agents generate deliverables based on templates in `agents/templates/` and requirements in `planning-mds/`
- Agents never hard-code project-specific business logic in their role definitions

---

### Rule 4: Starting a New Project

**Principle:** Reusing agents for a new project should be as simple as copying `agents/` and creating new `planning-mds/`.

**Process:**
1. Copy entire `agents/` directory to new project
2. Create new `planning-mds/` directory structure
3. Write new domain knowledge in `planning-mds/domain/`
4. Create new project-specific examples in `planning-mds/examples/`
5. Write new INCEPTION.md for the new project
6. Agents are immediately ready to use with new project context

---

## Enforcement

### Pre-Commit Checks

Run validation before committing changes to `agents/`:

```bash
# Check for project-specific terms (customize for your project)
grep -r "YourProjectName\|YourDomainEntity" agents/ --include="*.md"
```

### Code Review Checklist

When reviewing PRs that modify `agents/`:
- [ ] No project-specific terminology in agent files
- [ ] Examples are generic and span multiple domains
- [ ] No hard-coded business rules or domain logic
- [ ] All project-specific content belongs in `planning-mds/`

When reviewing PRs that modify `planning-mds/`:
- [ ] Content is specific to current project
- [ ] No generic best practices (those belong in `agents/`)
- [ ] References to `agents/` resources are correct

---

## Examples

### ✅ GOOD: Generic Example

**File:** `agents/product-manager/references/persona-examples.md`

```markdown
## Example: B2B SaaS - Sales Representative

**Name:** Alex Rivera
**Role:** Enterprise Sales Representative
**Goals:** Close enterprise deals, reduce sales cycle, improve lead qualification
```

**Why good:** Uses a generic domain (B2B SaaS), generic persona name (Alex Rivera), no project-specific terms.

---

### ❌ BAD: Project-Specific in agents/

**File:** `agents/product-manager/references/persona-examples.md` (WRONG)

```markdown
## Example: Insurance CRM - Distribution Manager

**Name:** Sarah Chen
**Role:** Distribution Manager
**Goals:** Manage broker relationships, track submissions, process renewals
```

**Why bad:** References insurance domain, project-specific persona (Sarah Chen), project-specific entities (broker, submissions, renewals). This belongs in `planning-mds/examples/personas/`.

---

### ✅ GOOD: Solution-Specific in planning-mds/

**File:** `planning-mds/examples/personas/nebula-personas.md`

```markdown
## Persona: Distribution Manager

**Name:** Sarah Chen
**Role:** Distribution Manager at Nebula Insurance
**Goals:** Manage broker relationships, track submissions, process renewals
```

**Why good:** Project-specific persona in the correct location (`planning-mds/`).

---

## Version History

**Version 1.0** - 2026-02-01 - Initial boundary policy

# Project Planning Documents

This directory contains all solution-specific planning documents, domain knowledge, and examples for this project.

---

## Directory Structure

```
planning-mds/
├── INCEPTION.md                     # Master specification (single source of truth)
├── BOUNDARY-POLICY.md               # Policy defining agents/ vs planning-mds/ boundary
│
├── domain/                          # Domain knowledge
│   ├── insurance-glossary.md        # Insurance terminology
│   ├── crm-competitive-analysis.md  # CRM competitive landscape
│   └── crm-architecture-patterns.md # Insurance CRM architectural patterns
│
├── examples/                        # Project-specific examples
│   ├── personas/                    # Project personas (Sarah Chen, Marcus, Jennifer)
│   ├── features/                    # Project features (F1-F6)
│   ├── stories/                     # Project user stories
│   ├── screens/                     # Project screen specs
│   └── architecture/                # Project architecture examples
│       └── adrs/                    # Architecture Decision Records
│
├── frontend/                        # Frontend-specific artifacts (design tokens)
├── features/                        # Actual project features
├── stories/                         # Actual project user stories
├── architecture/                    # Architecture decisions and specs
├── api/                             # API contracts
├── security/                        # Security artifacts
└── workflows/                       # Workflow specifications
```

---

## What's in This Directory

### Domain Knowledge (`domain/`)

**Purpose:** Domain-specific knowledge that Product Managers and Architects need to understand this project.

**Contents:**
- Domain glossary (insurance terminology, definitions)
- Competitive analysis (CRM market landscape, baseline features)
- Domain-specific architecture patterns (broker hierarchy, submission workflow, etc.)

**When to use:** Reference during requirements definition and architecture design to understand domain context.

---

### Examples Library (`examples/`)

**Purpose:** Real project examples showing how to apply generic templates to this specific solution.

**Product Examples:**
- `personas/` - Project user personas (Sarah Chen, Marcus, Jennifer)
- `features/` - Project feature examples (F1-F6: Broker Management, Account 360, etc.)
- `stories/` - Project user story examples
- `screens/` - Project screen specifications

**Architecture Examples:**
- `architecture/` - Complete architecture examples for this project
- `architecture/adrs/` - Architecture Decision Records explaining key technical choices

**When to use:** Reference when writing actual features, stories, and architecture. Use as templates showing "here's how the generic template was applied to our solution."

---

## Relationship to agents/

**agents/ = Generic (reusable across projects)**
- Contains agent role definitions (SKILL.md, README.md)
- Contains generic best practices
- Contains generic examples from multiple domains
- Contains generic templates
- **Can be copied wholesale to a new project**

**planning-mds/ = Solution-Specific (this project only)**
- Contains project domain knowledge
- Contains project-specific examples
- Contains actual project requirements
- **Unique to this project** - would be replaced for a new project

**See:** `BOUNDARY-POLICY.md` for detailed rules on what belongs where.

---

## How to Use

**For Product Managers:**
1. Read `domain/` to understand domain terminology and competitive landscape
2. Reference `examples/personas/`, `examples/features/`, `examples/stories/` to see how to write project specs
3. Use `../agents/templates/` for generic templates
4. Write actual features/stories in `features/` and `stories/` directories

**For Architects:**
1. Read `domain/` to understand domain-specific architecture patterns
2. Reference `examples/architecture/` to see complete module examples
3. Use `../agents/templates/adr-template.md` for ADR structure
4. Write actual architecture decisions in `architecture/decisions/`
5. Write actual API contracts in `api/`

---

## Version History

**Version 1.0** - 2026-02-01 - Initial planning-mds structure with domain knowledge and examples separated from generic agents

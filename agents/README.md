# Generic Agent Roles - Agent-Driven Builder Framework

## Purpose

This directory contains **generic, reusable** agent role definitions for building software using an agent-driven builder methodology. Each agent represents a specialized role with clear responsibilities, inputs, outputs, and quality standards.

**✅ 100% Generic** - All content in this directory is domain-agnostic and can be applied to any software project (insurance, healthcare, e-commerce, B2B SaaS, etc.).

## Philosophy

Agent-driven builder methodology treats the software product lifecycle as a series of specialized roles that collaborate through well-defined handoffs. This approach:

- **Reduces ambiguity** — Each agent has explicit scope and deliverables
- **Ensures quality** — Built-in review gates and acceptance criteria
- **Enables parallelization** — Independent agents can work concurrently where appropriate
- **Maintains consistency** — Templates and standards ensure uniform outputs
- **Facilitates iteration** — Clear handoff points allow for feedback loops

## How to Use Agent Roles

### Phase-Based Activation

Agent roles activate according to the three-phase development process defined in `planning-mds/INCEPTION.md`:

**Phase A — Product Manager Mode**
- Product Manager Agent (primary)
- Blogger Agent (optional: dev log)

**Phase B — Architect/Tech Lead Mode**
- Architect Agent (primary)
- Security Agent (security design review)
- Blogger Agent (optional: architecture decisions)

**Phase C — Implementation Mode**
- Backend Developer Agent
- Frontend Developer Agent
- Quality Engineer Agent
- DevOps Agent
- Technical Writer Agent
- Code Reviewer Agent
- Security Agent (security testing)
- Blogger Agent (implementation progress)

### Invoking an Agent

When you need an agent to perform work:

1. **Read the agent specification** (`agents/<role>/SKILL.md`)
2. **Check prerequisites** — Ensure required inputs/artifacts are available
3. **Provide clear context** — Reference the single source of truth (INCEPTION.md or derived specs)
4. **Use templates** — Agents should use templates from `agents/templates/` for consistency
5. **Verify outputs** — Check that deliverables meet the agent's definition of done

### Handoff Rules

**Critical Principle:** Agents must not invent requirements or make assumptions beyond their scope.

1. **Sequential Dependencies**
   - Product Manager → Architect (requirements must be finalized)
   - Architect → Developers (technical spec must be complete)
   - Developers → Quality Engineer (code must be testable)
   - Quality Engineer → Code Reviewer (tests must pass)

2. **Parallel Work**
   - Backend and Frontend can work in parallel once API contracts are defined
   - Quality Engineer can begin writing tests in parallel once testable code/APIs exist
   - Security Agent reviews can happen alongside development
   - Technical Writer can document APIs as they're built
   - Blogger Agent operates independently throughout

3. **Feedback Loops**
   - Any agent can raise questions/blockers to upstream agents
   - Code Reviewer can send work back to developers
   - Quality Engineer can flag architectural issues to Architect

4. **Quality Gates**
   - No phase transition without completion criteria met
   - No code merge without Code Reviewer approval
   - No deployment without Quality Engineer sign-off (tests pass) + DevOps approval (infrastructure ready)

## Agent Collaboration Model

```
┌─────────────────┐
│ Product Manager │ ──────> Phase A Complete
└────────┬────────┘
         │
         v
┌─────────────────┐
│   Architect     │ ──────> Phase B Complete
└────────┬────────┘         ↓ (Security design review)
         │                  │
         v                  v
┌────────────────────────────────────────┐
│  Backend Dev  │  Frontend Dev  │  ...  │ ──> Implementation
└────────────────────────────────────────┘
         │                  ↓ (Security testing)
         v                  │
┌─────────────────┐         v
│ Quality Engineer│ ──────> Tests Pass
└────────┬────────┘
         │
         v
┌─────────────────┐
│  Code Reviewer  │ ──────> Approved
└────────┬────────┘
         │
         v
┌─────────────────┐
│   DevOps Agent  │ ──────> Deployed
└─────────────────┘

      Blogger Agent (continuous, all phases)
      Security Agent (Phase B design review + Phase C testing)
```

## Single Source of Truth

All agent work must derive from:

1. **`planning-mds/INCEPTION.md`** — The master specification
2. **Phase A outputs** — Product Manager deliverables (requirements, stories)
3. **Phase B outputs** — Architect deliverables (technical specs, data models, API contracts)

**Rule:** If a requirement isn't documented in these sources, agents should raise questions rather than invent solutions.

## Templates

The `templates/` directory contains standard formats for common deliverables. Agents should use these to ensure consistency:

- `story-template.md` — User story format (Product Manager)
- `review-checklist.md` — Code review checklist (Code Reviewer)
- `test-plan-template.md` — Test strategy and plan (Quality Engineer)
- `runbook-template.md` — Operational runbook (Technical Writer)
- `devlog-template.md` — Development blog post (Blogger)

## Repo Layout

```
/
├── experience/          # React frontend
├── engine/              # C# / ASP.NET Core backend
├── soma/                # Intelligence layer
│   ├── models/          # local/finetuned/slm/quantized assets
│   ├── app-agents/      # application-specific agents + prompts + tools
│   └── mcp/             # MCP servers/endpoints + configs
├── agents/              # builder agent specs (roles, templates, references)
├── planning-mds/        # product artifacts: epics/stories/specs
└── docs/                # shared documentation (optional)
```

## Getting Started

1. Read `ROLES.md` for a quick reference of all agent roles
2. Review the specific agent specification for the role you need
3. Check `planning-mds/INCEPTION.md` for project context
4. Use the appropriate template for deliverables
5. Follow the handoff rules and quality gates

## Reusing Agents for New Projects

### How to Use for a New Project

**Step 1: Copy agents/ Directory**
```bash
cp -r current-project/agents new-project/agents
```
Result: You now have all agent roles, best practices, and templates ready to use.

**Step 2: Create Solution-Specific planning-mds/**
```bash
mkdir -p new-project/planning-mds/{domain,examples,features,stories,architecture}
```

**Step 3: Create Domain Knowledge**
- Create `planning-mds/domain/[domain]-glossary.md` - Define domain-specific terminology
- Create `planning-mds/domain/[domain]-competitive-analysis.md` - Analyze competitive landscape
- Create `planning-mds/domain/[domain]-architecture-patterns.md` - Document domain-specific patterns

**Step 4: Create Solution-Specific Examples**
- Create personas, features, stories, and architecture examples in `planning-mds/examples/`

**Step 5: Create INCEPTION.md**
- Reference domain knowledge and examples specific to your new project

**Step 6: Start Building**
- All generic best practices, templates, and patterns are ready to use from `agents/`

### What's Generic (in agents/) vs Solution-Specific (in planning-mds/)

**agents/ = Generic and Reusable**
- Agent role definitions
- Generic best practices (SOLID, DDD, INVEST, etc.)
- Generic examples from multiple domains
- Generic templates

**planning-mds/ = Solution-Specific**
- Project domain knowledge and terminology
- Project-specific personas, features, stories, architecture
- Actual project requirements

**See:** `../planning-mds/BOUNDARY-POLICY.md` for detailed rules on what belongs where.

---

## Questions or Issues

If agent role boundaries are unclear or handoffs are blocked:
- Raise the issue explicitly
- Reference the specific agent roles involved
- Propose a resolution or request clarification

**Version History:**
- **v2.0** - 2026-02-01 - Separated generic agents from solution-specific content
- **v1.0** - 2026-01-26 - Initial agent-driven builder framework

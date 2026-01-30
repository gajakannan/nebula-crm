---
template: architecture-decision-record
version: 1.0
applies_to: architect
---

# ADR-[NNN]: [Short Title of Decision]

**Status:** [Proposed | Accepted | Deprecated | Superseded]

**Date:** [YYYY-MM-DD]

**Deciders:** [List of people involved in the decision]

**Technical Story:** [Link to user story, epic, or issue if applicable]

---

## Context and Problem Statement

[Describe the context and problem statement in 2-3 sentences. Use active voice and present tense.]

**Example:**
> Nebula needs a strategy for authorization that supports fine-grained access control based on user roles, resource ownership, and contextual attributes. The system must enforce authorization consistently across all API endpoints while remaining maintainable as business rules evolve.

**Questions to answer:**
- What is the architectural decision we need to make?
- What problem are we trying to solve?
- Why is this decision important now?

---

## Decision Drivers

[List the key factors influencing this decision, in priority order]

- [Driver 1]: [Description]
- [Driver 2]: [Description]
- [Driver 3]: [Description]

**Example:**
- **Security:** Must enforce least-privilege access control
- **Complexity:** Solution must be understandable and maintainable by team
- **Flexibility:** Need to support evolving business rules without code changes
- **Performance:** Authorization checks must complete in < 10ms
- **Auditability:** Need to log all authorization decisions for compliance

---

## Considered Options

[List all options that were seriously considered]

1. **[Option 1 Name]** - [One-line description]
2. **[Option 2 Name]** - [One-line description]
3. **[Option 3 Name]** - [One-line description]

**Example:**
1. **Role-Based Access Control (RBAC)** - Simple roles with fixed permissions
2. **Attribute-Based Access Control (ABAC) with Casbin** - Policy-based authorization using attributes
3. **Custom Authorization Framework** - Build our own authorization engine

---

## Decision Outcome

**Chosen option:** "[Option Name]"

[Explain why this option was chosen in 1-2 paragraphs. Focus on how it best addresses the decision drivers.]

**Example:**
> Chosen option: "Attribute-Based Access Control (ABAC) with Casbin" because it provides the flexibility needed for complex authorization rules (e.g., "underwriters can only update submissions assigned to them") while remaining maintainable through externalized policy configuration. Casbin is mature, well-documented, and has proven performance characteristics (< 1ms per authorization check).

---

## Consequences

### Positive Consequences

[List the benefits of this decision]

- ✅ [Benefit 1]
- ✅ [Benefit 2]
- ✅ [Benefit 3]

**Example:**
- ✅ Fine-grained authorization rules without code changes
- ✅ Clear separation between application logic and authorization logic
- ✅ Mature library with active community support
- ✅ Excellent performance (< 1ms authorization checks)
- ✅ Easy to test authorization policies independently

### Negative Consequences

[List the drawbacks or tradeoffs of this decision]

- ❌ [Drawback 1]
- ❌ [Drawback 2]

**Example:**
- ❌ Learning curve for team unfamiliar with Casbin policy syntax
- ❌ Additional dependency on external library
- ❌ Policy configuration is separate from code (could get out of sync)

### Risks and Mitigation

[List potential risks and how they will be mitigated]

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| [Risk description] | High/Medium/Low | High/Medium/Low | [Mitigation strategy] |

**Example:**
| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Policy misconfiguration grants unauthorized access | High | Medium | Comprehensive authorization tests, policy review process, audit logging |
| Casbin library becomes unmaintained | Medium | Low | Library is mature with active community; can fork if needed |
| Performance degrades with complex policies | Medium | Low | Performance testing in CI, policy optimization guidelines |

---

## Detailed Analysis of Options

### Option 1: [Option Name]

**Description:**
[Provide a detailed description of this option, including how it would be implemented]

**Pros:**
- ✅ [Pro 1]
- ✅ [Pro 2]

**Cons:**
- ❌ [Con 1]
- ❌ [Con 2]

**Implementation Complexity:** [Low | Medium | High]

**Estimated Effort:** [Hours/Days/Weeks]

**Example:**
### Option 1: Role-Based Access Control (RBAC)

**Description:**
Use simple role-based permissions where each user has one or more roles (e.g., "Distribution", "Underwriter", "Admin") and each role has a fixed set of permissions. Authorization checks would be implemented as `[Authorize(Roles = "Distribution")]` attributes on controllers or in middleware.

**Pros:**
- ✅ Simple to understand and implement
- ✅ Built into ASP.NET Core
- ✅ No external dependencies
- ✅ Fast authorization checks

**Cons:**
- ❌ Cannot express complex rules like "underwriters can only update their assigned submissions"
- ❌ Adding new permissions requires code changes and deployment
- ❌ Role explosion as business rules become more granular
- ❌ Difficult to test authorization logic independently

**Implementation Complexity:** Low

**Estimated Effort:** 2-3 days

---

### Option 2: [Option Name]

[Repeat structure for each option]

---

### Option 3: [Option Name]

[Repeat structure for each option]

---

## Decision Matrix

[Optional: Create a scoring matrix if multiple factors need to be weighed]

| Criteria | Weight | Option 1 | Option 2 | Option 3 |
|----------|--------|----------|----------|----------|
| [Criterion] | [1-5] | [Score × Weight] | [Score × Weight] | [Score × Weight] |
| **Total** | | **[Total]** | **[Total]** | **[Total]** |

**Scoring:** 1 (Poor) to 5 (Excellent)

**Example:**
| Criteria | Weight | RBAC | ABAC (Casbin) | Custom |
|----------|--------|------|---------------|--------|
| Flexibility | 5 | 2 × 5 = 10 | 5 × 5 = 25 | 5 × 5 = 25 |
| Simplicity | 4 | 5 × 4 = 20 | 3 × 4 = 12 | 1 × 4 = 4 |
| Performance | 3 | 5 × 3 = 15 | 5 × 3 = 15 | 3 × 3 = 9 |
| Maintainability | 5 | 3 × 5 = 15 | 4 × 5 = 20 | 2 × 5 = 10 |
| Time to Implement | 2 | 5 × 2 = 10 | 4 × 2 = 8 | 1 × 2 = 2 |
| **Total** | | **70** | **80** | **50** |

---

## Implementation Plan

[Optional: High-level implementation steps if the decision requires significant work]

1. **Phase 1:** [Description]
   - [ ] [Task]
   - [ ] [Task]

2. **Phase 2:** [Description]
   - [ ] [Task]
   - [ ] [Task]

**Example:**
1. **Phase 1: Setup** (1 day)
   - [ ] Add Casbin NuGet package
   - [ ] Create policy configuration structure
   - [ ] Define initial policy model

2. **Phase 2: Integration** (2 days)
   - [ ] Create authorization middleware
   - [ ] Define subject/resource attribute extractors
   - [ ] Wire into ASP.NET Core pipeline

3. **Phase 3: Policies** (3 days)
   - [ ] Write MVP authorization policies
   - [ ] Create authorization tests
   - [ ] Document policy management process

4. **Phase 4: Validation** (1 day)
   - [ ] End-to-end testing
   - [ ] Performance testing
   - [ ] Security review

---

## Validation and Success Metrics

[Define how you'll know if this decision was successful]

**Success Criteria:**
- [ ] [Criterion 1]
- [ ] [Criterion 2]
- [ ] [Criterion 3]

**Metrics to Track:**
- [Metric]: [Target value]

**Example:**
**Success Criteria:**
- [ ] All API endpoints enforce authorization
- [ ] Authorization checks complete in < 10ms (p95)
- [ ] Zero unauthorized access incidents
- [ ] Policy changes don't require code deployment
- [ ] Authorization tests achieve 100% coverage

**Metrics to Track:**
- Authorization check latency: < 10ms (p95)
- Policy update frequency: Track how often policies change
- Authorization test coverage: 100%
- Security audit findings: 0 critical/high

---

## Related Decisions

[Link to related ADRs or decisions]

- [ADR-XXX]: [Title] - [How it relates]

**Example:**
- ADR-002: Authentication with Keycloak - Defines how users are authenticated before authorization
- ADR-005: Audit Trail Strategy - Authorization decisions are logged to audit trail

---

## References

[Links to supporting documentation, research, or examples]

- [Reference 1]: [Description]
- [Reference 2]: [Description]

**Example:**
- Casbin Documentation: https://casbin.org/docs/overview
- NIST ABAC Guide: https://csrc.nist.gov/publications/detail/sp/800-162/final
- ASP.NET Core Authorization: https://learn.microsoft.com/aspnet/core/security/authorization/

---

## Notes

[Any additional notes, caveats, or context]

**Example:**
- Initial implementation will focus on MVP scenarios (broker CRUD, submission workflow)
- Advanced features (resource-level attributes, dynamic roles) deferred to Phase 1
- Policy syntax training session scheduled for development team

---

## Change Log

| Date | Author | Change |
|------|--------|--------|
| YYYY-MM-DD | [Name] | Initial draft |
| YYYY-MM-DD | [Name] | [Change description] |

**Example:**
| Date | Author | Change |
|------|--------|--------|
| 2024-01-15 | Architect Agent | Initial draft |
| 2024-01-20 | Security Agent | Added security validation criteria |
| 2024-01-25 | Team | Accepted after review |

---

## Template Instructions

**How to Use This Template:**

1. **Copy this template** to `planning-mds/architecture/decisions/ADR-[NNN]-[short-title].md`
2. **Replace all bracketed placeholders** with actual content
3. **Delete this instructions section** before committing
4. **Number ADRs sequentially** starting from ADR-001
5. **Keep ADRs immutable** - If a decision changes, create a new ADR that supersedes the old one
6. **Link from INCEPTION.md** Section 4.7 if significant architectural decisions

**ADR Naming Convention:**
- Use format: `ADR-[NNN]-[short-title-kebab-case].md`
- Example: `ADR-001-use-casbin-for-authorization.md`

**Status Values:**
- **Proposed:** Under discussion, not yet accepted
- **Accepted:** Decision has been made and is being implemented
- **Deprecated:** No longer recommended, but not yet replaced
- **Superseded:** Replaced by a newer ADR (reference the new ADR)

---

## Version History

**Version 1.0** - 2026-01-26 - Initial ADR template based on Michael Nygard's format

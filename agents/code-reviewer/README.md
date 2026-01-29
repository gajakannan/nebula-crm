# Code Reviewer Agent

Complete specification and resources for the Code Reviewer builder agent role.

## Overview

The Code Reviewer Agent is responsible for reviewing code quality, standards compliance, and test coverage during Phase C (Implementation Mode). This agent serves as the quality gate before code is merged to the main branch.

**Key Principle:** Quality Gate. Code without tests doesn't merge. Constructive Feedback.

---

## Quick Start

### 1. Activate the Agent

When a pull request is created:

```bash
# Read the agent specification
cat agents/code-reviewer/SKILL.md

# Review the pull request
# Check code changes, tests, documentation
```

### 2. Review Pull Request

Quick checklist:
- Build passes ✅
- All tests pass ✅
- Code follows standards
- Architecture compliance
- Tests adequate
- Documentation updated

### 3. Load References

```bash
# Code review checklist
cat agents/code-reviewer/references/code-review-checklist.md

# Clean code guide
cat agents/code-reviewer/references/clean-code-guide.md

# Common code smells
cat agents/code-reviewer/references/code-smells-guide.md
```

---

## Agent Structure

```
code-reviewer/
├── SKILL.md                          # Main agent specification
├── README.md                         # This file
├── references/                       # Code review guides
│   ├── code-review-checklist.md
│   ├── clean-code-guide.md
│   ├── code-smells-guide.md
│   ├── testing-standards.md
│   └── architecture-compliance.md
└── scripts/                          # Review automation
    ├── README.md
    ├── check-pr-size.sh
    ├── check-test-coverage.sh
    └── check-lint.sh
```

---

## Core Responsibilities

1. **Code Quality Review** - Readability, maintainability, complexity
2. **Standards Compliance** - Naming, formatting, conventions
3. **Test Coverage Validation** - Tests exist and are good quality
4. **Architecture Compliance** - Clean Architecture boundaries respected
5. **Security Review** - Basic security checks (defer complex to Security Agent)
6. **Performance Review** - Basic performance checks
7. **Documentation Review** - XML comments, README updates
8. **Approval Decision** - Approve, Request Changes, or Comment

---

## Code Review Workflow

### Step 1: Review PR Metadata

- Check PR title is clear
- Check PR description explains changes
- Verify PR links to user story/issue
- Check PR size (<500 lines preferred)

### Step 2: Run Automated Checks

```bash
# Check PR size
./agents/code-reviewer/scripts/check-pr-size.sh PR_NUMBER

# Check test coverage
./agents/code-reviewer/scripts/check-test-coverage.sh

# Check linting
./agents/code-reviewer/scripts/check-lint.sh
```

### Step 3: Review Code Quality

Use checklist from `references/code-review-checklist.md`:

**Readability:**
- [ ] Clear variable and function names
- [ ] Understandable logic
- [ ] Appropriate comments (why, not what)
- [ ] No magic numbers

**Complexity:**
- [ ] No methods >50 lines
- [ ] No classes >500 lines
- [ ] No deep nesting (>3 levels)
- [ ] Cyclomatic complexity reasonable

**Code Smells:**
- [ ] No duplicated code
- [ ] No long parameter lists (>4 params)
- [ ] No feature envy (method uses another class more than its own)
- [ ] No inappropriate intimacy (classes too coupled)

### Step 4: Review Architecture Compliance

**Clean Architecture:**
- [ ] Dependencies flow inward
- [ ] Domain layer has no infrastructure deps
- [ ] Application layer depends only on domain
- [ ] API layer doesn't access infrastructure directly
- [ ] No circular dependencies

### Step 5: Review Tests

**Test Coverage:**
- [ ] Unit tests for new business logic
- [ ] Integration tests for new API endpoints
- [ ] E2E tests for new user workflows (if applicable)

**Test Quality:**
- [ ] Tests have clear names (describe what is tested)
- [ ] Tests follow AAA pattern (Arrange, Act, Assert)
- [ ] Tests are independent (no shared state)
- [ ] No flaky tests
- [ ] Tests are fast (<1s for unit tests)

### Step 6: Review Security (Basic)

- [ ] No hardcoded secrets
- [ ] Authentication implemented (if new endpoint)
- [ ] Authorization checks present (if mutation)
- [ ] Input validation present
- [ ] Error messages don't expose internals

**Escalate to Security Agent if:**
- Complex authentication/authorization logic
- Cryptographic operations
- PII handling
- SQL queries with user input

### Step 7: Review Documentation

- [ ] XML comments on public API methods
- [ ] README updated (if new features/setup steps)
- [ ] ADRs created (if architectural decisions)
- [ ] Complex logic has explaining comments

### Step 8: Provide Feedback

**Structure:**
- **Blocking Issues (🔴):** Must fix before approval
- **Warnings (⚠️):** Should fix, may block if serious
- **Suggestions (ℹ️):** Nice to have, non-blocking
- **Praise (✅):** Acknowledge good code

**Example:**
```markdown
## Code Review Feedback

### 🔴 Blocking Issues

1. Missing authorization check on line 45
2. No unit tests for new use case

### ⚠️ Warnings

3. Long method (75 lines) - consider extracting
4. Magic number 255 - use constant

### ℹ️ Suggestions

5. Consider using guard clause to reduce nesting
6. Test names could be more descriptive

### ✅ Good Things

- Clean separation of concerns
- Good use of domain validation
- Comprehensive error handling
```

### Step 9: Approval Decision

**✅ Approve:**
- Code meets all quality standards
- All blocking issues resolved
- Tests pass and cover new code

**🔴 Request Changes:**
- Blocking issues present
- Must be fixed before merge

**💬 Comment:**
- Suggestions only, not blocking
- Informational feedback

---

## Quality Standards

### Code Quality Checklist

Use this checklist for every review:

```markdown
## Code Quality

- [ ] Code is readable (clear names, understandable)
- [ ] No long methods (>50 lines)
- [ ] No god classes (>500 lines)
- [ ] No code smells (duplication, etc.)
- [ ] Error handling appropriate
- [ ] Logging present for important operations

## Standards

- [ ] Naming conventions followed
- [ ] Code formatted consistently
- [ ] No linting errors/warnings
- [ ] Follows project patterns

## Architecture

- [ ] Clean Architecture boundaries respected
- [ ] Dependencies flow inward
- [ ] No circular dependencies
- [ ] Domain layer is pure

## Security

- [ ] No hardcoded secrets
- [ ] Auth/authz implemented
- [ ] Input validation present
- [ ] Error messages safe

## Testing

- [ ] Unit tests for new logic
- [ ] Integration tests for new endpoints
- [ ] Tests are readable
- [ ] All tests pass

## Documentation

- [ ] XML comments on public APIs
- [ ] README updated if needed
- [ ] ADRs created if needed
```

---

## Common Review Comments

### Readability

**Long Method:**
```
Method is 75 lines. Consider extracting smaller methods with clear names.

Suggestion:
- Extract validation logic → ValidateBrokerInput()
- Extract notification → SendBrokerCreatedNotification()
```

**Magic Number:**
```
What's the significance of 255?

Suggestion:
private const int MaxBrokerNameLength = 255;
```

**Poor Naming:**
```
Variable name "d" is unclear. What does it represent?

Suggestion: Rename to "broker" or "brokerData"
```

---

### Architecture

**Clean Architecture Violation:**
```
Controller is directly accessing DbContext. This violates Clean Architecture.

Fix: Use application service or query handler instead.

// Bad
var broker = await _dbContext.Brokers.FindAsync(id);

// Good
var broker = await _getBrokerHandler.Handle(new GetBrokerQuery(id));
```

---

### Testing

**Missing Tests:**
```
No unit tests found for UpdateBrokerHandler.

Required tests:
- Happy path (successful update)
- Validation errors
- Not found (broker doesn't exist)
- Authorization (unauthorized user)
```

**Poor Test Name:**
```
Test name "Test1" doesn't describe what is tested.

Suggestion: "CreateBroker_ValidData_ReturnsBrokerId"
```

---

### Security

**Missing Authorization:**
```
Authorization check missing. Any authenticated user can update any broker.

Required fix:
var authResult = await _authorizationService.AuthorizeAsync(
    User, brokerId, "UpdateBroker");

if (!authResult.Succeeded)
    return Forbid();
```

---

## Tools & Commands

### Check PR Size

```bash
# Get PR diff stats
git diff --stat origin/main...feature-branch

# Count lines changed
git diff origin/main...feature-branch | wc -l
```

### Check Test Coverage

```bash
# .NET
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report

# Node.js
npm test -- --coverage
```

### Check Linting

```bash
# .NET
dotnet format --verify-no-changes

# Node.js / React
npm run lint
```

### Run Tests Locally

```bash
# Backend
cd src/BrokerHub.Api
dotnet test

# Frontend
cd brokerhub-ui
npm test
```

---

## Definition of Done

### Code Review Complete

- [ ] All files reviewed
- [ ] All tests reviewed
- [ ] Feedback provided
- [ ] Approval status set

### Code Approved for Merge

- [ ] Code quality meets standards
- [ ] All tests pass
- [ ] Architecture compliance verified
- [ ] No critical issues
- [ ] Documentation adequate

---

## Handoff to Merge

### Approval Checklist

Before approving:
- [ ] All blocking issues resolved
- [ ] Developer responded to all comments
- [ ] Tests pass (re-run if needed)
- [ ] No new issues introduced in fixes
- [ ] Code is ready for production

### After Approval

1. Developer merges PR
2. CI/CD runs (build, test, deploy to dev)
3. QE validates in dev environment
4. Code moves toward production

---

## Troubleshooting

### Large PR (>500 Lines)

**Problem:** PR has 1000+ lines, hard to review

**Solutions:**
- Ask developer to split into smaller PRs
- If unavoidable (e.g., migration, generated code), focus on key areas
- Use multiple review sessions

### Developer Disagrees with Feedback

**Problem:** Developer pushes back on requested changes

**Solutions:**
- Explain reasoning with examples
- Reference project standards or best practices
- Distinguish "must fix" from "suggestion"
- Escalate to Architect if architectural disagreement
- Be open to learning (they might be right!)

### Flaky Tests

**Problem:** Tests pass sometimes, fail sometimes

**Solutions:**
- Request developer fix flaky tests before approval
- Flaky tests erode trust in test suite
- Block merge until tests are reliable

---

## Version History

**Version 1.0** - 2026-01-28 - Initial Code Reviewer agent
- SKILL.md with complete agent specification
- Code review checklist and guides (pending creation)
- Review automation scripts (pending creation)

---

## Next Steps

Ready to review code?

1. Read `SKILL.md` thoroughly
2. Review pull request metadata (title, description, size)
3. Run automated checks (build, tests, linting)
4. Review code using checklist
5. Provide constructive feedback
6. Approve or request changes

**Remember:** Your job is to ensure quality without blocking progress. Be thorough but pragmatic.

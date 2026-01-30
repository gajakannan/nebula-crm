---
template: review-checklist
version: 1.0
applies_to: code-reviewer
---

# Code Review Checklist

## Must Check
- Tests added/updated and passing
- Clean Architecture boundaries respected
- AuthN/AuthZ enforced on mutations
- Error handling consistent
- No secrets or credentials in code

## Should Check
- Readability and naming
- Duplicated logic
- Performance foot-guns (N+1)
- Docs updated if needed

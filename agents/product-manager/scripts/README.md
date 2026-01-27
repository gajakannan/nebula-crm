# Product Manager Scripts

Lightweight automation scripts to support PM workflows.

## Available Scripts

### 1. validate-stories.py

Validates user stories for completeness and quality.

**Purpose:**
- Check story format (As a...I want...So that...)
- Validate required sections exist
- Check INVEST criteria quality
- Identify missing acceptance criteria, edge cases, audit requirements

**Usage:**
```bash
python agents/product-manager/scripts/validate-stories.py planning-mds/stories/S1-create-broker.md
```

**Output:**
```
Validating story: planning-mds/stories/S1-create-broker.md
------------------------------------------------------------

❌ ERRORS (Must Fix):
  1. Missing or malformed user story (As a...I want...So that...)
  2. Missing 'Acceptance Criteria' section

⚠️  WARNINGS (Should Fix):
  1. Missing 'Out of Scope' section - explicitly document what's NOT included
  2. No edge cases or error scenarios documented - consider adding

============================================================
❌ Story validation FAILED with 2 error(s) and 2 warning(s)
```

**Exit Codes:**
- `0` - Validation passed (no errors)
- `1` - Validation failed (errors found)

**Checks Performed:**
- ✅ User story format (As a...I want...So that...)
- ✅ Acceptance criteria exist
- ✅ Data requirements section
- ✅ Dependencies section
- ✅ Out of scope section
- ✅ Definition of done
- ⚠️ INVEST criteria quality
- ⚠️ Vague terms in acceptance criteria
- ⚠️ Missing edge cases
- ⚠️ Missing permission checks
- ⚠️ Missing audit trail (for mutations)

---

### 2. generate-story-index.py

Generates an index/table of contents for all user stories.

**Purpose:**
- Create navigable index of stories
- Group stories by epic
- Summarize by phase and priority
- Auto-link to story files

**Usage:**
```bash
python agents/product-manager/scripts/generate-story-index.py planning-mds/stories/
```

**Output:**
Creates `planning-mds/stories/STORY-INDEX.md` with content like:

```markdown
# User Story Index

**Total Stories:** 12

---

## Epic E1: Broker & MGA Relationship Management

| Story ID | Title | Priority | Phase | Persona |
|----------|-------|----------|-------|---------|
| [S1](./S1-create-broker.md) | Create New Broker | Critical | MVP | Distribution Manager |
| [S2](./S2-search-brokers.md) | Search Brokers | High | MVP | Distribution Manager |

---

## Summary by Phase

| Phase | Count |
|-------|-------|
| MVP | 8 |
| Phase 1 | 4 |

---

## Summary by Priority

| Priority | Count |
|----------|-------|
| Critical | 3 |
| High | 5 |
| Medium | 4 |
```

**Extracted Metadata:**
- Story ID (from `**Story ID:**` or filename)
- Title (from `**Title:**` or first heading)
- Epic (from `**Epic:**`)
- Priority (from `**Priority:**`)
- Phase (from `**Phase:**`)
- Persona (from `**As a**` statement)

---

## Installation Requirements

**Python Version:** Python 3.7+

**Dependencies:** None (uses standard library only)

**Verify Installation:**
```bash
python --version
# Should show Python 3.7 or higher
```

---

## Integration with Workflows

### Pre-Commit Hook (Optional)

Validate stories before committing:

Create `.git/hooks/pre-commit`:
```bash
#!/bin/bash

# Find changed story files
STORY_FILES=$(git diff --cached --name-only --diff-filter=ACM | grep "stories/.*\.md$")

if [ -n "$STORY_FILES" ]; then
    echo "Validating changed stories..."
    for file in $STORY_FILES; do
        python agents/product-manager/scripts/validate-stories.py "$file"
        if [ $? -ne 0 ]; then
            echo "❌ Story validation failed. Commit aborted."
            exit 1
        fi
    done
fi

exit 0
```

Make executable:
```bash
chmod +x .git/hooks/pre-commit
```

---

### CI/CD Integration

Add to GitHub Actions or similar:

```yaml
- name: Validate Stories
  run: |
    for file in planning-mds/stories/*.md; do
      python agents/product-manager/scripts/validate-stories.py "$file" || exit 1
    done

- name: Generate Story Index
  run: |
    python agents/product-manager/scripts/generate-story-index.py planning-mds/stories/
    git add planning-mds/stories/STORY-INDEX.md
    git commit -m "Update story index" || true
```

---

## Extending Scripts

### Adding New Validation Rules

Edit `validate-stories.py`:

```python
def check_custom_rule(self):
    """Add your custom validation rule."""
    if "your-required-term" not in self.content.lower():
        self.warnings.append("Your custom warning message")

# Add to validate() method
def validate(self):
    # ... existing checks
    self.check_custom_rule()  # Add your check
```

### Adding New Index Sections

Edit `generate-story-index.py`:

```python
def generate_index(self):
    # ... existing sections

    # Add your custom section
    lines.append("## Custom Section")
    lines.append("")
    # Your custom content
```

---

## Troubleshooting

### Script Not Executable
```bash
chmod +x agents/product-manager/scripts/*.py
```

### Python Not Found
```bash
# Windows
py -3 agents/product-manager/scripts/validate-stories.py <file>

# Mac/Linux
python3 agents/product-manager/scripts/validate-stories.py <file>
```

### File Encoding Errors
Scripts expect UTF-8 encoding. If you encounter encoding errors:
- Save story files as UTF-8
- Or specify encoding in script (edit script to use `encoding='utf-8'`)

---

## Version History

**Version 1.0** - 2026-01-26 - Initial scripts created
- validate-stories.py: Story validation
- generate-story-index.py: Index generation

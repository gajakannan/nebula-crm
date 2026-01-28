# Architect Scripts

Automation scripts to support architecture validation workflows.

## Available Scripts

### 1. validate-api-contract.py

Validates OpenAPI specifications for completeness and REST compliance.

**Usage:**
```bash
python agents/architect/scripts/validate-api-contract.py planning-mds/api/broker-api.yaml
```

**Checks:**
- Required OpenAPI fields
- REST conventions (no verbs in URLs)
- Response definitions (success + error codes)
- Error contract consistency
- Security definitions
- Schema completeness

### 2. validate-architecture.py

Validates INCEPTION.md Phase B sections (4.1-4.6) for completeness.

**Usage:**
```bash
python agents/architect/scripts/validate-architecture.py planning-mds/INCEPTION.md
```

**Checks:**
- All Phase B sections present
- No TODO placeholders remain
- Key entities defined
- Workflows specified
- Authorization model complete
- NFRs measurable

## Installation

**Python Version:** Python 3.7+

**Dependencies:**
```bash
pip install pyyaml
```

## Integration

Add to CI/CD pipeline:
```yaml
- name: Validate Architecture
  run: |
    python agents/architect/scripts/validate-architecture.py planning-mds/INCEPTION.md
```

## Version History

**Version 1.0** - 2026-01-26 - Initial architect scripts

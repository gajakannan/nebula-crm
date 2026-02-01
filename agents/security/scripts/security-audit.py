#!/usr/bin/env python3
"""
Security Audit Script (Lightweight)

Validates that required security planning artifacts exist and are not empty.
This is a planning-phase check; it does not replace SAST/DAST or dependency scans.

Usage:
    python security-audit.py [path-to-planning-security-dir]
"""

import sys
from pathlib import Path

REQUIRED_FILES = [
    "threat-model.md",
    "authorization-review.md",
    "data-protection.md",
    "secrets-management.md",
    "owasp-top-10-results.md",
]


def is_effectively_empty(path: Path) -> bool:
    content = path.read_text(encoding="utf-8", errors="ignore").strip()
    if not content:
        return True
    # Consider a single heading as empty
    lines = [line for line in content.splitlines() if line.strip()]
    return len(lines) <= 1


def main() -> int:
    base = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("planning-mds/security")
    if not base.exists():
        print(f"❌ Security directory not found: {base}")
        return 1

    errors = []
    warnings = []

    for name in REQUIRED_FILES:
        path = base / name
        if not path.exists():
            errors.append(f"Missing security artifact: {path}")
            continue
        if is_effectively_empty(path):
            warnings.append(f"Security artifact looks empty: {path}")

    if errors:
        print("❌ SECURITY ARTIFACT ERRORS:")
        for item in errors:
            print(f"  - {item}")

    if warnings:
        print("⚠️  SECURITY ARTIFACT WARNINGS:")
        for item in warnings:
            print(f"  - {item}")

    if not errors and not warnings:
        print("✅ Security artifacts present and non-empty.")
        return 0

    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())

#!/usr/bin/env python3
"""
Genericness Validation Script

Prevents solution-specific terms from entering agents/ directory.
Pulls the blocked term list from the domain glossary — no hardcoded
terms in this script.

Usage:
    python validate-genericness.py [--glossary <path>] [--agents-dir <path>]
    python validate-genericness.py
    python validate-genericness.py --glossary planning-mds/domain/insurance-glossary.md
"""

import sys
import io
import re
from pathlib import Path

# Windows cp1252 stdout can't encode emojis found in scanned files.
# Reconfigure stdout/stderr to utf-8 unconditionally — safe on all platforms.
if hasattr(sys.stdout, 'buffer'):
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
if hasattr(sys.stderr, 'buffer'):
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')


def extract_blocked_terms(glossary_path: str) -> list:
    """
    Extract blocked terms from the glossary's 'Genericness-Blocked Terms' section.
    Parses bullet-point entries (- Term) within that section only.
    """
    try:
        content = Path(glossary_path).read_text(encoding='utf-8')
    except Exception as e:
        print(f"[ERROR] Could not read glossary at '{glossary_path}': {e}")
        return []

    terms = []
    in_section = False

    for line in content.split('\n'):
        stripped = line.strip()

        # Detect the target section heading
        if re.match(r'^##\s+Genericness-Blocked Terms', stripped):
            in_section = True
            continue

        # Exit on the next ## section heading
        if in_section and re.match(r'^##\s+', stripped):
            break

        # Parse bullet entries within the section
        if in_section:
            match = re.match(r'^-\s+(.+)$', stripped)
            if match:
                terms.append(match.group(1).strip())

    return terms


def scan_directory(agents_dir: str, terms: list) -> list:
    """
    Scan agents/ for occurrences of blocked terms (case-insensitive, word-boundary).
    Returns list of (filepath, line_number, line_content) tuples.
    """
    agents_path = Path(agents_dir)
    if not agents_path.is_dir():
        print(f"[ERROR] Directory not found: {agents_dir}")
        return []

    # Case-insensitive word-boundary pattern
    pattern = re.compile(
        r'\b(' + '|'.join(re.escape(t) for t in terms) + r')\b',
        re.IGNORECASE
    )

    # Files explicitly allowed to contain blocked terms
    skip_files = {'TECH-STACK-ADAPTATION.md'}

    # Phrases that make a blocked-term match a legitimate false positive.
    # If ANY of these substrings appears in a line (case-insensitive), skip it.
    exception_phrases = [
        'pact broker',              # Pact Broker = contract-testing tool
        'form submission',          # generic web concept
        'content security policy',  # web security standard (CSP)
        'casbin policy',            # authorization policy (Casbin)
        'abac policy',              # authorization policy
        'policy file',              # Casbin policy file
        'policy matches',           # authorization rule evaluation
        'pseudo-policy',            # generic auth example
        'policy examples',          # generic auth documentation
        'token renewal',            # OAuth token refresh
    ]

    # Scan all text files in agents/
    extensions = {'.md', '.py', '.sh', '.yaml', '.yml'}
    violations = []

    for file_path in sorted(agents_path.rglob('*')):
        if not file_path.is_file():
            continue
        if file_path.suffix not in extensions:
            continue
        if file_path.name in skip_files:
            continue
        if '.git' in file_path.parts:
            continue

        try:
            lines = file_path.read_text(encoding='utf-8').splitlines()
        except Exception:
            continue

        for line_num, line in enumerate(lines, start=1):
            if pattern.search(line):
                line_lower = line.lower()
                if any(ep in line_lower for ep in exception_phrases):
                    continue
                violations.append((str(file_path), line_num, line.strip()))

    return violations


def main():
    import argparse

    parser = argparse.ArgumentParser(
        description='Validate that agents/ contains no solution-specific terms'
    )
    parser.add_argument(
        '--glossary',
        default='planning-mds/domain/insurance-glossary.md',
        help='Path to domain glossary (extracts blocked terms)'
    )
    parser.add_argument(
        '--agents-dir',
        default='agents',
        help='Path to agents directory to scan'
    )
    args = parser.parse_args()

    print(f"Validating genericness of {args.agents_dir}/")
    print("-" * 60)

    # Extract blocked terms from glossary
    blocked_terms = extract_blocked_terms(args.glossary)

    if not blocked_terms:
        print("[ERROR] No blocked terms found. Check --glossary path and 'Genericness-Blocked Terms' section.")
        sys.exit(1)

    print(f"[Terms]  {len(blocked_terms)} blocked term(s): {', '.join(blocked_terms)}")
    print(f"[Source] Term list from:    {args.glossary}\n")

    # Scan
    violations = scan_directory(args.agents_dir, blocked_terms)

    if not violations:
        print("[PASS] agents/ directory is generic — no blocked terms found")
        sys.exit(0)

    # Report violations grouped by file
    print(f"[FAIL] Solution-specific terms found ({len(violations)} hit(s)):\n")

    current_file = None
    for filepath, line_num, line_content in violations:
        if filepath != current_file:
            current_file = filepath
            print(f"  {filepath}")
        print(f"    {line_num}: {line_content}")

    sys.exit(1)


if __name__ == "__main__":
    main()

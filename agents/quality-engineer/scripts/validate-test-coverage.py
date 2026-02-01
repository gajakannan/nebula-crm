#!/usr/bin/env python3
"""
Test Coverage Validation Script

Validates test coverage against a minimum threshold.
Supports lcov (.info) and Cobertura XML formats.

Usage:
    python validate-test-coverage.py <coverage-file> [--min 80]
    python validate-test-coverage.py --auto [--min 80]
"""

import argparse
import sys
from pathlib import Path
import xml.etree.ElementTree as ET

LCOV_CANDIDATES = [
    "coverage/lcov.info",
    "coverage/lcov-report/lcov.info",
    "lcov.info",
]

COBERTURA_CANDIDATES = [
    "coverage.xml",
    "coverage/coverage.xml",
    "cobertura.xml",
]


def parse_lcov(path: Path) -> float:
    """Return line coverage percent from lcov file."""
    total = 0
    hit = 0
    for line in path.read_text(encoding="utf-8", errors="ignore").splitlines():
        if line.startswith("LF:"):
            total += int(line.split(":", 1)[1].strip() or 0)
        elif line.startswith("LH:"):
            hit += int(line.split(":", 1)[1].strip() or 0)
    if total == 0:
        return 0.0
    return (hit / total) * 100.0


def parse_cobertura(path: Path) -> float:
    """Return line coverage percent from Cobertura XML file."""
    tree = ET.parse(path)
    root = tree.getroot()
    line_rate = root.attrib.get("line-rate")
    if line_rate is None:
        return 0.0
    return float(line_rate) * 100.0


def find_auto_file() -> Path | None:
    for candidate in LCOV_CANDIDATES + COBERTURA_CANDIDATES:
        path = Path(candidate)
        if path.exists():
            return path
    return None


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate test coverage.")
    parser.add_argument("coverage_file", nargs="?", help="Path to coverage file")
    parser.add_argument("--min", type=float, default=0.0, help="Minimum coverage percentage")
    parser.add_argument("--auto", action="store_true", help="Auto-detect coverage file")
    args = parser.parse_args()

    if args.auto:
        path = find_auto_file()
        if path is None:
            print("❌ No coverage file found (auto-detect).")
            return 1
    else:
        if not args.coverage_file:
            print("Usage: python validate-test-coverage.py <coverage-file> [--min 80]")
            print("   or: python validate-test-coverage.py --auto [--min 80]")
            return 1
        path = Path(args.coverage_file)
        if not path.exists():
            print(f"❌ Coverage file not found: {path}")
            return 1

    try:
        if path.suffix == ".info":
            coverage = parse_lcov(path)
        elif path.suffix in {".xml"}:
            coverage = parse_cobertura(path)
        else:
            print(f"❌ Unsupported coverage file format: {path.name}")
            return 1
    except Exception as exc:
        print(f"❌ Failed to parse coverage file: {exc}")
        return 1

    print(f"Coverage: {coverage:.2f}% (min {args.min:.2f}%)")
    if coverage < args.min:
        print("❌ Coverage below minimum threshold.")
        return 1

    print("✅ Coverage check passed.")
    return 0


if __name__ == "__main__":
    sys.exit(main())

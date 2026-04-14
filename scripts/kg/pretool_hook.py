#!/usr/bin/env python3
"""Claude Code PreToolUse adapter — thin wrapper around hint.py.

Reads Claude Code hook stdin (JSON with tool_name and tool_input), extracts
the search path, and delegates to hint.py for the actual KG lookup. This
file is Claude Code-specific; other agents should call hint.py directly.

Configured in .claude/settings.json under hooks.PreToolUse.
"""
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
HINT_SCRIPT = SCRIPT_DIR / "hint.py"


def main() -> int:
    try:
        raw = sys.stdin.read()
        if not raw.strip():
            return 0
        payload = json.loads(raw)
    except (json.JSONDecodeError, OSError):
        return 0

    tool_input = payload.get("tool_input", {})
    search_path = tool_input.get("path")
    if not search_path or not isinstance(search_path, str):
        return 0

    result = subprocess.run(
        [sys.executable, str(HINT_SCRIPT), search_path],
        capture_output=True,
        text=True,
        timeout=10,
    )

    if result.stdout.strip():
        print(result.stdout.strip())

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

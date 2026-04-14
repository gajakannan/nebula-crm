#!/usr/bin/env python3
"""Maintain a structured working-state file for long agent sessions.

During long architect or implementation sessions, context compaction can lose
key decisions and progress. This tool maintains a structured YAML file that
captures decisions, files touched, and open questions so post-compaction
recovery reads structured state instead of re-deriving from conversation.

The working-state file is session-scoped (not committed) and its location is
set via --state-file. Agent-agnostic — any coding agent that supports long
sessions can pass its preferred scratch location.

Usage:
    python3 scripts/kg/workstate.py --state-file /tmp/ws.yaml init --role architect --scope F0007
    python3 scripts/kg/workstate.py --state-file /tmp/ws.yaml decision "Added rationale field"
    python3 scripts/kg/workstate.py --state-file /tmp/ws.yaml dump --compact
"""
from __future__ import annotations

import argparse
import json
import sys
from datetime import UTC, datetime
from pathlib import Path
from typing import Any

import yaml

from kg_common import REPO_ROOT, load_bundle, normalize_target_id, repo_relative


def now_iso() -> str:
    return datetime.now(UTC).isoformat(timespec="seconds")


def load_state(path: Path) -> dict[str, Any]:
    if not path.exists():
        return {}
    return yaml.safe_load(path.read_text(encoding="utf-8")) or {}


def save_state(state: dict[str, Any], path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        yaml.safe_dump(state, sort_keys=False, allow_unicode=True, width=120),
        encoding="utf-8",
    )


def cmd_init(args: argparse.Namespace) -> int:
    """Initialize a new working-state file for a session."""
    sf = Path(args.state_file)
    if sf.exists() and not args.force:
        print(
            f"Working state already exists at {sf}. "
            "Use --force to overwrite.",
            file=sys.stderr,
        )
        return 1

    state: dict[str, Any] = {
        "session": {
            "started": now_iso(),
            "role": args.role,
            "scope": args.scope,
        },
        "decisions": [],
        "files_touched": [],
        "open_questions": [],
    }

    # Enrich scope with ontology label if available
    if args.scope:
        try:
            bundle = load_bundle()
            target_id = normalize_target_id(args.scope)
            node = bundle["all_nodes"].get(target_id)
            if node:
                state["session"]["scope_label"] = node.get("label", args.scope)
                state["session"]["scope_id"] = target_id
        except SystemExit:
            pass

    save_state(state, sf)
    print(f"Initialized working state: {sf}")
    return 0


def cmd_decision(args: argparse.Namespace) -> int:
    """Record a decision made during the session."""
    sf = Path(args.state_file)
    state = load_state(sf)
    if not state:
        print("No working state. Run 'init' first.", file=sys.stderr)
        return 1

    entry: dict[str, Any] = {
        "timestamp": now_iso(),
        "summary": args.summary,
    }
    if args.files:
        entry["files_affected"] = [repo_relative(f) for f in args.files]
    if args.rationale:
        entry["rationale"] = args.rationale

    state.setdefault("decisions", []).append(entry)
    save_state(state, sf)
    idx = len(state["decisions"]) - 1
    print(f"Decision #{idx} recorded.")
    return 0


def cmd_touch(args: argparse.Namespace) -> int:
    """Record a file that was read or modified."""
    sf = Path(args.state_file)
    state = load_state(sf)
    if not state:
        print("No working state. Run 'init' first.", file=sys.stderr)
        return 1

    rel_path = repo_relative(args.path)
    action = args.action or "modified"

    # Check if already tracked — update action if needed
    touched = state.setdefault("files_touched", [])
    for entry in touched:
        if entry["path"] == rel_path:
            if entry.get("action") != action:
                entry["action"] = action
                entry["last_touched"] = now_iso()
                save_state(state, sf)
                print(f"Updated {rel_path} -> {action}")
            else:
                entry["last_touched"] = now_iso()
                save_state(state, sf)
                print(f"Refreshed {rel_path}")
            return 0

    touched.append({
        "path": rel_path,
        "action": action,
        "first_touched": now_iso(),
    })
    save_state(state, sf)
    print(f"Tracked {rel_path} ({action})")
    return 0


def cmd_question(args: argparse.Namespace) -> int:
    """Record an open question or blocker."""
    sf = Path(args.state_file)
    state = load_state(sf)
    if not state:
        print("No working state. Run 'init' first.", file=sys.stderr)
        return 1

    entry: dict[str, Any] = {
        "question": args.text,
        "added": now_iso(),
        "resolved": False,
    }
    if args.context:
        entry["context"] = args.context

    questions = state.setdefault("open_questions", [])
    questions.append(entry)
    save_state(state, sf)
    idx = len(questions) - 1
    print(f"Question #{idx} recorded.")
    return 0


def cmd_resolve(args: argparse.Namespace) -> int:
    """Mark an open question as resolved."""
    sf = Path(args.state_file)
    state = load_state(sf)
    if not state:
        print("No working state. Run 'init' first.", file=sys.stderr)
        return 1

    questions = state.get("open_questions", [])
    if args.index >= len(questions):
        print(f"Question #{args.index} does not exist (have {len(questions)}).", file=sys.stderr)
        return 1

    questions[args.index]["resolved"] = True
    questions[args.index]["resolved_at"] = now_iso()
    if args.answer:
        questions[args.index]["answer"] = args.answer
    save_state(state, sf)
    print(f"Question #{args.index} resolved.")
    return 0


def cmd_dump(args: argparse.Namespace) -> int:
    """Dump working state for context recovery."""
    sf = Path(args.state_file)
    state = load_state(sf)
    if not state:
        print("No working state found.", file=sys.stderr)
        return 1

    if args.compact:
        # Compact format for post-compaction recovery: just the essentials
        compact: dict[str, Any] = {
            "session": state.get("session", {}),
        }
        decisions = state.get("decisions", [])
        if decisions:
            compact["decisions"] = [
                d["summary"] for d in decisions
            ]
        files = state.get("files_touched", [])
        if files:
            compact["files_touched"] = [
                f"{f['path']} ({f.get('action', 'touched')})" for f in files
            ]
        open_qs = [
            q for q in state.get("open_questions", [])
            if not q.get("resolved")
        ]
        if open_qs:
            compact["open_questions"] = [q["question"] for q in open_qs]
        resolved_qs = [
            q for q in state.get("open_questions", [])
            if q.get("resolved")
        ]
        if resolved_qs:
            compact["resolved_questions"] = [
                f"{q['question']} -> {q.get('answer', '(resolved)')}"
                for q in resolved_qs
            ]
        state = compact

    if args.json:
        json.dump(state, sys.stdout, indent=2)
        sys.stdout.write("\n")
    else:
        yaml.safe_dump(state, sys.stdout, sort_keys=False, allow_unicode=True, width=120)

    return 0


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Maintain structured working state for long agent sessions."
    )
    parser.add_argument(
        "--state-file",
        required=True,
        help="Path to the working-state YAML file. Agent-agnostic — each tool passes its own scratch location.",
    )
    sub = parser.add_subparsers(dest="command")

    # init
    p_init = sub.add_parser("init", help="Initialize a new working-state file.")
    p_init.add_argument("--role", required=True, help="Agent role (architect, product-manager, etc.).")
    p_init.add_argument("--scope", default=None, help="Feature or story ID (e.g. F0007, F0007-S0003).")
    p_init.add_argument("--force", action="store_true", help="Overwrite existing state.")

    # decision
    p_dec = sub.add_parser("decision", help="Record a decision.")
    p_dec.add_argument("summary", help="One-line decision summary.")
    p_dec.add_argument("--files", nargs="*", help="Files affected by this decision.")
    p_dec.add_argument("--rationale", help="Brief rationale for the decision.")

    # touch
    p_touch = sub.add_parser("touch", help="Record a file read or modified.")
    p_touch.add_argument("path", help="File path.")
    p_touch.add_argument("--action", choices=["read", "modified", "created", "deleted"], default="modified")

    # question
    p_q = sub.add_parser("question", help="Record an open question.")
    p_q.add_argument("text", help="The question.")
    p_q.add_argument("--context", help="Additional context.")

    # resolve
    p_res = sub.add_parser("resolve", help="Mark a question as resolved.")
    p_res.add_argument("index", type=int, help="Question index (from dump output).")
    p_res.add_argument("--answer", help="Resolution answer.")

    # dump
    p_dump = sub.add_parser("dump", help="Dump working state.")
    p_dump.add_argument("--compact", action="store_true", help="Compact format for post-compaction recovery.")
    p_dump.add_argument("--json", action="store_true", help="Output as JSON instead of YAML.")

    args = parser.parse_args()
    if not args.command:
        parser.print_help()
        return 1

    handlers = {
        "init": cmd_init,
        "decision": cmd_decision,
        "touch": cmd_touch,
        "question": cmd_question,
        "resolve": cmd_resolve,
        "dump": cmd_dump,
    }
    return handlers[args.command](args)


if __name__ == "__main__":
    raise SystemExit(main())

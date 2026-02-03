#!/bin/sh
# check-lint.sh — Run linter and formatter checks for the project.
#
# Usage:
#   ./check-lint.sh [--frontend-dir DIR]
#
# Defaults:
#   --frontend-dir  . (current directory; set to the directory containing package.json)
#
# What it checks:
#   Frontend: ESLint (npm run lint) + Prettier format check (npm run format -- --check)
#   Backend:  No linter configured yet — see NOTE below.
#
# Exit codes:
#   0  All checks passed
#   1  One or more checks failed
#   2  Lint tooling not found (package.json or scripts missing)
#
# NOTE (backend):
#   No backend linter is specified in the current project conventions.
#   When one is adopted, add its invocation in the "backend checks" section below.

FRONTEND_DIR="."
FAILED=0

# --- parse arguments ---
while [ $# -gt 0 ]; do
  case "$1" in
    --frontend-dir)  FRONTEND_DIR="$2"; shift 2 ;;
    -h|--help)
      echo "Usage: $0 [--frontend-dir DIR]"
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
  esac
done

# --- helper: run a named check, capture exit code ---
run_check() {
  label="$1"
  shift
  echo "--- ${label} ---"
  "$@"
  rc=$?
  if [ $rc -ne 0 ]; then
    echo "FAIL: ${label} (exit ${rc})"
    FAILED=1
  else
    echo "OK: ${label}"
  fi
}

# =====================================================================
# Frontend checks
# =====================================================================
PKG="${FRONTEND_DIR}/package.json"

if [ ! -f "$PKG" ]; then
  echo "SKIP (frontend): package.json not found at '${FRONTEND_DIR}/' — nothing to lint."
else
  # Verify the expected scripts exist before running them.
  HAS_LINT=$(node -e "const p=require('./${PKG}');process.exit(p.scripts&&p.scripts.lint?0:1)" 2>/dev/null && echo yes || echo no)
  HAS_FORMAT=$(node -e "const p=require('./${PKG}');process.exit(p.scripts&&p.scripts.format?0:1)" 2>/dev/null && echo yes || echo no)

  if [ "$HAS_LINT" = "yes" ]; then
    cd "$FRONTEND_DIR" || exit 1
    run_check "ESLint" npm run lint
    cd - >/dev/null
  else
    echo "SKIP (frontend lint): no 'lint' script in package.json"
  fi

  if [ "$HAS_FORMAT" = "yes" ]; then
    cd "$FRONTEND_DIR" || exit 1
    run_check "Prettier (format check)" npm run format -- --check
    cd - >/dev/null
  else
    echo "SKIP (frontend format): no 'format' script in package.json"
  fi
fi

# =====================================================================
# Backend checks
# =====================================================================
echo "--- Backend linter ---"
echo "SKIP: no backend linter configured. Add one here when the project adopts one."

# =====================================================================
# Summary
# =====================================================================
echo ""
if [ $FAILED -eq 0 ]; then
  echo "All lint checks passed."
  exit 0
else
  echo "One or more lint checks FAILED. See output above."
  exit 1
fi

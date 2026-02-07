#!/bin/sh
# check-accessibility.sh -- Run frontend accessibility checks.
#
# Usage:
#   sh check-accessibility.sh [--frontend-dir DIR] [-- <runner-args>]
#
# Examples:
#   sh check-accessibility.sh
#   sh check-accessibility.sh --frontend-dir experience -- --run
#   A11Y_TEST_CMD="npm run test:a11y -- --run" sh check-accessibility.sh
#
# Behavior:
# - If A11Y_TEST_CMD is set, run that command in FRONTEND_DIR.
# - Otherwise detect package manager and run "test:a11y" or "a11y" npm script.
# - Exits non-zero for missing setup or failed accessibility checks.

FRONTEND_DIR="${FRONTEND_DIR:-experience}"

print_usage() {
  echo "Usage: $0 [--frontend-dir DIR] [-- <runner-args>]"
}

# Parse script options only. Remaining args are forwarded.
while [ $# -gt 0 ]; do
  case "$1" in
    --frontend-dir)
      FRONTEND_DIR="$2"
      shift 2
      ;;
    -h|--help)
      print_usage
      exit 0
      ;;
    --)
      shift
      break
      ;;
    *)
      break
      ;;
  esac
done

if [ ! -d "$FRONTEND_DIR" ]; then
  echo "ERROR: frontend directory not found: $FRONTEND_DIR" >&2
  exit 2
fi

PKG="${FRONTEND_DIR}/package.json"
if [ ! -f "$PKG" ]; then
  echo "ERROR: package.json not found: $PKG" >&2
  exit 2
fi

if [ -n "${A11Y_TEST_CMD:-}" ]; then
  echo "Running accessibility checks via A11Y_TEST_CMD in $FRONTEND_DIR"
  (
    cd "$FRONTEND_DIR" || exit 2
    sh -c "$A11Y_TEST_CMD \"\$@\"" sh "$@"
  )
  exit $?
fi

if grep -Eq '"test:a11y"[[:space:]]*:' "$PKG"; then
  SCRIPT_NAME="test:a11y"
elif grep -Eq '"a11y"[[:space:]]*:' "$PKG"; then
  SCRIPT_NAME="a11y"
else
  echo "ERROR: no accessibility script found in $PKG." >&2
  echo "Expected \"test:a11y\" or \"a11y\", or set A11Y_TEST_CMD." >&2
  exit 2
fi

if [ -f "${FRONTEND_DIR}/pnpm-lock.yaml" ] && command -v pnpm >/dev/null 2>&1; then
  RUNNER="pnpm"
elif [ -f "${FRONTEND_DIR}/yarn.lock" ] && command -v yarn >/dev/null 2>&1; then
  RUNNER="yarn"
elif command -v npm >/dev/null 2>&1; then
  RUNNER="npm"
else
  echo "ERROR: no supported package manager found (pnpm, yarn, npm)." >&2
  exit 2
fi

echo "Running accessibility checks with ${RUNNER} (${SCRIPT_NAME}) in $FRONTEND_DIR"
(
  cd "$FRONTEND_DIR" || exit 2
  case "$RUNNER" in
    pnpm)
      if [ $# -gt 0 ]; then
        pnpm run "$SCRIPT_NAME" -- "$@"
      else
        pnpm run "$SCRIPT_NAME"
      fi
      ;;
    yarn)
      if [ $# -gt 0 ]; then
        yarn run "$SCRIPT_NAME" "$@"
      else
        yarn run "$SCRIPT_NAME"
      fi
      ;;
    npm)
      if [ $# -gt 0 ]; then
        npm run "$SCRIPT_NAME" -- "$@"
      else
        npm run "$SCRIPT_NAME"
      fi
      ;;
  esac
)
exit $?

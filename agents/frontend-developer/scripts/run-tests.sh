#!/bin/sh
# run-tests.sh — Run frontend tests with sensible defaults.
#
# Usage:
#   sh run-tests.sh [--frontend-dir DIR] [-- <test-runner-args>]
#
# Examples:
#   sh run-tests.sh
#   sh run-tests.sh -- --run
#   FRONTEND_TEST_CMD="npm run test:coverage" sh run-tests.sh
#
# Behavior:
# - If FRONTEND_TEST_CMD is set, run that command (and forward args after --).
# - Otherwise auto-detect package manager (pnpm/yarn/npm) and run test script.
# - Exits non-zero on missing setup or test failures.

FRONTEND_DIR="${FRONTEND_DIR:-experience}"

print_usage() {
  echo "Usage: $0 [--frontend-dir DIR] [-- <test-runner-args>]"
}

# Parse script options only; remaining args are forwarded.
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

if [ -n "${FRONTEND_TEST_CMD:-}" ]; then
  echo "Running frontend tests via FRONTEND_TEST_CMD in $FRONTEND_DIR"
  (
    cd "$FRONTEND_DIR" || exit 2
    sh -c "$FRONTEND_TEST_CMD \"\$@\"" sh "$@"
  )
  exit $?
fi

if grep -Eq '"test"[[:space:]]*:' "$PKG"; then
  HAS_TEST_SCRIPT=1
else
  HAS_TEST_SCRIPT=0
fi

if [ "$HAS_TEST_SCRIPT" -ne 1 ]; then
  echo "ERROR: no \"test\" script found in $PKG." >&2
  echo "Set FRONTEND_TEST_CMD to run a custom test command." >&2
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

echo "Running frontend tests with ${RUNNER} in $FRONTEND_DIR"
(
  cd "$FRONTEND_DIR" || exit 2
  case "$RUNNER" in
    pnpm)
      pnpm test "$@"
      ;;
    yarn)
      yarn test "$@"
      ;;
    npm)
      if [ $# -gt 0 ]; then
        npm test -- "$@"
      else
        npm test
      fi
      ;;
  esac
)
exit $?

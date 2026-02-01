#!/bin/sh
# Prevent solution-specific terms from entering agents/

INSURANCE_TERMS='Nebula|broker|MGA|underwriter|policy|premium|claim|insured'

VIOLATIONS=$(rg -n -i -e "$INSURANCE_TERMS" agents/ \
  --glob '!agents/TECH-STACK-ADAPTATION.md' \
  --glob '!**/.git/**' \
  2>/dev/null)

if [ -n "$VIOLATIONS" ]; then
  echo "❌ Solution-specific terms found in agents/:"
  echo "$VIOLATIONS"
  exit 1
fi

echo "✅ agents/ directory is generic"

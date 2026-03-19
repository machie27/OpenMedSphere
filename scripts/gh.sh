#!/usr/bin/env bash
#
# Wrapper for gh CLI. Passes all arguments directly to gh.
# Usage: ./scripts/gh.sh <command> [args...]
#
# Examples:
#   ./scripts/gh.sh label list
#   ./scripts/gh.sh issue view 123
#   ./scripts/gh.sh issue view 123 --comments
#   ./scripts/gh.sh search issues "query" --limit 10
#

set -euo pipefail

if [[ $# -eq 0 ]]; then
  echo "Usage: ./scripts/gh.sh <command> [args...]" >&2
  exit 1
fi

exec gh "$@"

#!/usr/bin/env bash
# Scarica gli artifact di un run di GitHub Actions in results/actions/<data>-<run-id>/.
# Uso: scripts/scarica-run.sh <run-id>      (serve gh autenticato)
set -euo pipefail
cd "$(dirname "$0")/.."
RUN=$1
DATA=$(gh run view "$RUN" --json createdAt -q .createdAt | cut -c1-10)
OUT="results/actions/$DATA-$RUN"
mkdir -p "$OUT"
gh run download "$RUN" --dir "$OUT"
gh run view "$RUN" --json workflowName,headSha,headBranch,status,conclusion,url > "$OUT/run.json"
echo "$OUT"
find "$OUT" -name '*-report-github.md' | sort

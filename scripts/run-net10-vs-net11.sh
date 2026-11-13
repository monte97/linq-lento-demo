#!/usr/bin/env bash
# Stesso codice, due runtime: alterna net10 e net11 classe per classe (stesso ordine temporale).
# Uso: scripts/run-net10-vs-net11.sh [suffisso-cartella]. Risultati in results/net10 e results/net11.
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p results/net10 results/net11
SUF="${1:--cmp11}"
{
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "--- docker ps all'avvio"; docker ps --format '{{.Names}} {{.Status}}'
  echo "--- docker stats all'avvio"; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'
} > "results/ambiente-avvio${SUF}.txt"
for cls in DispatchEndToEnd ScanVsGroupBy ListVsHashSet AllocationsVsForeach TripleNesting; do
  for ver in 10 11; do
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*${cls}Benchmark*" "${cls}${SUF}" > "results/net$ver/${cls}${SUF}.console.log" 2>&1 || echo "FALLITO $cls net$ver"
  done
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> "results/ambiente-avvio${SUF}.txt"

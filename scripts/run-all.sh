#!/usr/bin/env bash
# Esegue tutte le classi di benchmark, alternando net8 e net10 classe per classe
# (stesso ordine temporale per entrambe le versioni). Uso: scripts/run-all.sh [suffisso-cartella]
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p results/net8 results/net10
SUF="${1:-}"
{
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "--- docker ps all'avvio"; docker ps --format '{{.Names}} {{.Status}}'
  echo "--- docker stats all'avvio"; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'
} > "results/ambiente-avvio${SUF}.txt"
for cls in DispatchEndToEnd TripleNesting ScanVsGroupBy ListVsHashSet AsParallel AllocationsVsForeach; do
  for ver in 8 10; do
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*${cls}Benchmark*" "${cls}${SUF}" > "results/net$ver/${cls}${SUF}.console.log" 2>&1 || echo "FALLITO $cls net$ver"
  done
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> "results/ambiente-avvio${SUF}.txt"

#!/usr/bin/env bash
# Secondo giro di misure (2026-10-28): benchmark aggiunti dopo la revisione (R4, R5, R12)
# e due ripetizioni di DispatchEndToEnd per vedere il rumore. Alterna net8 e net10, un container alla volta.
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p results/net8 results/net10
{
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "--- docker ps all'avvio"; docker ps --format '{{.Names}} {{.Status}}'
  echo "--- docker stats all'avvio"; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'
} > results/ambiente-avvio-secondo-giro.txt
run() { # classe suffisso
  for ver in 8 10; do
    echo "=== $1$2 net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*$1Benchmark*" "$1$2" > "results/net$ver/$1$2.console.log" 2>&1 || echo "FALLITO $1 net$ver"
  done
}
run MultipleEnumeration ""
run AnyPosition ""
run AsParallelPost ""
run ListLength ""
run DispatchEndToEnd "-giro2"
run DispatchEndToEnd "-giro3"
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> results/ambiente-avvio-secondo-giro.txt

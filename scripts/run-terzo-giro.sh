#!/usr/bin/env bash
# NON ESEGUITO (2026-10-28): scritto, mai lanciato. Vedi docs/misure/2026-10-28-secondo-giro.md.
# Terzo giro (2026-10-28): controllo di AsParallel (Select parallelo) e costo di costruzione dell'HashSet.
set -euo pipefail
cd "$(dirname "$0")/.."
{ date -u +"inizio %Y-%m-%dT%H:%M:%SZ"; docker ps --format '{{.Names}} {{.Status}}'; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'; } > results/ambiente-avvio-terzo-giro.txt
for cls in AsParallelSelect SetBuild; do
  for ver in 8 10; do
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*${cls}Benchmark*" "$cls" > "results/net$ver/$cls.console.log" 2>&1 || echo "FALLITO $cls net$ver"
  done
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> results/ambiente-avvio-terzo-giro.txt

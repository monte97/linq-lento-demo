#!/usr/bin/env bash
# Giro del 22/11: ZoneCoverage con ClassicHashSet su .NET 11, e ScanVsGroupBy ripetuto
# su .NET 10 e 11 alternati (due run ciascuno) per capire se la regressione a 2000 zone e' vera.
# Toolchain in-process come il 13/11, cosi' i numeri sono confrontabili con -cmp11ip.
set -euo pipefail
cd "$(dirname "$0")/.."
{
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "--- docker ps all'avvio"; docker ps --format '{{.Names}} {{.Status}}'
} > results/ambiente-avvio-2211.txt
echo "=== ZoneCoverage net11 $(date -u +%H:%M:%S)"
scripts/run-docker.sh 11 "*ZoneCoverageBenchmark*" "ZoneCoverage-2211" --inProcess > results/net11/ZoneCoverage-2211.console.log 2>&1 || echo "FALLITO ZoneCoverage"
for run in 1 2; do
  for ver in 10 11; do
    echo "=== ScanVsGroupBy net$ver run$run $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*ScanVsGroupByBenchmark*" "ScanVsGroupBy-2211-r$run" --inProcess > "results/net$ver/ScanVsGroupBy-2211-r$run.console.log" 2>&1 || echo "FALLITO ScanVsGroupBy net$ver run$run"
  done
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> results/ambiente-avvio-2211.txt
echo FINE

#!/usr/bin/env bash
# Stesso codice su .NET 8, 10 e 11, alternati classe per classe (stesso ordine temporale).
# Toolchain in-process per tutte e tre: BenchmarkDotNet 0.15.8 non riconosce net11.0 con la
# toolchain di default (NotImplementedException in DotNetSdkValidator, 13/11/2026).
# Uso: scripts/run-cmp-8-10-11.sh [suffisso]. Risultati in results/net<ver>/<Classe><suffisso>.
set -euo pipefail
cd "$(dirname "$0")/.."
SUF="${1:--cmp11ip}"
mkdir -p results/net8 results/net10 results/net11
{
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "--- docker ps all'avvio"; docker ps --format '{{.Names}} {{.Status}}'
  echo "--- docker stats all'avvio"; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'
} > "results/ambiente-avvio${SUF}.txt"
for cls in DispatchEndToEnd ScanVsGroupBy ListVsHashSet AllocationsVsForeach TripleNesting; do
  for ver in 8 10 11; do
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*${cls}Benchmark*" "${cls}${SUF}" --inProcess > "results/net$ver/${cls}${SUF}.console.log" 2>&1 || echo "FALLITO $cls net$ver"
  done
done
echo "=== ZoneCoverage net11 $(date -u +%H:%M:%S)"
scripts/run-docker.sh 11 "*ZoneCoverageBenchmark*" "ZoneCoverage${SUF}" --inProcess > "results/net11/ZoneCoverage${SUF}.console.log" 2>&1 || echo "FALLITO ZoneCoverage net11"
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> "results/ambiente-avvio${SUF}.txt"

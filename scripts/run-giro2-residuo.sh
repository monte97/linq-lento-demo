#!/usr/bin/env bash
# Misure residue (2026-10-28): una ripetizione di DispatchEndToEnd (suffisso -giro2), net8 poi net10, un container alla volta.
# Tetto: 30 minuti per misura (timeout), dopo si ferma il container.
set -uo pipefail
cd "$(dirname "$0")/.."
for ver in 8 10; do
  echo "=== DispatchEndToEnd-giro2 net$ver inizio $(date -u +%H:%M:%S)Z"
  scripts/run-docker.sh $ver "*DispatchEndToEndBenchmark*" "DispatchEndToEnd-giro2" > "results/net$ver/DispatchEndToEnd-giro2.console.log" 2>&1 &
  pid=$!
  start=$(date +%s)
  while kill -0 $pid 2>/dev/null; do
    if [ $(( $(date +%s) - start )) -gt 1800 ]; then
      echo "TETTO 30 min superato net$ver: docker stop"; docker stop linq-bench-net$ver; break
    fi
    sleep 10
  done
  wait $pid; echo "=== fine net$ver exit=$? dopo $(( $(date +%s) - start )) s"
done
echo "=== tutto finito $(date -u +%H:%M:%S)Z"

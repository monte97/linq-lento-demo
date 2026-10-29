#!/usr/bin/env bash
# Ripetizione di DispatchOpenSlots (net8 poi net10, un container alla volta, tetto 25 min) dopo che nel primo giro
# net8 L@70 aveva StdDev 41 ms con altri container avviati durante il run (stack osservare-a-strati).
set -uo pipefail
cd "$(dirname "$0")/.."
{ date -u +"inizio %Y-%m-%dT%H:%M:%SZ"; docker ps --format '{{.Names}} {{.Status}}'; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'; } > results/ambiente-avvio-assegnamenti-giro2.txt
for ver in 8 10; do
  name="linq-bench-net$ver"
  echo "=== DispatchOpenSlots-giro2 net$ver $(date -u +%H:%M:%S)"
  ( sleep 1500; if docker ps --format '{{.Names}}' | grep -qx "$name"; then echo "TETTO 25 min net$ver, docker stop"; docker stop "$name"; fi ) &
  wd=$!
  CPUS=4 MEM=8g scripts/run-docker.sh $ver '*DispatchOpenSlots*' DispatchOpenSlots-giro2 > "results/net$ver/DispatchOpenSlots-giro2.console.log" 2>&1 || echo "FALLITO o interrotto net$ver"
  kill $wd 2>/dev/null
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> results/ambiente-avvio-assegnamenti-giro2.txt

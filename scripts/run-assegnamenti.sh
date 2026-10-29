#!/usr/bin/env bash
# DispatchOpenSlots (dataset con assegnamenti, @70) su net8 e net10, un container alla volta,
# stessi limiti (CPUS/MEM). Tetto 25 minuti per misura: oltre, docker stop e [dato da misurare].
set -uo pipefail
cd "$(dirname "$0")/.."
{ date -u +"inizio %Y-%m-%dT%H:%M:%SZ"; docker ps --format '{{.Names}} {{.Status}}'; docker stats --no-stream --format '{{.Name}} {{.CPUPerc}}'; } > results/ambiente-avvio-assegnamenti.txt
for ver in 8 10; do
  name="linq-bench-net$ver"
  echo "=== DispatchOpenSlots net$ver $(date -u +%H:%M:%S)"
  ( sleep 1500; if docker ps --format '{{.Names}}' | grep -qx "$name"; then echo "TETTO 25 min net$ver, docker stop"; docker stop "$name"; fi ) &
  wd=$!
  CPUS=4 MEM=8g scripts/run-docker.sh $ver '*DispatchOpenSlots*' DispatchOpenSlots > "results/net$ver/DispatchOpenSlots.console.log" 2>&1 || echo "FALLITO o interrotto net$ver"
  kill $wd 2>/dev/null
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> results/ambiente-avvio-assegnamenti.txt

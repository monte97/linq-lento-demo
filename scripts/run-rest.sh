#!/usr/bin/env bash
# Come run-all.sh ma parte dalle classi indicate (uso: scripts/run-rest.sh Classe1 Classe2 ...)
set -euo pipefail
cd "$(dirname "$0")/.."
for cls in "$@"; do
  for ver in 8 10; do
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    scripts/run-docker.sh $ver "*${cls}Benchmark*" "${cls}" > "results/net$ver/${cls}.console.log" 2>&1 || echo "FALLITO $cls net$ver"
  done
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" > results/fine-resto.txt

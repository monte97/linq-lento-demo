#!/usr/bin/env bash
# Cronometro dal vivo (modalita' palco, host, net10) sul dataset storico e sul profilo con assegnamenti (@70).
# 5 lanci per scala; sensibilita' sulla quota (XL12 con @50 e @90, 3 lanci). Output in results/assegnamenti/cronometro.txt
set -uo pipefail
cd "$(dirname "$0")/.."
OUT=results/assegnamenti/cronometro.txt
dotnet build -c Release -f net10.0 src/LinqDeepDive.Demo -v q > /dev/null
{ date -u +"inizio %Y-%m-%dT%H:%M:%SZ"; echo "host: $(sysctl -n machdep.cpu.brand_string), dotnet $(dotnet --version)"; } > $OUT
run() { # scala lanci
  for i in $(seq 1 $2); do
    echo "\$ dotnet run -c Release -f net10.0 --project src/LinqDeepDive.Demo -- --dispatch $1   (lancio $i)" >> $OUT
    dotnet run --no-build -c Release -f net10.0 --project src/LinqDeepDive.Demo -- --dispatch $1 2>&1 | grep -E "^(Dataset|Before|After|Risultati)" >> $OUT
  done
}
for s in L XL XL12 XL15; do run $s 5; run "$s@70" 5; done
run "XL12@50" 3; run "XL12@90" 3
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> $OUT

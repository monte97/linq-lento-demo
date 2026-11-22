#!/usr/bin/env bash
# Benchmark sul runner GitHub Actions (windows-latest, bash di Git for Windows).
# Stesso schema di scripts/run-cmp-8-10-11.sh: per ogni classe, .NET Framework 4.8, .NET 8, 10 e 11
# uno dopo l'altro, toolchain in-process. ZoneCoverage esiste solo da .NET 11 (FullJoin) e gira solo
# lì; su 4.8 mancano i metodi che usano API nuove (#if !NETFRAMEWORK), quindi si misura il «prima».
# Uso: scripts/run-actions.sh "<classi separate da spazi>" <cartella di uscita>
# Variabili: VERSIONI (default "48 8 10 11"; 48 = .NET Framework 4.8).
set -uo pipefail
cd "$(dirname "$0")/.."
CLASSI=$1; OUT=$2
VERSIONI=${VERSIONI:-"48 8 10 11"}
mkdir -p "$OUT"
esito=0
for cls in $CLASSI; do
  for ver in $VERSIONI; do
    if [ "$ver" = 48 ]; then tfm=net48; else tfm="net$ver.0"; fi
    if [ "$cls" = ZoneCoverage ] && [ "$ver" != 11 ]; then continue; fi
    echo "=== $cls net$ver $(date -u +%H:%M:%S)"
    mkdir -p "$OUT/net$ver"
    LinqTfms="$tfm" dotnet run -c Release -f "$tfm" --project src/LinqDeepDive.Benchmarks -- \
      --filter "*${cls}Benchmark*" --artifacts "$OUT/net$ver/$cls" --inProcess \
      > "$OUT/net$ver/$cls.console.log" 2>&1 || { echo "FALLITO $cls net$ver"; esito=1; }
  done
done
# Stessi dati su tutti i runtime? Il setup del dispatcher stampa l'esito: deve coincidere riga per riga.
grep -H "// Dispatch" "$OUT"/net*/DispatchEndToEnd.console.log > "$OUT/dispatch-esiti.txt" 2>/dev/null || true
exit $esito

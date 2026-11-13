#!/usr/bin/env bash
# Uso: scripts/run-docker.sh <8|10|11> "<filtro BenchmarkDotNet>" <nome-cartella-output> [argomenti extra BDN...]
# Esegue BenchmarkDotNet in Release dentro mcr.microsoft.com/dotnet/sdk:<versione>.
# Stessi limiti per entrambe le versioni: CPUS e MEM (default 4 CPU, 8g).
# Sorgenti montati in sola lettura e copiati nel container; risultati in results/net<versione>/<nome>.
set -euo pipefail
VER="$1"; FILTER="$2"; NAME="$3"; shift 3
CPUS="${CPUS:-4}"; MEM="${MEM:-8g}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$ROOT/results/net$VER"
docker run --rm --name "linq-bench-net$VER" --cpus "$CPUS" --memory "$MEM" --memory-swap "$MEM" \
  -e LinqTfms=net$VER.0 -e DOTNET_CLI_TELEMETRY_OPTOUT=1 -e DOTNET_NOLOGO=1 \
  -v "$ROOT":/src:ro -v "$ROOT/results/net$VER":/results \
  "mcr.microsoft.com/dotnet/sdk:$VER.0" bash -c "
    set -e
    mkdir /work && cd /src && tar cf - --exclude=bin --exclude=obj --exclude=results --exclude=.git . | tar xf - -C /work
    cd /work
    dotnet run -c Release -f net$VER.0 --project src/LinqDeepDive.Benchmarks -- \
      --filter '$FILTER' --artifacts /results/$NAME $*
  "

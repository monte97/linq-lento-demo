#!/usr/bin/env bash
# Uso: scripts/run-demo-docker.sh <8|10> <argomenti della console Demo...>
# Esegue la console Demo (es. --dispatch L) in un container sdk con gli stessi limiti dei benchmark.
set -euo pipefail
VER="$1"; shift
CPUS="${CPUS:-4}"; MEM="${MEM:-8g}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
docker run --rm --name "linq-demo-net$VER" --cpus "$CPUS" --memory "$MEM" --memory-swap "$MEM" \
  -e LinqTfms=net$VER.0 -e DOTNET_CLI_TELEMETRY_OPTOUT=1 -e DOTNET_NOLOGO=1 \
  -v "$ROOT":/src:ro "mcr.microsoft.com/dotnet/sdk:$VER.0" bash -c "
    set -e
    mkdir /work && cd /src && tar cf - --exclude=bin --exclude=obj --exclude=results --exclude=.git . | tar xf - -C /work
    cd /work
    dotnet build -c Release -f net$VER.0 src/LinqDeepDive.Demo -v q > /dev/null
    dotnet run --no-build -c Release -f net$VER.0 --project src/LinqDeepDive.Demo -- $*
  "

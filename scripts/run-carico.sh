#!/usr/bin/env bash
# Giro di carico: seed, API in Release, k6 su un endpoint alla volta.
# Uso: scripts/run-carico.sh <scala> <zone>      (es. scripts/run-carico.sh L 20)
# Riepiloghi in results/carico/<data>/<endpoint>.json, ambiente in ambiente.txt.
#
# I comandi stanno in variabili, con default Docker per il Mac; su Windows (Task 8)
# si sovrascrivono con PostgreSQL, dotnet e k6 nativi:
#   CARICO_DB_UP / CARICO_DB_DOWN   avvio e arresto del database ("" = già avviato)
#   CARICO_DOTNET                   comando dotnet (default: container sdk:11.0, rete host)
#   CARICO_K6                       comando k6 (default: container grafana/k6, rete host)
#   CARICO_CURL                     comando curl per il controllo di salute (default: container
#                                   in rete host, perché dal Mac la rete host di Docker non si vede)
#   CARICO_CONNECTION               stringa di connessione (default in CaricoConnection.cs)
#   CARICO_PORTA (5080), CARICO_PAUSA (10s fra due endpoint), CARICO_RAMPA (10s),
#   CARICO_DURATA (60s), CARICO_VUS (20), CARICO_OUT (cartella dei risultati),
#   CARICO_RISCALDA (5s: prima delle misure ogni endpoint gira tanto così, senza riepilogo,
#   altrimenti il primo misurato paga JIT e cache fredde: il 22/11 classic e modern, stesso
#   caricamento, differivano di due volte solo per l'ordine)
set -euo pipefail
cd "$(dirname "$0")/.."
[ $# -eq 2 ] || { echo "uso: $0 <scala> <zone>" >&2; exit 2; }
SCALA=$1; ZONE=$2

COMPOSE="docker compose -f docker-compose.carico.yml"
DB_UP=${CARICO_DB_UP-"$COMPOSE up -d --wait"}
DB_DOWN=${CARICO_DB_DOWN-"$COMPOSE down"}
DOTNET=${CARICO_DOTNET:-"docker run --rm -i --network host -v $PWD:/src -v linq-nuget:/root/.nuget/packages -w /src -e LinqTfms -e CARICO_CONNECTION -e ASPNETCORE_URLS mcr.microsoft.com/dotnet/sdk:11.0 dotnet"}
K6=${CARICO_K6:-"docker run --rm -i --network host -v $PWD:/w -w /w -e BASE_URL -e ENDPOINT -e OUT -e VUS -e RAMPA -e DURATA grafana/k6:2.3.0"}
CURL=${CARICO_CURL:-"docker run --rm --network host curlimages/curl:8.10.1"}
PORTA=${CARICO_PORTA:-5080}
PAUSA=${CARICO_PAUSA:-10}
export BASE_URL="http://localhost:$PORTA" ASPNETCORE_URLS="http://localhost:$PORTA"
export VUS=${CARICO_VUS:-20} RAMPA=${CARICO_RAMPA:-10s} DURATA=${CARICO_DURATA:-60s}
[ -n "${CARICO_CONNECTION:-}" ] && export CARICO_CONNECTION
OUTDIR=${CARICO_OUT:-results/carico/$(date -u +%Y-%m-%d)}
PUB=src/LinqDeepDive.Api/bin/carico
# Il Domain è multi-target (LinqTfms): senza questo l'API prende la sua versione net10.0, senza FullJoin
export LinqTfms=net11.0
ENDPOINTS="/coverage/classic /coverage/modern /coverage/db /dispatch/before /dispatch/after"
mkdir -p "$OUTDIR"

API_PID=""
fine() {
  [ -n "$API_PID" ] && kill "$API_PID" 2>/dev/null && wait "$API_PID" 2>/dev/null || true
  [ -n "$DB_DOWN" ] && $DB_DOWN >/dev/null 2>&1 || true
}
trap fine EXIT

[ -n "$DB_UP" ] && $DB_UP
$DOTNET publish src/LinqDeepDive.Api/LinqDeepDive.Api.csproj -c Release -f net11.0 -o "$PUB" --nologo -v q
$DOTNET "$PUB/LinqDeepDive.Api.dll" --seed "$SCALA" "$ZONE" | tee "$OUTDIR/seed.txt"

$DOTNET "$PUB/LinqDeepDive.Api.dll" > "$OUTDIR/api.log" 2>&1 &
API_PID=$!
for _ in $(seq 1 60); do
  $CURL -sf "$BASE_URL/health" >/dev/null && break
  kill -0 "$API_PID" 2>/dev/null || { echo "API terminata, vedi $OUTDIR/api.log" >&2; exit 1; }
  sleep 1
done
$CURL -sf "$BASE_URL/health" >/dev/null || { echo "API non risponde su $BASE_URL" >&2; exit 1; }

{
  echo "scala $SCALA, zone $ZONE"
  date -u +"inizio %Y-%m-%dT%H:%M:%SZ"
  echo "commit $(git rev-parse --short HEAD)$(git diff --quiet || echo ' (modifiche locali)')"
  echo "vus $VUS, rampa $RAMPA, regime $DURATA, pausa ${PAUSA}s, riscaldamento ${CARICO_RISCALDA:-5s} per endpoint"
  echo "dotnet $($DOTNET --version 2>/dev/null | tr -d '\r')"
  echo "k6 $($K6 version 2>/dev/null | head -1 | tr -d '\r')"
  uname -a
} > "$OUTDIR/ambiente.txt"

RISCALDA=${CARICO_RISCALDA:-5s}
for ep in $ENDPOINTS; do
  echo "--- riscaldamento $ep"
  ENDPOINT=$ep OUT= RAMPA=1s DURATA=$RISCALDA $K6 run --quiet load/carico.js >/dev/null || { echo "riscaldamento fallito su $ep" >&2; exit 1; }
done

esito=0
primo=1
for ep in $ENDPOINTS; do
  [ $primo -eq 1 ] || sleep "$PAUSA"
  primo=0
  nome=$(echo "${ep#/}" | tr / -)
  echo "=== $ep $(date -u +%H:%M:%S)"
  ENDPOINT=$ep OUT="$OUTDIR/$nome.json" $K6 run --quiet load/carico.js || { echo "SOGLIA SUPERATA o errore su $ep" >&2; esito=1; }
done
date -u +"fine %Y-%m-%dT%H:%M:%SZ" >> "$OUTDIR/ambiente.txt"
exit $esito

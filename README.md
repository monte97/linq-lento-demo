# linq-lento-demo

Demo del talk "Quando LINQ diventa lento". Repo del talk: `2_repo/2_slides-talks/linq-lento-talk`.

Porting di `999_old/CONTENT/dotnet-linq-demo` (serie LINQ Deep Dive su montelli.dev). Dominio: gestione flotta e assegnamento consegne.

## Struttura

```
src/
  LinqDeepDive.Domain/       Modelli, DataGenerator (Bogus, parametri espliciti), Scenarios/Dispatcher
  LinqDeepDive.Demo/         Console: i quattro errori prima e dopo, anche non interattiva
  LinqDeepDive.Benchmarks/   BenchmarkDotNet 0.15
  LinqDeepDive.Tracing/      OpenTelemetry verso Grafana LGTM (docker compose)
scripts/                     Esecuzione dei benchmark in container
results/net8, results/net10  Artifacts grezzi di BenchmarkDotNet
```

## Target framework

Multi-target `net8.0;net10.0` (`Directory.Build.props`). Un SDK 8 non conosce `net10.0`: per compilare con l'SDK 8 si imposta `LinqTfms=net8.0` (lo fa `scripts/run-docker.sh`).

## Parametri del DataGenerator

`DataGenerator.Generate(new FleetOptions(Vehicles, Deliveries, Restrictions, Zones, Seed))`. Le taglie storiche (`FleetSize.Small/Medium/Large`) hanno 20 zone fisse. Con seed 42 e 20 zone il generatore produce gli stessi dati della versione di 999_old.

`Scales` (nei benchmark) aggiunge due taglie oltre la Large, a zone fisse (20):

| Scala | Veicoli | Consegne | Vincoli |
|---|---|---|---|
| S | 50 | 200 | 100 |
| M | 200 | 2.000 | 1.000 |
| L | 500 | 5.000 | 2.000 |
| XL | 1.000 | 10.000 | 4.000 |
| XXL | 2.000 | 20.000 | 8.000 |

Per `ListVsHashSet` (veicoli e zone) e `ScanVsGroupBy` (zone e consegne) i parametri sono dichiarati con `[Params]` nella classe.

## Benchmark

| Classe | Cosa misura |
|---|---|
| `ListVsHashSetBenchmark` | `List`, array, `HashSet`, `FrozenSet`; famiglie ToList e Count; veicoli x zone |
| `ScanVsGroupByBenchmark` | scansione ripetuta contro `GroupBy`; zone x consegne |
| `TripleNestingBenchmark` | triplo nesting originale contro `ToLookup` |
| `DispatchEndToEndBenchmark` | un giro di dispatch con i quattro pattern, prima e dopo (`Dispatcher`) |
| `AllocationsVsForeachBenchmark` | `ToList` intermedi, `Count() > 0`, `OrderBy().First()`, a coppie per categoria |
| `AsParallelBenchmark` | `AsParallel` sulla versione gia' con `ToLookup` |

Senza `RuntimeMoniker`: il job usa il runtime con cui gira il progetto (`-f net8.0` o `-f net10.0`).

## Come si lancia

Su host (richiede l'SDK installato):

```bash
dotnet run -c Release -f net10.0 --project src/LinqDeepDive.Benchmarks -- --filter '*DispatchEndToEnd*'
dotnet run -f net10.0 --project src/LinqDeepDive.Demo -- --all L      # demo non interattiva, tempi a colpo singolo
```

In container, per confrontare i due runtime nello stesso ambiente:

```bash
CPUS=4 MEM=8g scripts/run-docker.sh 8  '*DispatchEndToEnd*' DispatchEndToEnd
CPUS=4 MEM=8g scripts/run-docker.sh 10 '*DispatchEndToEnd*' DispatchEndToEnd
scripts/run-all.sh        # tutte le classi, alternando net8 e net10
```

I risultati finiscono in `results/net8` e `results/net10`. Il riassunto con ambiente e tabelle sta in `linq-lento-talk/docs/misure/2026-10-28-net8-vs-net10.md`.

## Tracing

```bash
docker compose up -d
dotnet run -f net10.0 --project src/LinqDeepDive.Tracing
docker compose down
```

L'immagine `grafana/otel-lgtm` e' pensata per sviluppo e demo. Il tracing non e' stato rieseguito in questo porting [da verificare].

## Note sul dataset

Con i vincoli di `FleetData` (2.000 vincoli su 20 zone x 3 tipi x 5 fasce) quasi ogni combinazione risulta bloccata: gli assegnamenti del dispatcher sono 0 alle taglie L e oltre. Il comportamento di default non cambia.

Profilo con assegnamenti (opzionale): `FleetOptions.BlockedShare` (es. `0.7`) limita i vincoli al 70% delle 300 combinazioni zona/tipo/fascia, ripetendoli con motivi diversi; il resto resta libero. Dalla console e dai benchmark si usa il suffisso `@<percentuale>` sul nome della scala (`--dispatch XL12@70 --prima`, `Scales.Get("XL12@70")`). Benchmark: `DispatchOpenSlotsBenchmark`. Numeri: `linq-lento-talk/docs/misure/2026-10-29-dataset-con-assegnamenti.md`.

## Carico

Carico su PostgreSQL 17 per misurare quanto della velocita' in memoria resta su un endpoint reale. Database temporaneo (tmpfs, porta 5433), minimal API .NET 11 in `src/LinqDeepDive.Api`.

Versioni: **EF Core 11.0.0**, **Npgsql.EntityFrameworkCore.PostgreSQL 11.0.0** (Npgsql 11.0.0).

```bash
docker compose -f docker-compose.carico.yml up -d
dotnet run --project src/LinqDeepDive.Api -- --seed L 20   # richiede l'SDK 11
docker compose -f docker-compose.carico.yml down
```

`--seed <scala> <zone>` rigenera i dati con `DataGenerator` (stesse scale dei benchmark, `Scales.cs`) e ricarica le tabelle `vehicles`, `drivers`, `deliveries`, `zones`, `restrictions`. Le liste e i record annidati (`TimeSlot`, `DeliveryRequirement`) stanno in colonne `jsonb`; i record del Domain non cambiano. Connessione predefinita `Host=localhost;Port=5433;Database=linq_carico;Username=linq;Password=linq` (override con `CARICO_CONNECTION`).

Verifica di andata e ritorno (container acceso): `dotnet test tests/LinqDeepDive.Api.Tests`.

Report di copertura delle zone, tre varianti con le stesse righe:

| Endpoint | Come |
|---|---|
| `GET /coverage/classic` | righe caricate con EF, `ZoneCoverage.Classic` in memoria |
| `GET /coverage/modern` | righe caricate con EF, `ZoneCoverage.Modern` in memoria |
| `GET /coverage/db` | `GroupBy` + `Count` e `FullJoin` su `IQueryable`, tradotti in SQL (`FULL JOIN`) |

EF Core 11 non traduce `CountBy` su `IQueryable`: `/coverage/db` usa `GroupBy` + `Count`, che produce la stessa SQL. Il test di integrazione confronta le tre varianti riga per riga e controlla che la query di `/coverage/db` contenga `FULL JOIN`.

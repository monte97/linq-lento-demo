#!/usr/bin/env python3
"""Tabella per le slide dai riepiloghi del giro di carico.

Uso: scripts/tabella-carico.py <cartella del giro> [--bdn <cartella dei risultati BenchmarkDotNet>]
     es. scripts/tabella-carico.py results/carico/2026-11-22 --bdn results/net11

Legge <endpoint>.json scritti da scripts/run-carico.sh e scrive <cartella>/tabella.md:
endpoint, p95, richieste al secondo, rapporto contro la variante classica dello stesso
gruppo (coverage-classic, dispatch-before) e, accanto, il tempo medio BenchmarkDotNet
dello stesso codice in memoria, alla stessa scala e con le stesse zone (dal csv più recente
che le contiene). Rapporti come in BenchmarkDotNet: variante / classica, sotto 1 è meglio.
"""
import argparse, csv, glob, json, os, re, sys

UNIT = {"ns": 1e-6, "us": 1e-3, "μs": 1e-3, "ms": 1.0, "s": 1e3}

# endpoint -> (gruppo, classe BenchmarkDotNet, metodo, la classe ha il parametro Zones)
ENDPOINT = {
    "coverage-classic": ("coverage", "ZoneCoverageBenchmark", "Classic", True),
    "coverage-modern": ("coverage", "ZoneCoverageBenchmark", "Modern", True),
    "coverage-db": ("coverage", None, None, True),
    "dispatch-before": ("dispatch", "DispatchEndToEndBenchmark", "Before", False),
    "dispatch-after": ("dispatch", "DispatchEndToEndBenchmark", "After", False),
}
CLASSICA = {"coverage": "coverage-classic", "dispatch": "dispatch-before"}
ZONE_DEFAULT = 20  # FleetOptions.Zones: le classi senza parametro Zones girano con 20


def ms(v):
    n, u = v.replace(",", "").split()
    return float(n) * UNIT[u]


def num(x, cifre=1):
    return f"{x:,.{cifre}f}".replace(",", " ").replace(".", ",").replace(" ", ".")


def tempo(x):
    return f"{num(x * 1e3, 0)} μs" if x < 1 else f"{num(x, 1)} ms"


def bdn(cartella, classe, metodo, scala, zone, ha_zone):
    if not cartella or not classe:
        return None
    if not ha_zone and zone != ZONE_DEFAULT:
        return None
    csvs = glob.glob(os.path.join(cartella, "**", f"*{classe}-report.csv"), recursive=True)
    for p in sorted(csvs, key=os.path.getmtime, reverse=True):
        for r in csv.DictReader(open(p, encoding="utf-8")):
            if r["Method"] == metodo and r.get("Scale") == scala and (not ha_zone or r.get("Zones") == str(zone)):
                return ms(r["Mean"]), os.path.relpath(p, cartella)
    return None


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("giro")
    ap.add_argument("--bdn")
    a = ap.parse_args()

    amb = open(os.path.join(a.giro, "ambiente.txt"), encoding="utf-8").read()
    m = re.search(r"scala (\S+), zone (\d+)", amb)
    if not m:
        sys.exit("ambiente.txt senza 'scala X, zone N'")
    scala, zone = m.group(1), int(m.group(2))

    dati = {}
    for nome in ENDPOINT:
        p = os.path.join(a.giro, f"{nome}.json")
        if os.path.exists(p):
            dati[nome] = json.load(open(p, encoding="utf-8"))["riepilogo"]
    if not dati:
        sys.exit(f"nessun riepilogo in {a.giro}")

    righe = [
        f"Carico a scala {scala} con {zone} zone. {amb.splitlines()[3] if len(amb.splitlines()) > 3 else ''}",
        "",
        "| Endpoint | p95 | Richieste/s | p95 rispetto alla classica | Richieste/s rispetto alla classica | BenchmarkDotNet (media, in memoria) | BDN rispetto alla classica |",
        "|---|---|---|---|---|---|---|",
    ]
    fonti = set()
    for nome, (gruppo, classe, metodo, ha_zone) in ENDPOINT.items():
        if nome not in dati:
            continue
        d, c = dati[nome], dati.get(CLASSICA[gruppo])
        b = bdn(a.bdn, classe, metodo, scala, zone, ha_zone)
        _, classe_c, metodo_c, _ = ENDPOINT[CLASSICA[gruppo]]
        bc = bdn(a.bdn, classe_c, metodo_c, scala, zone, ha_zone)
        if b:
            fonti.add(b[1])
        righe.append("| {} | {} | {} | {} | {} | {} | {} |".format(
            d["endpoint"],
            tempo(d["ms"]["p95"]),
            num(d["richieste_al_secondo"], 0),
            num(d["ms"]["p95"] / c["ms"]["p95"], 2) if c else "n.d.",
            num(d["richieste_al_secondo"] / c["richieste_al_secondo"], 2) if c else "n.d.",
            tempo(b[0]) if b else "n.d.",
            num(b[0] / bc[0], 2) if b and bc else "n.d.",
        ))
        if d.get("errori"):
            righe.append(f"|  | errori {num(d['errori'] * 100, 2)}% |  |  |  |  |  |")
    righe += ["", "Fonti BenchmarkDotNet: " + (", ".join(sorted(fonti)) if fonti else "nessuna")]
    out = os.path.join(a.giro, "tabella.md")
    open(out, "w", encoding="utf-8").write("\n".join(righe) + "\n")
    print(open(out, encoding="utf-8").read())


if __name__ == "__main__":
    main()

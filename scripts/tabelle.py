#!/usr/bin/env python3
"""Tabelle markdown dai csv di BenchmarkDotNet in results/net8 e results/net10.
Uso: scripts/tabelle.py <Classe> [suffisso]   es. scripts/tabelle.py ListLength
I valori sono quelli del csv; i rapporti sono calcolati sulle medie (colonna Mean)."""
import csv, glob, sys, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
UNIT = {"ns": 1.0, "us": 1e3, "μs": 1e3, "ms": 1e6, "s": 1e9}

def ns(v):
    n, u = v.replace(",", "").split()
    return float(n) * UNIT[u]

def fmt(x):
    if x < 1e3: return f"{x:.0f} ns"
    if x < 1e6: return f"{x/1e3:.1f} us"
    if x < 1e9: return f"{x/1e6:.2f} ms"
    return f"{x/1e9:.2f} s"

def rows(ver, cls, suf=""):
    p = glob.glob(f"{ROOT}/results/net{ver}/{cls}{suf}/results/*-report.csv")
    if not p: return []
    return list(csv.DictReader(open(p[0])))

def main():
    cls = sys.argv[1]; suf = sys.argv[2] if len(sys.argv) > 2 else ""
    for ver in (8, 10):
        r = rows(ver, cls, suf)
        if not r: print(f"(nessun csv per net{ver})"); continue
        # parametri = colonne fra 'Categories'/'WarmupCount' e 'Mean'
        keys = list(r[0].keys())
        pcols = keys[keys.index("WarmupCount")+1:keys.index("Mean")]
        pcols = [c for c in pcols if c != "Categories"]
        print(f"\n### net{ver}  ({cls}{suf})\n")
        print("| " + " | ".join(["Metodo"] + pcols + ["Media", "StdDev", "Allocato"]) + " |")
        print("|" + "---|" * (len(pcols) + 4))
        for x in r:
            print("| " + " | ".join([x["Method"]] + [x[c] for c in pcols] + [x["Mean"].replace("μ","u"), x["StdDev"].replace("μ","u"), x["Allocated"]]) + " |")

if __name__ == "__main__":
    main()

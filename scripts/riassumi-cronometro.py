#!/usr/bin/env python3
"""Mediane delle mediane (e min/max delle mediane) per scala dal file del cronometro."""
import re, sys, statistics as st, collections
txt = open(sys.argv[1] if len(sys.argv) > 1 else "results/assegnamenti/cronometro.txt").read()
runs = collections.OrderedDict()
cur = None
for line in txt.splitlines():
    m = re.search(r"--dispatch (\S+)", line)
    if m: cur = m.group(1); runs.setdefault(cur, {"B": [], "A": [], "n": None}); continue
    m = re.match(r"(Before|After ): min ([\d,]+) ms, mediana ([\d,]+) ms", line)
    if m and cur:
        runs[cur]["B" if m.group(1) == "Before" else "A"].append(float(m.group(3).replace(",", ".")))
    m = re.search(r"assegnamenti: (\d+)", line)
    if m and cur: runs[cur]["n"] = int(m.group(1))
print("scala | assegnamenti | lanci | Before: mediana delle mediane (min-max) | After: mediana delle mediane (min-max)")
f = lambda v: f"{st.median(v):.1f} ({min(v):.1f}-{max(v):.1f})".replace(".", ",")
for k, r in runs.items():
    print(f"{k} | {r['n']} | {len(r['B'])} | {f(r['B'])} | {f(r['A'])}")

#!/usr/bin/env python3
"""Punto di pareggio fra List.Contains e un set costruito a ogni chiamata.
Costo di costruzione: results/net<V>/SetBuildCost (SetBuildCostBenchmark).
Costo per ricerca: results/net<V>/ListLength (ListLengthBenchmark), Mean / Vehicles
(ogni misura fa Vehicles ricerche con Count). Pareggio N = costruzione / (ricerca_lista - ricerca_set).
Uso: python3 scripts/pareggio.py"""
import csv, glob, os
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
U = {'ns': 1, 'μs': 1e3, 'us': 1e3, 'ms': 1e6, 's': 1e9}
def ns(s):
    v, u = s.replace(',', '').split(); return float(v) * U[u]
def rows(path):
    return list(csv.DictReader(open(glob.glob(os.path.join(ROOT, path, '*report.csv'))[0])))
for ver in (8, 10):
    ll = rows(f'results/net{ver}/ListLength/results'); sb = rows(f'results/net{ver}/SetBuildCost/results')
    print(f'== .NET {ver} ==')
    print('ListLength | costruz HashSet ns | costruz FrozenSet ns | vehicles | ns/ricerca List | HashSet | Frozen | pareggio HashSet | pareggio FrozenSet')
    for L in (7, 47, 224, 700):
        b = {r['Method']: ns(r['Mean']) for r in sb if int(r['ListLength']) == L}
        for V in (500, 5000):
            m = {r['Method']: ns(r['Mean']) / V for r in ll if int(r['ListLength']) == L and int(r['Vehicles']) == V}
            ph = b['BuildHashSet'] / (m['ListContains'] - m['HashSetContains'])
            pf = b['BuildFrozenSet'] / (m['ListContains'] - m['FrozenSetContains'])
            print(f"{L} | {b['BuildHashSet']:.0f} | {b['BuildFrozenSet']:.0f} | {V} | {m['ListContains']:.2f} | {m['HashSetContains']:.2f} | {m['FrozenSetContains']:.2f} | {ph:.1f} | {pf:.1f}")

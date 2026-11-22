import csv,glob,os,re,sys
root=sys.argv[1]
U={'ns':1e-3,'μs':1,'us':1,'ms':1e3,'s':1e6}
def us(v):
    v=v.replace(',','').strip()
    m=re.match(r'([\d.]+)\s*(\S+)',v); return float(m.group(1))*U[m.group(2)] if m else None
def fmt(x):
    if x is None: return '-'
    if x>=1e6: return f'{x/1e6:.2f} s'
    if x>=1e3: return f'{x/1e3:.1f} ms'
    return f'{x:.1f} μs'
data={}
for f in glob.glob(f'{root}/benchmark-giro-*/net*/*/results/*-report.csv'):
    p=f.split('/'); g=p[-5][-1]; ver=p[-4]; cls=p[-3]
    rows=list(csv.DictReader(open(f,encoding='utf-8')))
    h=list(rows[0].keys()); i0=h.index('Categories')+1 if 'Categories' in h else h.index('Toolchain')+1; i1=h.index('Mean')
    params=[k for k in h[:i1] if k in ('Scale','Zones','ZoneCount','DeliveryCount','VehicleCount')]
    for r in rows:
        key=(cls,r['Method'],' '.join(f'{k}={r[k]}' for k in params))
        data.setdefault(key,{})[(ver,g)]=(us(r['Mean']),r.get('Allocated',''))
V=['net48','net8','net10','net11']
cur=None
for key in sorted(data):
    if key[0]!=cur:
        cur=key[0]; print(f'\n## {cur}\n\n| Metodo | Parametri | '+' | '.join(f'{v} g1 / g2' for v in V)+' | alloc 11 |\n|'+'---|'*(len(V)+3))
    d=data[key]
    cells=[f"{fmt(d.get((v,'1'),(None,))[0])} / {fmt(d.get((v,'2'),(None,))[0])}" for v in V]
    print(f'| {key[1]} | {key[2]} | '+' | '.join(cells)+f" | {d.get(('net11','1'),(0,''))[1]} |")

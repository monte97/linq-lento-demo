import csv,glob,re,sys
root=sys.argv[1]
known=set("Method,Job,AnalyzeLaunchVariance,EvaluateOverhead,MaxAbsoluteError,MaxRelativeError,MinInvokeCount,MinIterationTime,OutlierMode,Affinity,EnvironmentVariables,Jit,LargeAddressAware,Platform,PowerPlanMode,Runtime,AllowVeryLargeObjects,Concurrent,CpuGroups,Force,HeapAffinitizeMask,HeapCount,NoAffinitize,RetainVm,Server,Arguments,BuildConfiguration,Clock,EngineFactory,NuGetReferences,Toolchain,IsMutator,InvocationCount,IterationCount,IterationTime,LaunchCount,MaxIterationCount,MaxWarmupIterationCount,MemoryRandomization,MinIterationCount,MinWarmupIterationCount,RunStrategy,UnrollFactor,WarmupCount,Mean,Error,StdDev,Median,Ratio,RatioSD,Gen0,Gen1,Gen2,Allocated,Alloc Ratio".split(","))
def us(s):
    s=s.replace(",","")
    m=re.match(r"([\d.]+)\s*(ns|μs|us|ms|s)",s); v=float(m.group(1))
    return v*{"ns":1e-3,"μs":1,"us":1,"ms":1e3,"s":1e6}[m.group(2)]
def kb(s):
    s=s.replace(",","")
    if s.strip() in("-",""):return 0
    m=re.match(r"([\d.]+)\s*(B|KB|MB|GB)",s); v=float(m.group(1))
    return v*{"B":1/1024,"KB":1,"MB":1024,"GB":1024**2}[m.group(2)]
def load(v,c):
    f=glob.glob(f"{root}/results/net{v}/{c}-cmp11ip/results/*-report.csv")[0]
    out={}
    for r in csv.DictReader(open(f)):
        params=",".join(f"{k}={r[k]}" for k in r if k not in known)
        out[(r["Method"],params)]=(us(r["Mean"]),us(r["StdDev"]),kb(r["Allocated"]))
    return out
def fmt(x):
    return f"{x/1000:.2f} ms" if x>=1000 else (f"{x:.1f} μs" if x>=1 else f"{x*1000:.0f} ns")
def fk(x): return f"{x/1024:.1f} MB" if x>=1024 else f"{x:.1f} KB"
for c in sys.argv[2:]:
    if c=="ZoneCoverage":
        d=load(11,c); print("##",c)
        for k,(m,s,a) in d.items(): print(f"| {k[0]} | {k[1]} | {fmt(m)} ±{fmt(s)} | {fk(a)} |")
        continue
    d={v:load(v,c) for v in (8,10,11)}
    print("##",c,"| metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11")
    for k in d[8]:
        m8,s8,a8=d[8][k]; m10,_,a10=d[10][k]; m11,s11,a11=d[11][k]
        print(f"| {k[0]} | {k[1]} | {fmt(m8)} | {fmt(m10)} | {fmt(m11)} | {m11/m8:.2f} | {m11/m10:.2f} | {fk(a8)} | {fk(a11)} | {100*s11/m11:.0f}% {100*s8/m8:.0f}%|")

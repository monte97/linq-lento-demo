Carico a scala L con 20 zone. vus 20, rampa 10s, regime 60s, pausa 10s, riscaldamento 5s per endpoint

| Endpoint | p95 | Richieste/s | p95 rispetto alla classica | Richieste/s rispetto alla classica | BenchmarkDotNet (media, in memoria) | BDN rispetto alla classica |
|---|---|---|---|---|---|---|
| /coverage/classic | 250,9 ms | 124 | 1,00 | 1,00 | n.d. | n.d. |
| /coverage/modern | 216,3 ms | 138 | 0,86 | 1,12 | n.d. | n.d. |
| /coverage/db | 11,6 ms | 3.153 | 0,05 | 25,44 | n.d. | n.d. |
| /dispatch/before | 584,4 ms | 47 | 1,00 | 1,00 | n.d. | n.d. |
| /dispatch/after | 279,1 ms | 100 | 0,48 | 2,16 | n.d. | n.d. |

Fonti BenchmarkDotNet: nessuna

Carico a scala L con 20 zone. vus 20, rampa 10s, regime 60s, pausa 10s, riscaldamento 5s per endpoint

| Endpoint | p95 | Richieste/s | p95 rispetto alla classica | Richieste/s rispetto alla classica | BenchmarkDotNet (media, in memoria) | BDN rispetto alla classica |
|---|---|---|---|---|---|---|
| /coverage/classic | 296,5 ms | 94 | 1,00 | 1,00 | n.d. | n.d. |
| /coverage/modern | 285,7 ms | 100 | 0,96 | 1,06 | n.d. | n.d. |
| /coverage/db | 18,9 ms | 2.047 | 0,06 | 21,69 | n.d. | n.d. |
| /dispatch/before | 815,4 ms | 33 | 1,00 | 1,00 | n.d. | n.d. |
| /dispatch/after | 380,0 ms | 73 | 0,47 | 2,17 | n.d. | n.d. |

Fonti BenchmarkDotNet: nessuna

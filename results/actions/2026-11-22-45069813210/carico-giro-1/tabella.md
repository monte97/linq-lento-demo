Carico a scala L con 20 zone. vus 20, rampa 10s, regime 60s, pausa 10s, riscaldamento 5s per endpoint

| Endpoint | p95 | Richieste/s | p95 rispetto alla classica | Richieste/s rispetto alla classica | BenchmarkDotNet (media, in memoria) | BDN rispetto alla classica |
|---|---|---|---|---|---|---|
| /coverage/classic | 433,7 ms | 64 | 1,00 | 1,00 | n.d. | n.d. |
| /coverage/modern | 424,1 ms | 64 | 0,98 | 0,99 | n.d. | n.d. |
| /coverage/db | 27,0 ms | 1.350 | 0,06 | 20,98 | n.d. | n.d. |
| /dispatch/before | 1.394,2 ms | 21 | 1,00 | 1,00 | n.d. | n.d. |
| /dispatch/after | 647,9 ms | 44 | 0,46 | 2,14 | n.d. | n.d. |

Fonti BenchmarkDotNet: nessuna

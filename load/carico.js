// Carico su un endpoint solo: rampa a 20 utenti virtuali, poi 60 secondi a regime.
// Le metriche che contano sono quelle del regime (scenario "regime"): la rampa
// scalda il JIT e il pool di connessioni e resta fuori dai numeri.
// Variabili: BASE_URL, ENDPOINT (es. /coverage/classic), OUT (file JSON del riepilogo),
// VUS (20), RAMPA (10s), DURATA (60s).
import http from 'k6/http';
import { check } from 'k6';

const VUS = parseInt(__ENV.VUS || '20', 10);
const RAMPA = __ENV.RAMPA || '10s';
const DURATA = __ENV.DURATA || '60s';
const URL = `${__ENV.BASE_URL || 'http://localhost:5080'}${__ENV.ENDPOINT}`;

function secondi(d) {
  const m = /^(\d+)(s|m)$/.exec(d);
  if (!m) throw new Error(`durata non valida: ${d}`);
  return parseInt(m[1], 10) * (m[2] === 'm' ? 60 : 1);
}

export const options = {
  summaryTrendStats: ['avg', 'min', 'med', 'max', 'p(90)', 'p(95)', 'p(99)'],
  scenarios: {
    rampa: {
      executor: 'ramping-vus',
      startVUs: 1,
      stages: [{ duration: RAMPA, target: VUS }],
      gracefulRampDown: '0s',
    },
    regime: {
      executor: 'constant-vus',
      vus: VUS,
      duration: DURATA,
      startTime: RAMPA,
    },
  },
  thresholds: {
    // soglia del piano: nessun errore, su tutto il giro
    http_req_failed: ['rate==0'],
    checks: ['rate==1'],
    // soglie sempre vere: servono solo a far comparire le metriche del regime nel riepilogo
    'http_req_duration{scenario:regime}': ['max>=0'],
    'http_reqs{scenario:regime}': ['count>=0'],
  },
};

export default function () {
  const r = http.get(URL);
  check(r, { 'status 200': (x) => x.status === 200 });
}

export function handleSummary(data) {
  const d = data.metrics['http_req_duration{scenario:regime}'].values;
  const n = data.metrics['http_reqs{scenario:regime}'].values.count;
  const riepilogo = {
    endpoint: __ENV.ENDPOINT,
    vus: VUS,
    rampa: RAMPA,
    durata_regime_s: secondi(DURATA),
    richieste: n,
    richieste_al_secondo: n / secondi(DURATA),
    ms: { p50: d.med, p90: d['p(90)'], p95: d['p(95)'], p99: d['p(99)'], media: d.avg, max: d.max },
    errori: data.metrics.http_req_failed.values.rate,
    soglie_ok: !Object.values(data.metrics).some((m) => m.thresholds && Object.values(m.thresholds).some((t) => !t.ok)),
  };
  const out = { stdout: `${JSON.stringify(riepilogo)}\n` };
  if (__ENV.OUT) out[__ENV.OUT] = JSON.stringify({ riepilogo, k6: data }, null, 2);
  return out;
}

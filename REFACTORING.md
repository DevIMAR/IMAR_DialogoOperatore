# Refactoring Plan - DialogoOperatore

## Completati

- [x] `Operatore.AttivitaAperte` e `MacchineAssegnate` inizializzati a lista vuota
- [x] `CaricamentoAttivitaInBackgroundService.UpdateAttivitaAsync()` scomposto in 6 metodi privati
- [x] Null-check in `AttivitaService.ConfrontaCausaliAttivita()` e `TimbratureService.GetTimbratureOperatore()`
- [x] Fix `IsAttivitaNonSchedulata`: escluse attivita indirette dall'avviso
- [x] Redesign TaskPopup: multi-checkbox, grid raggruppata, follower automatici Asana

---

## P1 - CRITICO: Sicurezza

### Credenziali hardcoded nel codice sorgente
- [ ] `ImarApiClient.cs:124` — Basic auth `user_spc:BsaKqh8YzA%W5pLy` hardcoded
- [ ] `JmesApiClient.cs:16-17` — Token login hardcoded come `const`
- [ ] **Azione**: spostare in `appsettings.json` sezione dedicata, iniettare via `IOptions<T>` o `IConfiguration`

### SQL Injection nelle query AS400
- [ ] `AttivitaService.cs:143,193,367-376` — concatenazione stringhe con valori utente
- [ ] `MacchinaService.cs:30` — stessa vulnerabilita
- [ ] **Azione**: usare query parametrizzate Dapper `@param`

### HttpClient leak — `new HttpClient()` per ogni request
- [ ] `ImarApiClient.cs:23,34,48,63,79,88,107` — crea nuovo HttpClient ad ogni chiamata
- [ ] **Azione**: usare `IHttpClientFactory` con named/typed client

### Config Asana hardcoded
- [ ] `AsanaTaskCompilerHelper.cs:122-127` — Workspace, Project, Followers, Assignee hardcoded
- [ ] **Azione**: spostare in `appsettings.json` sezione `AsanaConfig`

---

## P2 - ALTO: SRP e codice duplicato

### AttivitaService.cs — God Class (566 righe, 7 dipendenze)
- [ ] Estrarre query SQL duplicata 3 volte in metodo condiviso `QueryAttivitaAs400(string whereClause)`
- [ ] Unificare `GetAttivitaOperatoreDellUltimaGiornataNonContabilizzate/Contabilizzate` in metodo generico
- [ ] Scomporre `GetAttivitaOperatoreAperte()` (44 righe, 4 LINQ join) in passi intermedi
- [ ] Dipende da `CaricamentoAttivitaInBackgroundService` concreto → estrarre interfaccia

### Codice duplicato: `CreaEdInviaSegnalazioneDifformita()`
- [ ] `ConfermaCommand.cs:84-111` e `InviaTaskCommand.cs:111-138` — quasi identici
- [ ] Magic number `CostoGestioneDifformita = 5`
- [ ] **Azione**: estrarre in `ISegnalazioneDifformitaHelper` condiviso

### `AggiornaOperatoreSelezionatoAsync()` triplicato
- [ ] `ConfermaOperazioneHelper.cs:166`, `CreaFaseNonPianificataHelper.cs:65`, `IngressoUscitaCommand.cs:165`
- [ ] **Azione**: estrarre in helper/servizio condiviso

### Helper/Command dipendono da ViewModel concreti
- [ ] `ConfermaOperazioneHelper.cs:16` → `AttivitaGridViewModel` concreto
- [ ] `IngressoUscitaCommand.cs:18` → `InfoOperatoreViewModel` concreto
- [ ] **Azione**: passare per observer invece che manipolare direttamente il ViewModel

---

## P3 - MEDIO: Architettura e performance

### DialogoOperatoreObserver — God Object (12 proprietà, 12 eventi)
- [ ] Splittare in sub-observer: `IOperatoreStateObserver`, `IOperazioneStateObserver`, `IUIStateObserver`

### Full table load in memoria
- [ ] `TimbratureService.cs` — `.Get().ToList()` su tabelle intere (AngRes, TblResClk, TblResBrk)
- [ ] `OperatoreService.cs:56-58` — carica tutti gli operatori poi filtra in C#
- [ ] **Azione**: push filtri server-side via IQueryable

### OperatoreService — stato mutabile
- [ ] `OperatoreService.cs:22` — proprietà `Operatore` mutabile in un service scoped
- [ ] **Azione**: rendere stateless, restituire da metodi senza memorizzare

### Business logic in UI layer
- [ ] `PopupConfermaHelper.cs` — validazione precedenza fasi, concorrenza bolla, saldo, quantità
- [ ] **Azione**: estrarre in `IActivityValidationService` in Application layer

### Costruttori con troppi parametri (>5)
- [ ] `ConfermaCommand` (9), `InviaTaskCommand` (9), `IngressoUscitaCommand` (9)
- [ ] `PulsantieraGeneraleViewModel` (9) — cast a ViewModel concreto per event subscribe

### Badge esclusione timbrature hardcoded
- [ ] `TimbratureService.cs:110-113` — 22 badge hardcoded
- [ ] **Azione**: spostare in configurazione o tabella DB

### Event subscription senza unsubscribe
- [ ] `ConfermaCommand`, `InviaTaskCommand` — subscribe in costruttore, no Dispose override

---

## Metriche attuali

| Metrica | Valore | Target |
|---------|--------|--------|
| Credenziali hardcoded | 3 | 0 |
| SQL injection potenziale | 5 query | 0 |
| HttpClient leak | 7 istanze | 0 |
| Servizi >200 righe | 2 | 0 |
| Costruttori >5 param | 4 | 0 |
| Business logic in UI | 2 helper | 0 |
| Codice duplicato | 3 casi | 0 |
| Full table load | 5 query | 0 |
| Violazioni layer | 0 | 0 |

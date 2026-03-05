# Refactoring Plan - DialogoOperatore

## Completati (rischio zero)

- [x] `Operatore.AttivitaAperte` e `MacchineAssegnate` inizializzati a lista vuota (previene NullReferenceException)
- [x] `CaricamentoAttivitaInBackgroundService.UpdateAttivitaAsync()` scomposto da 137 righe in 6 metodi privati
- [x] Null-check in `AttivitaService.ConfrontaCausaliAttivita()` per lista null
- [x] Null-check in `TimbratureService.GetTimbratureOperatore()` per operatore non trovato
- [x] Fix `IsAttivitaNonSchedulata`: escluse attivita indirette dall'avviso

---

## Priorita 1 - Rischio basso (refactoring interni, stessa classe)

### AttivitaService.cs (522 righe, 13+ metodi) - God Class
- [ ] Estrarre query SQL duplicata 3 volte (`OttieniAttivitaApertaDaBolla`, `GetAttivitaPerOdp`, background service) in un metodo condiviso `QueryAttivitaAs400(string filtroOdp)`
- [ ] Scomporre `GetAttivitaOperatoreAperte()` (44 righe, 4 LINQ join) in passi intermedi
- [ ] Unificare `GetAttivitaOperatoreDellUltimaGiornataNonContabilizzate()` e `Contabilizzate()` in metodo generico con parametro tabella

### OperatoreService.cs (223 righe)
- [ ] Scomporre `OttieniOperatoreAsync()` (36 righe) in: `CaricaTimbrature()`, `PopolaDatiOperatore()`, `CalcolaStato()`

---

## Priorita 2 - Rischio medio (spostamento logica tra layer)

### Business logic nei Helpers UI -> Servizi Application

#### PopupConfermaHelper.cs (210 righe) -> `IActivityValidationService`
Contiene regole di validazione che dovrebbero stare nel layer Application/Infrastructure:
- Validazione precedenza fasi
- Rilevamento attivita concorrenti su stessa bolla
- Controllo stato saldo
- Validazione quantita prodotte

**Piano**: Creare `IActivityValidationService` in Application, implementare in Infrastructure, iniettare nell'Helper ridotto.

#### ConfermaOperazioneHelper.cs (159 righe) -> `IActivityOperationService`
E' uno state machine dispatcher con 7 case nel switch. Dovrebbe essere un servizio:
- `BeginWorkAsync()`, `EndWorkAsync()`, `AdvanceAsync()`, `BeginEquipmentAsync()`, `EndEquipmentAsync()`

**Anti-pattern critico**: Dipende da `AttivitaGridViewModel` (concrete UI class nel costruttore). Sostituire con interfaccia observer.

---

## Priorita 3 - Rischio medio-alto (refactoring strutturali)

### ConfermaCommand.cs - 8 parametri nel costruttore
Responsabilita multiple: segnalazioni difformita, assegnazione macchine, orchestrazione conferma.
- [ ] Estrarre logica segnalazioni difformita in servizio dedicato
- [ ] Ridurre a 4-5 parametri

### Codice duplicato: `CreaEdInviaSegnalazioneDifformita()`
Metodo identico in `ConfermaCommand.cs` e `InviaTaskCommand.cs`.
- [ ] Estrarre in `ISegnalazioneDifformitaHelper` condiviso

---

## Note architetturali

### Cosa funziona bene
- Confini dei layer rispettati (nessuna violazione Domain->Infrastructure)
- Repository/UoW pattern corretto
- Observer pattern per stato UI ben implementato
- Dependency direction corretta in tutti i progetti

### Metriche attuali
| Metrica | Valore | Target |
|---------|--------|--------|
| Servizi >200 righe | 2 | 0 |
| Costruttori >5 param | 3 | 0 |
| Business logic in UI | 2 helper | 0 |
| Codice duplicato | 2 casi | 0 |
| Violazioni layer | 0 | 0 |

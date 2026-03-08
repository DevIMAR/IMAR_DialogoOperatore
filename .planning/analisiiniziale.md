# Analisi Iniziale - IMAR DialogoOperatore

**Data**: 2026-02-21
**Solution**: .NET 9 Blazor Server | DevExpress 24.2 | 5 DB Contexts | External APIs

---

## Punti positivi

- Clean Architecture rispettata (Domain pulito, layer separati)
- Pattern ViewModel/Command/Observer coerente e ben strutturato
- `GlobalErrorBoundary` per gestione errori UI
- `ReaderWriterLockSlim` nel BackgroundService per thread-safety sulla cache
- Unit of Work consistente sui 4 contesti EF Core
- `LogoutTimerManager` implementa correttamente il Dispose pattern

---

## CRITICAL - Da risolvere subito

### 1. `JmesApiClient` blocca il thread SignalR con `.GetAwaiter().GetResult()`
**23 occorrenze** di chiamate HTTP sincrone. Ogni operazione (inizio lavoro, pausa, avanzamento) **congela l'UI** dell'operatore per tutta la durata della chiamata HTTP. Con più utenti simultanei → thread pool starvation.

### 2. Memory leak su ~20 componenti Razor
Quasi tutti i componenti sottoscrivono `NotifyStateChanged` ma **non implementano `IDisposable`** per disiscriversi. In Blazor Server i circuiti sono long-lived → i componenti rimossi dal render tree restano in memoria e continuano a ricevere notifiche fantasma.

**Solo 4 componenti** lo fanno correttamente: `MainLayout`, `MessageBoxView`, `FasiAttivitaGrid`, `Clock`.

### 3. Singleton `CaricamentoAttivitaInBackgroundService` cattura servizi da uno scope disposed
Il costruttore crea uno scope, risolve `IJmesApiClient` e `IAs400Repository`, poi **disposa lo scope**. I servizi restano referenziati come campo ma operano su risorse dispose.

---

## HIGH - Performance significative

### 4. `async void` su tutti i Command
`CommandBase.Execute()` ritorna `void` → tutte le implementazioni async sono `async void`. Eccezioni non gestite = crash silenzioso del circuito.

### 5. `Thread.Sleep(50)` su `InfoOperatoreView`
Blocca il thread SignalR ad ogni cambio stato per forzare il focus sul badge.

### 6. `ObserverBase.InvokeAsync` - Dead code
Il `Task.Run` fa sì che `SynchronizationContext.Current` sia **sempre null** nel thread pool → il branch `syncContext.Post` non esegue mai. Gli event handler girano fuori dal contesto Blazor.

### 7. `HttpClient` creati manualmente ovunque
`ImarApiClient` crea `new HttpClient()` per ogni chiamata, `JmesApiClient` ne tiene 2 per scope. Nessun uso di `IHttpClientFactory` → socket exhaustion.

### 8. Nessuna resilience sulle API esterne
Zero timeout, retry, circuit breaker su JMes/IMAR/Morpheus API. Se JMes rallenta → tutto si blocca a catena.

### 9. `SynergyJmesUoW` crea un nuovo repository ad ogni accesso
```csharp
public IGenericRepository<AngBdg> AngBdg => _angBdg ?? new GenericRepository<AngBdg>(_context);
```
I campi `readonly` non vengono mai assegnati → `??` crea **sempre** una nuova istanza.

---

## MEDIUM - Da migliorare

| Issue | Impatto |
|-------|---------|
| Render a cascata: 1 cambio proprietà → 8+ `StateHasChanged` simultanei | UI lenta |
| `DocumentaleView` esegue service call **dentro il render** (non in lifecycle) | 50 chiamate per render griglia |
| `Clock` triggera `StateHasChanged` ogni secondo su tutto il subtree | Render inutili |
| Popup sempre renderizzati anche quando nascosti (7 popup) | Memory/CPU |
| `Max Pool Size=10000` sulla connection string Imar_Produzione | Spreco risorse SQL |
| Domain dipende da `Microsoft.EntityFrameworkCore.SqlServer` | Violazione Clean Architecture |
| Nessun `EnableRetryOnFailure()` sui DbContext | Errori transitori non gestiti |
| Credenziali hardcoded nel codice e in `appsettings.json` committato | Sicurezza |

---

## Piano d'azione suggerito (per impatto)

1. **Rendere `JmesApiClient` completamente async** - il singolo cambio con più impatto sulle performance
2. **Aggiungere `IDisposable` a tutti i componenti** con unsubscribe dagli eventi
3. **Fixare `CaricamentoAttivitaInBackgroundService`** - creare scope nei timer callback, non nel costruttore
4. **Introdurre `IHttpClientFactory`** con Polly per retry/timeout/circuit breaker
5. **Fixare `SynergyJmesUoW`** con il pattern `??=` (assegnamento lazy)
6. **Convertire `CommandBase`** per supportare `Task Execute()` invece di `void`

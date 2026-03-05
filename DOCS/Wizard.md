Documentazione wizard rapidi
Indice
1	Simulatore di wizard rapidi
2	Chiamate dall'esterno
2.1	Inizio attrezzaggio (MesEquipStart)
2.2	Sospendi attrezzaggio (MesEquipSuspension)
2.3	Riprendi attrezzaggio (MesEquipResume)
2.4	Fine attrezzaggio (MesEquipEnd)
2.5	Disattrezza (MesEquipRemove)
2.6	Inizio lavoro (MesWorkStart)
2.7	Sospendi lavoro (MesWorkSuspension)
2.8	Riprendi lavoro (MesWorkResume)
2.9	Inizio lavoro indiretta (MesWorkStartIndirect)
2.10	Fine lavoro indiretta (MesWorkEndIndirect)
2.11	Dichiarazione quantità (MesWorkQuantityDeclaration)
2.12	Fine lavoro (MesWorkEnd)
2.13	Inizio sospensione (MesSuspensionStart)
2.14	Fine sospensione (MesSuspensionEnd)
2.15	Dichiarazione differita (MesWorkDeferred)
2.16	Dichiarazione differita fermo (MesStopDeferred)
2.17	Dichiarazione avanzamento (MesAdvanceDeclaration)
2.18	Inizio pausa (MesBreakStart)
2.19	Fine pausa (MesBreakEnd)
2.20	Entra in squadra (MesJoinTeam)
2.21	Controllo quadratura (MesQuantityBalancingCheck)
2.22	Controllo quantità (MesQuantityCheck)
2.23	Movimentazioni (MesMaterialMovimentation)
2.24	Stampa etichette (MesLabelsPrint)
2.25	Fine fermo macchina manuale (MesManualStopEnd)
2.26	Verifica squadra di lavoro (MesJoinWorkingTeam)
2.27	Scelta squadra base (MesBaseTeamChoice)
2.28	Timbratura automatica (MesAutoClock)
2.29	Dichiarazione avanzamento scarti (MesAdvanceRejectedDeclaration)
2.30	Fine lavoro multiplo (MesWorkEndMultiple)
2.31	Dichiarazione avanzamento multipla (MesAdvanceDeclarationMultiple)
2.32	Ripristino lavori (MesRestoreWorks)
Simulatore di wizard rapidi
I wizard presenti in JMes sono disponibili anche in una versione rapida, e che non richiede l'interazione con l'utente. 

Tramite questa funzionalità, accessibile da JMES → Utilità di sistema → Simulatore wizard rapidi, è possibile simulare l'esecuzione di un wizard rapido fornendo i parametri in input tramite la scrittura di un oggetto JSON.

In questa pagina sono riportati i parametri utilizzabili in input da ogni wizard.

Il simulatore, dopo aver lanciato un wizard, visualizzerà se il flusso è terminato correttamente e visualizzerà gli eventuali messaggi informativi, di avviso e di errore.

Chiamate dall'esterno
Per avviare un wizard rapido è sufficiente una chiamata HTTP di  tipo POST con i seguenti parametri specificati nel payload.

"entity": {
    "paramsIO": {
        "qck": true,
        "param1": 3,
        "param2": "ciao"
    }
}

al seguente URL: http://SERVER_JMES:8080/synergy-ws/ws/spec/sys/wzd/start?token=123456789ABC&wzdCod=MesWorkStart
passando come URL params:

"wzdCod": il codice del wizard (campo WzdCod della tabella DefWzd, riportato tra parentesi accanto al titolo del wizard in questa pagina)
"token": con il token di sessione
 

Qui di seguito sono riportati, per ogni wizard, i parametri da passare in input per utilizzare la modalità rapida.

Inizio attrezzaggio (MesEquipStart)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
"notCod": il numero di bolla singola che si vuole iniziare a lavorare.
"clkMacUid": l'uid della macchina da utilizzare (AngRes.Uid).
"notPlnCod": il codice dell'attività non pianificata.
"notPlnNotCod": il numero di bolla singola da cui si vuole generare la non non pianificata.
"notPlnOdpCod": il numero di ordine di produzione da cui si vuole generare la non pianificata.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "notCod": "021382", "clkMacUid": 10 }

Esempio di payload per attività non pianificata: { "qck": true, "clkBdgCod": "0001", "notPlnCod": "GEN", "notPlnNotCod": "021382", "clkMacUid": 10 } o { "qck": true, "clkBdgCod": "0001", "notPlnCod": "GEN", "notPlnOdpCod": "84774015", "clkMacUid": 10 }

Sospendi attrezzaggio (MesEquipSuspension)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
"sspUid": l'uid della causale di sospensione (AngMesSsp.Uid).
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 2, "sspUid": 1 }

Riprendi attrezzaggio (MesEquipResume)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 2 }

Fine attrezzaggio (MesEquipEnd)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
"producedQuantity": la quantità prodotta da tenere nel contapezzi (se le opzioni macchina sono settate per chiedere la quantità).
"rejectedQuantity": la quantità scarta da tenere nel contapezzi (se le opzioni macchina sono settate per chiedere la quantità).
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 2 }

Disattrezza (MesEquipRemove)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 2 }

Inizio lavoro (MesWorkStart)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"notCod": il numero di bolla singola che si vuole iniziare a lavorare.
"indCod": codice della attività indiretta.
"cnlCod": il codice del canale secondario su cui eseguire la lavorazione; se non specificato, la lavorazione viene eseguita nel canale standard [FACOLTATIVO]
"clkMacUid": l'uid della macchina da utilizzare (AngRes.Uid). [Se cnlCod è valorizzato è obbligatorio, altrimenti è facoltativo]
"notPlnCod": il codice dell'attività non pianificata.
"notPlnNotCod": il numero di bolla singola da cui si vuole generare la non non pianificata.
"notPlnOdpCod": il numero di ordine di produzione da cui si vuole generare la non pianificata.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "notCod": "021382", "clkMacUid": 10 }
Esempio di payload per attività indiretta: { "qck": true, "clkBdgCod": "DAD1", "indCod": "pulizia" }
Esempio di payload per un'aggraffata (solo dalla versione 13.11 e 14): { "qck": true, "clkBdgCod": "DAD1", "stpCod": "458", "clkMacUid": 10 }

Esempio di payload per attività non pianificata: { "qck": true, "clkBdgCod": "0001", "notPlnCod": "GEN", "notPlnNotCod": "021382", "clkMacUid": 10 } o { "qck": true, "clkBdgCod": "0001", "notPlnCod": "GEN", "notPlnOdpCod": "84774015", "clkMacUid": 10 }

Sospendi lavoro (MesWorkSuspension)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
"sspUid": l'uid della causale di sospensione (AngMesSsp.Uid).
"producedQuantity": la quantità prodotta.
"notCompliantQuantity": la quantità non conforme.
"rejectedQuantity": la quantità resa.
"producedQuantity2": la quantità prodotta 2.
"notCompliantQuantity2": la quantità non conforme 2.
"rejectedQuantity2": la quantità resa 2.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "clkDiaOpeUid": 2, "sspUid": 1, "producedQuantity": 1.2 }

Note: il separatore decimale nell'indicare le quantità è il punto. Naturalmente le quantità non sono tutte obbligatorie.

Riprendi lavoro (MesWorkResume)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "clkDiaOpeUid": 2 }

Inizio lavoro indiretta (MesWorkStartIndirect)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"indCod": codice della attività indiretta.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "indCod": "pulizia"}

Fine lavoro indiretta (MesWorkEndIndirect)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per la dichiarazione (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "clkDiaOpeUid": 2}

Dichiarazione quantità (MesWorkQuantityDeclaration)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
"clkDiaOpeUid": l'uid del record di dialogo operatore (MesDiaOpe.Uid).
"producedQuantity": la quantità prodotta.
"notCompliantQuantity": la quantità non conforme.
"rejectedQuantity": la quantità resa.
"producedQuantity2": la quantità prodotta 2.
"notCompliantQuantity2": la quantità non conforme 2.
"rejectedQuantity2": la quantità resa 2.
Esempio di payload: {"qck": true,"clkBdgCod": "0001","clkDiaOpeUid": 2, "producedQuantity": 1.2, "notCompliantQuantity": 2}

Note: il separatore decimale nell'indicare le quantità è il punto. Naturalmente le quantità non sono tutte obbligatorie.
La gestione della causalizzazione degli scarti è disabilitata per il flusso rapido. 

Il wizard rapido di "Dichiarazione quantità" è utilizzabile anche per bolle aggraffate, andando ad aggiungere il parametro "stampledDeclaration" e andando a definire al suo interno le quantità per le singole bolle facenti parte dell'aggraffatura.

Esempio di payload per aggraffatura: {"qck": true,"clkBdgCod": "0001","clkDiaOpeUid": 166, "stampledDeclaration": {"045966": {"producedQuantity": 1.2, "defDecAdv": true}}}

Nota 1: se non viene inserito il flag saldo/acconto al wizard gli arriva null che poi si traduce in acconto. 

Nota 2: se non vengono inserite una o più o nessuna quantità per una delle bolle, le quantità non inserite vengono impostate a 0. 

Fine lavoro (MesWorkEnd)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole utilizzare per chiudere il lavoro (AngBdg.BdgCod).
"clkDiaOpeUid": il campo MesDiaOpe.Uid per cui si vuole effettuare il fine lavoro.
"producedQuantity": la quantità prodotta.
"notCompliantQuantity": la quantità non conforme.
"rejectedQuantity": la quantità resa.
"producedQuantity2": la quantità prodotta 2.
"notCompliantQuantity2": la quantità non conforme 2.
"rejectedQuantity2": la quantità resa 2.
"defDecAdv": booleano per il saldo/acconto.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "clkDiaOpeUid": 3 }
Esempio di payload attività indiretta: { "qck": true, "clkBdgCod": "DAD1", "clkDiaOpeUid": 3 }

Note: il separatore decimale nell'indicare le quantità è il punto. Naturalmente le quantità non sono tutte obbligatorie.

Inizio sospensione (MesSuspensionStart)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
"clkMacUid": l'uid della risorsa di tipo macchina sulla quale eseguire la sospensione (AngRes.Uid).
"sspUid": l'uid della causale sospensione macchina (AngMesSsp.Uid).
Esempio di payload:  { "qck": true, "clkBdgCod": "DAD1", "clkMacUid":50, "sspUid": 1}

Fine sospensione (MesSuspensionEnd)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkMacUid": l'uid della risorsa di tipo macchina sulla quale eseguire la sospensione (AngRes.Uid).
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
Esempio di payload:{ "qck": true, "clkMacUid": 50, "clkBdgCod": "DAD1"}

Dichiarazione differita (MesWorkDeferred)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"notCod": il numero di bolla singola che si vuole iniziare a lavorare.
"indCod": codice della attività indiretta.
"prjTskUid": identificativo task  [UidProgetto#CodiceTask]
"clkMacUid": l'uid della macchina da utilizzare (AngRes.Uid). [facoltativo, eccetto per task di progetto]
"isWrk": booleano per definire se è un lavoro.
"tssStr": data/ora inizio in formato yyyyMMddHHmmss.
"tssEnd": data/ora fine in formato yyyyMMddHHmmss.
"defDecAdv": booleano per il saldo/acconto.
"producedQuantity": la quantità prodotta.
"notCompliantQuantity": la quantità non conforme.
"rejectedQuantity": la quantità resa.
"producedQuantity2": la quantità prodotta 2.
"notCompliantQuantity2": la quantità non conforme 2.
"rejectedQuantity2": la quantità resa 2.
"exeAutoClkIn": se true ed è configurata la timbratura automatica da opzioni gloabale, esegue la timbratura automatica da wizard rapido, altrimenti no.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "notCod": "021382", "clkMacUid": 10, "isWrk": true, "tssStr": "20190805150100", "tssEnd": "20190805150200", "producedQuantity": 1 }
Esempio di payload attività indiretta: { "qck": true, "clkBdgCod": "DAD1", "indCod": "pulizia", "tssStr": "20190805150100", "tssEnd": "20190805150200" }

Dichiarazione differita fermo (MesStopDeferred)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"clkMacUid": uid risorsa macchina oggetto del fermo
"tssStr": data/ora inizio in formato yyyyMMddHHmmss.
"tssEnd": data/ora fine in formato yyyyMMddHHmmss.
"stpUid": uid causale fermo
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "clkMacUid": 50, "tssStr": "20190805150100", "tssEnd": "20190805150200", "stpUid": 1}

Dichiarazione avanzamento (MesAdvanceDeclaration)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"notCod": il numero di bolla singola che si vuole iniziare a lavorare.
"clkMacUid": l'uid della macchina da utilizzare (AngRes.Uid). [FACOLTATIVO]
"tssStr": data/ora inizio in formato yyyyMMddHHmmss.
"defDecAdv": booleano per il saldo/acconto.
"producedQuantity": la quantità prodotta.
"notCompliantQuantity": la quantità non conforme.
"rejectedQuantity": la quantità resa.
"producedQuantity2": la quantità prodotta 2.
"notCompliantQuantity2": la quantità non conforme 2.
"rejectedQuantity2": la quantità resa 2.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "notCod": "021382", "clkMacUid": 10, "tssStr": "20190805150100", "producedQuantity": 1 }

Inizio pausa (MesBreakStart)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
"brkUid": l'uid della causale pausa (AngMesBrk.Uid).
Esempio di payload:  { "qck": true, "clkBdgCod": "0001", "brkUid": 1 }

Note: se si sceglie come clkBdgCod un codice appartenente ad un badge che non ha timbrato l'ingresso, viene restituito un errore.

Fine pausa (MesBreakEnd)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in pausa (AngBdg.BdgCod).
Esempio di payload:  { "qck": true, "clkBdgCod": "0001" }

Note: se si sceglie come clkBdgCod un codice appartenente ad un badge che non ha timbrato l'ingresso, viene restituito un errore.

Entra in squadra (MesJoinTeam)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa che si vuole far entrare in squadra (AngBdg.BdgCod).
"clkDiaOpeUid": il campo MesDiaOpe.Uid in cui si vuole entrare in squadra
Esempio di payload:  { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 1}

Controllo quadratura (MesQuantityBalancingCheck)
Note: in caso di wizard rapido, se appare il warning di quantità compresa nel range di tolleranza, viene aggiunto il warning ma il wizard prosegue.

Controllo quantità (MesQuantityCheck)
Note: in caso di wizard rapido, se durante il controllo quantità fase precedente e controllo sovrapproduzione vengono generati warning, il wizard termina correttamente segnalando il warning, se invece ci sono errori, il wizard esce con errore.

Movimentazioni (MesMaterialMovimentation)
Note: in caso di wizard rapido, il wizard esce e non viene movimentato alcun materiale.

Stampa etichette (MesLabelsPrint)
Note: in caso di wizard rapido, il wizard esce e non viene stampata alcuna etichetta.

Fine fermo macchina manuale (MesManualStopEnd)
Note: in caso di wizard rapido e nel caso ci sia un fermo manuale attivo nella macchina specificata, esce dal wizard dando errore.

Verifica squadra di lavoro (MesJoinWorkingTeam)
Note: in caso di wizard rapido e nel caso in cui esista già una squadra attiva per la bolla in input -> esce con errore.

Scelta squadra base (MesBaseTeamChoice)
Note: in caso di wizard rapido, il flusso esce.

Timbratura automatica (MesAutoClock)
Lista parametri accettati

"bdgCod": il codice badge della risorsa che si vuole far entrare/uscire
"clkSid": boolean che indica se si tratta di un ingresso (true) o di un'uscita (false)
"noChkBrk": nel caso di timbra uscita, se l'operatore è in pausa "noChkBrk": true chiude la pausa ed esegue il timbra uscita, "noChkBrk": false (oppure paramentro noChkBrk non presente) non fa niente
"resLgnUid": l'uid della risorsa di login, non obbligatorio
Esempio di payload:  { "bdgCod": "0001", "clkSid": false, "noChkBrk": true }

Dichiarazione avanzamento scarti (MesAdvanceRejectedDeclaration)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"notCod": il numero di bolla singola che si vuole iniziare a lavorare.
"clkMacUid": l'uid della macchina da utilizzare (AngRes.Uid). [FACOLTATIVO]
"rejectedQuantity": la quantità scarta.
"rejectedQuantity2": la quantità scarta 2.
Esempio di payload: { "qck": true, "clkBdgCod": "DAD1", "notCod": "021382", "clkMacUid": 10, "rejectedQuantity": 1 }

Fine lavoro multiplo (MesWorkEndMultiple)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"clkDiaOpeUid": Uid in formato numerico del dialogo operatore (non obbligatorio).
"clkDecQty": Quantità dichiarazione, di tipo stringa (può essere o QtyZero, QtyRes o QtyCou).
"clkDecAdv": Booleano per parametrizzare la chiusura in acconto o a saldo.
"clkDiaOpeUidLst": Lista dei dialogo operatore degli eventi da chiudere.
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkDiaOpeUid": 37, "clkDecQty": "QtyZero", "clkDecAdv": false, "clkDiaOpeUidLst": "41, 42" }

Dichiarazione avanzamento multipla (MesAdvanceDeclarationMultiple)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"clkManNotUidLst": Lista dei codici bolla, da inserire come stringa (Es: "clkManNotUidLst":"000001, 0000002, 100003").
"clkManNotStpUidLst": Lista dei codici lotto aggraffato, da inserire come stringa (Es: "clkManNotStpUidLst":"140, 141, 142").
"clkDecAdv": Booleano per parametrizzare la chiusura in acconto o a saldo.
Esempio di payload per bolle singole: { "qck": true, "clkBdgCod": "0001", "clkManNotUidLst": "000001, 000002, 000003", "clkDecAdv": true}

Esempio di payload per lotti aggraffati: { "qck": true, "clkBdgCod": "0001", "clkManNotStpUidLst": "140, 141, 142", "clkDecAdv": true}

Ripristino lavori (MesRestoreWorks)
Lista parametri accettati

"qck": true, per specificare che si tratta di un wizard rapido.
"clkBdgCod": il codice badge della risorsa uomo.
"clkEvtUidLst": Lista degli id degli eventi da riaprire, da inserire come stringa.
"clkFlg": Flag per capire in che modo procedere con il restore (1:Se è un batch di timbrature manuali, 2: da un timbratore fisico, 3: da una timbratura manuale, 4: da una richiesta badge) (opzionale: se non viene inserito nulla verranno presi tutti i lavori)
Esempio di payload: { "qck": true, "clkBdgCod": "0001", "clkEvtUidLst": "37, 38", "clkFlg": 1 }
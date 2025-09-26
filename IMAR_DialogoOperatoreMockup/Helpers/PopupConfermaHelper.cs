using IMAR_DialogoOperatore.Application;
using IMAR_DialogoOperatore.Application.Interfaces.Services.Activities;
using IMAR_DialogoOperatore.Domain.Models;
using IMAR_DialogoOperatore.Interfaces.Helpers;
using IMAR_DialogoOperatore.Interfaces.Observers;
using IMAR_DialogoOperatore.ViewModels;

namespace IMAR_DialogoOperatore.Helpers
{
    public class PopupConfermaHelper : IPopupConfermaHelper
    {
        private readonly IAttivitaService _attivitaService;
        private readonly IOperatoreService _operatoreService;
        private readonly IDialogoOperatoreObserver _dialogoOperatoreObserver;
        private readonly ICercaAttivitaObserver _cercaAttivitaObserver;
        private readonly IAvanzamentoObserver _avanzamentoObserver;

        public PopupConfermaHelper(
            IAttivitaService attivitaService,
            IOperatoreService operatoreService,
            IDialogoOperatoreObserver dialogoOperatoreObserver,
            ICercaAttivitaObserver cercaAttivitaObserver,
            IAvanzamentoObserver avanzamentoObserver)
        {
            _attivitaService = attivitaService;
            _operatoreService = operatoreService;
            _dialogoOperatoreObserver = dialogoOperatoreObserver;
            _cercaAttivitaObserver = cercaAttivitaObserver;
            _avanzamentoObserver = avanzamentoObserver;
        }

        public string GetTestoPopup()
        {
            string? operazioneInCorso = _dialogoOperatoreObserver.OperazioneInCorso;
            if (operazioneInCorso == null)
                return string.Empty;

            switch (operazioneInCorso)
            {
                case Costanti.INIZIO_LAVORO:
                    return GestisciInizioLavoro();

                case Costanti.INIZIO_ATTREZZAGGIO:
                    return GestisciInizioAttrezzaggio();

                case Costanti.AVANZAMENTO:
                case Costanti.FINE_LAVORO:
                    return GestisciAvanzamentoOChiusuraLavoro();

                default:
                    return string.Empty;
            }
        }

        private string GestisciInizioLavoro()
        {
            string messaggioPopup = string.Empty;

            messaggioPopup += GestisciCambioDiFaseSelezionata(_cercaAttivitaObserver.FaseCercata);
            messaggioPopup += GestisciLavoroApertoDaAltri();
            messaggioPopup += GestisciAperturaConQtProdottaFasePrecedenteAZero();
            messaggioPopup += GestisciCambioCausale();

            if (messaggioPopup != string.Empty)
                messaggioPopup += "Sei sicuro di voler iniziare questo lavoro?\n";

            return messaggioPopup;
        }

        private string GestisciInizioAttrezzaggio()
        {
            string messaggioPopup = string.Empty;

            messaggioPopup += GestisciCambioDiFaseSelezionata(_cercaAttivitaObserver.FaseCercata);
            messaggioPopup += GestisciCambioCausale();

            if (messaggioPopup != string.Empty)
                messaggioPopup += "Sei sicuro di voler iniziare questo lavoro?\n";

            return messaggioPopup;
        }

        private string GestisciCambioCausale()
        {
            string bollaAttivitaSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata.Bolla;
            Attivita? attivitaSelezionataDellOperatore = _dialogoOperatoreObserver.OperatoreSelezionato.AttivitaAperte.SingleOrDefault(x => x.Bolla == bollaAttivitaSelezionata);
            string operazioneInCorso = _dialogoOperatoreObserver.OperazioneInCorso;

            if (!_attivitaService.ConfrontaCausaliAttivita(_dialogoOperatoreObserver.OperatoreSelezionato.AttivitaAperte, bollaAttivitaSelezionata, operazioneInCorso))
                return string.Empty;

            string causaleAttivitaOperatore = attivitaSelezionataDellOperatore.Causale;
            string causaleAttuale = causaleAttivitaOperatore == Costanti.IN_LAVORO ? Costanti.JMES_IN_LAVORO : Costanti.JMES_IN_ATTREZZAGGIO;
            string causalefutura = operazioneInCorso == Costanti.INIZIO_LAVORO ? Costanti.JMES_IN_LAVORO : Costanti.JMES_IN_ATTREZZAGGIO;

            return "L'attività è attualmente aperta come " + causaleAttuale +
                ". Proseguendo si chiuderà la causale in corso e si aprirà " + causalefutura + ".\n";
        }

        private string GestisciCambioDiFaseSelezionata(string? faseCercata = null)
        {
            if (string.IsNullOrEmpty(faseCercata))
                return string.Empty;

            string faseSelezionata = _dialogoOperatoreObserver.AttivitaSelezionata.Fase;

            if (faseCercata == faseSelezionata)
                return string.Empty;

            return "La fase cercata inizialmente era la " + faseCercata + ", ma è stata selezionata la " + faseSelezionata + ".\n";
        }

        private string GestisciAperturaConQtProdottaFasePrecedenteAZero()
        {
            Attivita? attivitaFasePrecedente = GetAttivitaFasePrecedente();

            if (attivitaFasePrecedente == null || attivitaFasePrecedente.QuantitaProdottaContabilizzata + attivitaFasePrecedente.QuantitaProdottaNonContabilizzata != 0)
                return string.Empty;

            return "La fase precedente a quella in lavorazione ha prodotto " + attivitaFasePrecedente.QuantitaProdotta + " pezzi.\n";
        }

        private Attivita? GetAttivitaFasePrecedente()
        {
            List<Attivita> attivitaConStessoOdp = GetAttivitaConStessoOdp();
            int indiceAttivitaSelezionata = GetIndiceAttivitaSelezionata(attivitaConStessoOdp);

            if (indiceAttivitaSelezionata <= 0)
                return null;

            return attivitaConStessoOdp[indiceAttivitaSelezionata - 1];
        }

        private List<Attivita> GetAttivitaConStessoOdp()
        {
            return _attivitaService.GetAttivitaPerOdp(_dialogoOperatoreObserver.AttivitaSelezionata.Odp)
                    .OrderBy(x => x.Fase)
                    .ToList();
        }

        private int GetIndiceAttivitaSelezionata(List<Attivita> attivitaConStessoOdp)
        {
            for (int i = 0; i < attivitaConStessoOdp.Count(); i++)
            {
                if (attivitaConStessoOdp[i].Bolla == _dialogoOperatoreObserver.AttivitaSelezionata.Bolla)
                {
                    return i;
                }
            }

            return -1;
        }

        private string GestisciLavoroApertoDaAltri()
        {
            string messaggioAttivitaAperta = "Questa fase è già in lavorazione da:\n";

            IList<string>? idJmesOperatoriConStessaBollaAperta = _attivitaService.GetIdOperatoriConBollaAperta(_dialogoOperatoreObserver.AttivitaSelezionata.Bolla);
            if (idJmesOperatoriConStessaBollaAperta.Count == 0)
                return string.Empty;

            Operatore operatoreConAttivitaAperta;
            foreach (string idJmesOperatore in idJmesOperatoriConStessaBollaAperta)
            {
                operatoreConAttivitaAperta = _operatoreService.GetOperatoreDaIdJMes(idJmesOperatore);

                messaggioAttivitaAperta += $"- {operatoreConAttivitaAperta.Nome} {operatoreConAttivitaAperta.Cognome} ({operatoreConAttivitaAperta.Badge})\n";
            }

            return messaggioAttivitaAperta;
        }

        private string GestisciAvanzamentoOChiusuraLavoro()
        {
            string messaggioPopup = string.Empty;
            int quantitaProdottaAggiornata = _dialogoOperatoreObserver.AttivitaSelezionata.QuantitaProdotta + (int)_avanzamentoObserver.QuantitaProdotta;

            messaggioPopup += GestisciCambioDiFaseSelezionata();
            messaggioPopup += GestisciFaseConQtaProdottaMaggioreDiFasePrecedente();
            messaggioPopup += GestisciAvanzamentoFaseChiusaASaldo();

            messaggioPopup += "Stai dichiarando " + _avanzamentoObserver.QuantitaProdotta + " pezzi.\n" +
                                "La quantità totale prodotta per questa fase è " + quantitaProdottaAggiornata + "/" + _dialogoOperatoreObserver.AttivitaSelezionata.QuantitaOrdine + ".\n" +
                                "Sei sicuro di voler continuare?\n";

            return messaggioPopup;
        }

        private string GestisciFaseConQtaProdottaMaggioreDiFasePrecedente()
        {
            int quantitaProdottaAggiornata = _dialogoOperatoreObserver.AttivitaSelezionata.QuantitaProdotta + (int)_avanzamentoObserver.QuantitaProdotta;

            AttivitaViewModel? attivitaFasePrecedente = new AttivitaViewModel(GetAttivitaFasePrecedente());

            if (attivitaFasePrecedente == null || attivitaFasePrecedente.QuantitaProdotta >= quantitaProdottaAggiornata)
                return string.Empty;

            return "La fase precedente a quella in lavorazione ha prodotto " + attivitaFasePrecedente.QuantitaProdotta + " pezzi.\n";
        }

        private string GestisciAvanzamentoFaseChiusaASaldo()
        {
            if (!_dialogoOperatoreObserver.AttivitaSelezionata.SaldoAcconto.Equals(Costanti.SALDO))
                return string.Empty;

            return "La fase è già stata chiusa a saldo.\n";
        }
    }
}

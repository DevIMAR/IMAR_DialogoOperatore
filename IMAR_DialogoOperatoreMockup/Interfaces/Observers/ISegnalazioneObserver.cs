using IMAR_DialogoOperatore.Interfaces.ViewModels;

namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ISegnalazioneObserver
    {
        uint? QuantitaRecuperata { get; set; }
        string Categoria { get; set; }
        bool IsErroreFaseAttuale { get; set; }
        string DescrizioneDifetto { get; set; }
        IDatiAttivitaBase? AttivitaPerSegnalazione { get; set; }
        bool IsPopupVisible { get; set; }
        bool IsConfermato { get; set; }

        event Action OnQuantitaRecuperataChanged;
        event Action OnCategoriaChanged;
        event Action OnIsErroreFaseAttualeChanged;
        event Action OnDescrizioneDifettoChanged;
        event Action OnAttivitaPerSegnalazioneChanged;
        event Action OnIsPopupVisibleChanged;
        event Action OnIsConfermatoChanged;
    }
}

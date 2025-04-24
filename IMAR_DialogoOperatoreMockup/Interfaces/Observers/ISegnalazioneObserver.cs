namespace IMAR_DialogoOperatore.Interfaces.Observers
{
    public interface ISegnalazioneObserver
    {
        uint? QuantitaRecuperata { get; set; }
        string Categoria { get; set; }
        bool IsErroreFaseAttuale { get; set; }
        string DescrizioneDifetto { get; set; }

        event Action OnQuantitaRecuperataChanged;
        event Action OnCategoriaChanged;
        event Action OnIsErroreFaseAttualeChanged;
        event Action OnDescrizioneDifettoChanged;
    }
}

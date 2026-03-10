using IMAR_DialogoOperatore.Interfaces.Services;

namespace IMAR_DialogoOperatore.Services
{
    public class DeepLinkParameters : IDeepLinkParameters
    {
        public int? Badge { get; set; }
        public string? Odp { get; set; }
        public string? Fase { get; set; }
        public bool HasDeepLink => Badge.HasValue;

        public void Consume()
        {
            Badge = null;
            Odp = null;
            Fase = null;
        }
    }
}

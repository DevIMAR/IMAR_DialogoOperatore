using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IMAR_DialogoOperatore.Infrastructure.Utilities
{
    public class NetworkShareConnector : IDisposable
    {
        private readonly string _networkPath;
        private bool _disposed;

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        private class NetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string? LocalName;
            public string? RemoteName;
            public string? Comment;
            public string? Provider;
        }

        public NetworkShareConnector(string networkPath, string username, string password)
        {
            _networkPath = networkPath;

            var netResource = new NetResource
            {
                Scope = 0,
                Type = 1, // RESOURCETYPE_DISK
                DisplayType = 0,
                Usage = 0,
                RemoteName = networkPath
            };

            var result = WNetAddConnection2(netResource, password, username, 0);

            if (result != 0)
            {
                throw new Win32Exception(result, $"Errore nella connessione alla share di rete '{networkPath}'. Codice errore: {result}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                WNetCancelConnection2(_networkPath, 0, true);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~NetworkShareConnector()
        {
            Dispose();
        }
    }
}

using System.Threading;
using System.Net;

namespace URL_Shortcut.Utils
{
    public static class AsyncClientSocket
    {
        private static ManualResetEvent waitSignal_onConnect = new ManualResetEvent(false);
        private static ManualResetEvent waitSignal_onSend = new ManualResetEvent(false);
        private static ManualResetEvent waitSignal_onReceive = new ManualResetEvent(false);

        public static void LaunchClient(string ip, int port, string message)
        {

        }
    }
}

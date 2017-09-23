using System.Net.Sockets;
using System.Text;

namespace URL_Shortcut.Utils.Network
{
    public class CommunicationObject
    {
        // Buffer size expected to be 1K bytes
        private const short BUFFER_SIZE = 1024;

        public Socket connection;
        public byte[] buffer;
        public StringBuilder message;

        public CommunicationObject()
        {
            this.connection = null;
            this.buffer = new byte[BUFFER_SIZE];
            this.message = new StringBuilder();
        }
    }
}

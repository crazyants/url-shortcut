using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace URL_Shortcut.Utils.Network
{
    internal static class SyncClientSocket
    {
        private const short BUFFER_SIZE = 1024;

        public static void Transmit(string ip, int port, string message, out string response)
        {
            // Accumulation of responses from the server
            StringBuilder accumulation = new StringBuilder();

            // Create a TCP/IP socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Define the connection to the server
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteServer = new IPEndPoint(ipAddress, port);

                // Trying to connect to the server
                socket.Connect(remoteServer);

                // Prepare the message to be sent
                byte[] packet = Encoding.ASCII.GetBytes(message);

                // Send the message
                int bytesSent = socket.Send(packet);

                // Check if there's anything to be received
                if (socket.Available > 0)
                {
                    // Receive the response
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesReceived = 0;
                    do
                    {
                        bytesReceived = socket.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None, out SocketError errorCode);

                        // Check for failure if there's any
                        if (errorCode != SocketError.Success)
                        {
                            throw new Exception(errorCode.ToString());
                        }

                        // Translate the response
                        accumulation.Append(Encoding.ASCII.GetString(buffer, 0, bytesReceived));

                    } while (bytesReceived > 0);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                // Shutdown the socket
                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(false);
                socket.Close();
            }

            // Return the response
            response = accumulation.ToString();
        }

        private static void Log(string message)
        {
            // Logging...
        }
    }
}

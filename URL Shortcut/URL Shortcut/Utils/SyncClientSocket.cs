using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace URL_Shortcut.Utils
{
    public static class SyncClientSocket
    {
        private const short BUFFER_SIZE = 1024;

        public static string Transmit(string ip, int port, string message)
        {
            // Create a socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // The response to be retrieved
            StringBuilder response = new StringBuilder();

            try
            {
                // Define the connection of the server
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteServer = new IPEndPoint(ipAddress, port);

                // Trying to connect to the server
                socket.Connect(remoteServer);

                // Prepare the message to be sent
                byte[] packet = Encoding.ASCII.GetBytes(message);

                // Send the message
                int bytesSent = socket.Send(packet);

                //if (socket.Available > 0)
                {
                    // Receive the response
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesReceived = 0;
                    do
                    {
                        //bytesReceived = socket.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None, out SocketError errorCode);
                        bytesReceived = socket.Receive(buffer);

                        // Check for failure if there's any
                        //if (errorCode != SocketError.Success)
                        //{
                        //    throw new Exception(errorCode.ToString());
                        //}

                        // Translate the response
                        response.Append(Encoding.ASCII.GetString(buffer, 0, bytesReceived));
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
                socket.Close();
            }

            return response.ToString();
        }

        private static void Log(string message)
        {
            // Logging...
        }
    }
}

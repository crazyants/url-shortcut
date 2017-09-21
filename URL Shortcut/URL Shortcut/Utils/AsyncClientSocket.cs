using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace URL_Shortcut.Utils
{
    internal static class AsyncClientSocket
    {
        // Wait signals to arrange the Connect, Send, and Receive actions
        private static ManualResetEvent waitSignal_onConnect = new ManualResetEvent(false);
        private static ManualResetEvent waitSignal_onSend = new ManualResetEvent(false);
        private static ManualResetEvent waitSignal_onReceive = new ManualResetEvent(false);

        // Response from server to be returned
        private static string receivedMessage = string.Empty;

        public static void Transmit(string ip, int port, string message, out string response)
        {
            // Create a TCP/IP socket
            Socket socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Establish server's end-point
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

                // Connect
                Connect(socket, endPoint);

                // Wait until connected
                waitSignal_onConnect.WaitOne();

                // Send
                Send(socket, message);

                // Wait until sent
                waitSignal_onSend.WaitOne();

                // Receive
                Receive(socket);

                // Wait until received
                waitSignal_onReceive.WaitOne();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                // Return the response
                response = receivedMessage;

                try
                {
                    // Shutdown the socket
                    socket.Shutdown(SocketShutdown.Both);

                    // Disconnect from server
                    Disconnect(socket);

                    // Release the socket
                    socket.Close();
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        private static void Connect(Socket socket, IPEndPoint endPoint)
        {
            try
            {
                // Trying to connect...
                socket.BeginConnect(endPoint, new AsyncCallback(Connected), socket);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Connected(IAsyncResult asyncResult)
        {
            // Cast back the socket
            Socket socket = (Socket)asyncResult.AsyncState;

            try
            {
                // Finalize the connection
                socket.EndConnect(asyncResult);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                // Release the wait-signal
                waitSignal_onConnect.Set();
            }
        }

        private static void Send(Socket socket, string message)
        {
            // Prepare the packet to be sent
            byte[] packet = Encoding.ASCII.GetBytes(message);

            try
            {
                // Trying to send...
                socket.BeginSend(packet, 0, packet.Length, 
                    SocketFlags.None, out SocketError errorCode, 
                    new AsyncCallback(Sent), socket);

                // Log error if there's any
                if (errorCode != SocketError.Success)
                {
                    Log(errorCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Sent(IAsyncResult asyncResult)
        {
            // Cast back the socket
            Socket socket = (Socket)asyncResult.AsyncState;

            try
            {
                // Finalize the sending process
                int bytesSent = socket.EndSend(asyncResult, out SocketError errorCode);

                // Log error if there's any
                if (errorCode != SocketError.Success)
                {
                    Log(errorCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                // Release the wait-signal
                waitSignal_onSend.Set();
            }
        }

        private static void Receive(Socket socket)
        {
            // Create the communication object to handle the continuous retrieval
            CommunicationObject comObj = new CommunicationObject()
            {
                connection = socket
            };

            try
            {
                // Trying to receive...
                comObj.connection.BeginReceive(comObj.buffer, 0, comObj.buffer.Length,
                    SocketFlags.None, out SocketError errorCode,
                    new AsyncCallback(Received), comObj);

                // Log error if there's any
                if (errorCode != SocketError.Success)
                {
                    Log(errorCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Received(IAsyncResult asyncResult)
        {
            // Cast back the communication object
            CommunicationObject comObj = (CommunicationObject)asyncResult.AsyncState;

            try
            {
                // Finalize the receiving process
                int bytesReceived;
                bytesReceived = comObj.connection.EndReceive(asyncResult, out SocketError errorCodeER);

                // Log error if there's any
                if (errorCodeER != SocketError.Success)
                {
                    Log(errorCodeER.ToString());
                }

                // Check if anything received
                if (bytesReceived > 0)
                {
                    // Save whatever is received so far as there might be more to receive
                    string chunk = Encoding.ASCII.GetString(comObj.buffer, 0, comObj.buffer.Length);
                    comObj.message.Append(chunk);

                    // Receive more
                    comObj.connection.BeginReceive(comObj.buffer, 0, comObj.buffer.Length,
                        SocketFlags.None, out SocketError errorCodeBR,
                        new AsyncCallback(Received), comObj);

                    // Log error if there's any
                    if (errorCodeBR != SocketError.Success)
                    {
                        Log(errorCodeBR.ToString());
                    }
                } else {
                    // Entire message has been received
                    if (comObj.message.Length > 0)
                    {
                        receivedMessage = comObj.message.ToString();
                    }

                    // Release the wait-signal
                    waitSignal_onReceive.Set();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());

                // Release the wait-signal upon any exception
                waitSignal_onReceive.Set();
            }
        }

        private static void Disconnect(Socket socket)
        {
            try
            {
                // Trying to disconnect...
                socket.BeginDisconnect(false, new AsyncCallback(Disconnected), socket);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Disconnected(IAsyncResult asyncResult)
        {
            // Cast back the communication object
            CommunicationObject comObj = (CommunicationObject)asyncResult.AsyncState;

            try
            {
                // Finalize the disconnection
                comObj.connection.EndDisconnect(asyncResult);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Log(string message)
        {
            // Logging...
        }
    }
}

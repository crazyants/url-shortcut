using System.Threading;
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;

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
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
            try
            {
                // Cast back the socket
                Socket socket = (Socket)asyncResult.AsyncState;

                // Finalize the connection
                socket.EndConnect(asyncResult);

                // Release the wait-signal
                waitSignal_onConnect.Set();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Send(Socket socket, string message)
        {
            try
            {
                // Prepare the packet to be sent
                byte[] packet = Encoding.ASCII.GetBytes(message);

                // Trying to send...
                socket.BeginSend(packet, 0, packet.Length, 
                    SocketFlags.None, out SocketError errorCode, 
                    new AsyncCallback(Sent), socket);

                // Check for error if there's any
                if (errorCode != SocketError.Success)
                {
                    throw new Exception(errorCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Sent(IAsyncResult asyncResult)
        {
            try
            {
                // Cast back the socket
                Socket socket = (Socket)asyncResult.AsyncState;

                // Finalize the sending process
                int bytesSent = socket.EndSend(asyncResult, out SocketError errorCode);

                // Check if there was any error
                if (errorCode != SocketError.Success)
                {
                    throw new Exception(errorCode.ToString());
                }

                // Release the wait-signal
                waitSignal_onSend.Set();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Receive(Socket socket)
        {
            try
            {
                // Create the state object
                CommunicationObject comObj = new CommunicationObject()
                {
                    connection = socket
                };

                // Trying to receive...
                socket.BeginReceive(comObj.buffer, 0, comObj.buffer.Length,
                    SocketFlags.None, out SocketError errorCode,
                    new AsyncCallback(Received), comObj);

                // Check for error
                if (errorCode != SocketError.Success)
                {
                    throw new Exception(errorCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void Received(IAsyncResult asyncResult)
        {
            try
            {
                // Cast back the state object
                CommunicationObject comObj = (CommunicationObject)asyncResult.AsyncState;

                // Finalize the receiving process
                int bytesReceived;
                bytesReceived = comObj.connection.EndReceive(asyncResult, out SocketError errorCodeER);

                // Check if received successfully
                if (errorCodeER != SocketError.Success)
                {
                    throw new Exception(errorCodeER.ToString());
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

                    // Check for error
                    if (errorCodeBR != SocketError.Success)
                    {
                        throw new Exception(errorCodeBR.ToString());
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
            }
        }

        private static void Log(string message)
        {
            // Logging...
        }
    }
}

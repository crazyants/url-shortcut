using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using System.Text;

namespace URL_Shortcut_Service
{
    static class AsyncSocketServer
    {
        public static string LogFile { get; set; } = @"C:\URL_Shortcut_Service_Log.txt";
        public const string EOF = "<-EOF->";
        private static ManualResetEvent waitSignal = new ManualResetEvent(false);

        public static void LaunchServer(int port, int backlog)
        {
            // The main socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Bind the socket
                IPAddress ip = IPAddress.Any;
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                socket.Bind(endPoint);
                Log(string.Format("Socket bound on {0}:{1}", ip.ToString(), port));

                // Start listening
                socket.Listen(backlog);
                Log("Socket is listening");

                // Enter eternity
                while (true)
                {
                    // Reset signal
                    waitSignal.Reset();

                    // Launch an async socket to accept a connection
                    socket.BeginAccept(new AsyncCallback(Accepted), socket);
                    Log("Awaiting connections...");

                    // Wait until a connection is established
                    waitSignal.WaitOne();
                }
            }
            catch (Exception ex)
            {
                // Logging the exception
                Log(ex.ToString());
            }
            finally
            {
                // Shutdown the socket
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Log("Socket unexpected shutdown");
            }
        }

        private static void Accepted(IAsyncResult asyncResult)
        {
            /*
             * I believe the socket object that is passed
             * to this async callback function is cloned.
             * Otherwise, the wait signal must be set after
             * the client handler socket is acquired.
             */

            // Continue to accept another connection
            waitSignal.Set();

            // Cast back the socket
            Socket socket = (Socket)asyncResult.AsyncState;

            // Get client handler socket
            Socket clientSocket = socket.EndAccept(asyncResult);

            // Log the established connection
            Log(string.Format("Connection established: {0}", clientSocket.RemoteEndPoint.ToString()));

            // Create a communication object to serve this client
            CommunicationObject comObj = new CommunicationObject()
            {
                connection = clientSocket
            };

            // Start receiving client's packets
            clientSocket.BeginReceive(comObj.buffer, 0, comObj.buffer.Length, 
                SocketFlags.None, out SocketError errorCode, 
                new AsyncCallback(Receive), comObj);

            // Log error if there's any
            if (errorCode != SocketError.Success)
            {
                Log(string.Format("Error receiving packets: {0}", errorCode.ToString()));
            }
        }

        private static void Receive(IAsyncResult asyncResult)
        {
            // Log the action
            Log("Receiving...");

            // Cast back the communication object
            CommunicationObject comObj = (CommunicationObject)asyncResult.AsyncState;

            // Get the client handler socket
            Socket clientSocket = comObj.connection;

            // Retrieve data from client
            int bytesReceived = clientSocket.EndReceive(asyncResult);
            Log(string.Format("Received bytes: {0}", bytesReceived));

            // Proceed if anything is retrieved
            if (bytesReceived > 0)
            {
                // Translate the received packet
                string packet = Encoding.ASCII.GetString(comObj.buffer, 0, bytesReceived);
                Log(string.Format("Received packet: {0}", packet));

                // Keep whatever is received so far
                comObj.message.Append(packet);

                // Check for the end-of-file tag
                if (comObj.message.ToString().IndexOf(EOF) > -1)
                {
                    // Log the message
                    Log(string.Format("Message received: {0}", comObj.message.ToString()));

                    // Respond the client
                    Send(comObj);
                } else {
                    // Receive more packets
                    clientSocket.BeginReceive(comObj.buffer, 0, comObj.buffer.Length, 
                        SocketFlags.None, out SocketError errorCode, 
                        new AsyncCallback(Receive), comObj);

                    // Log error if there's any
                    if (errorCode != SocketError.Success)
                    {
                        Log(string.Format("Error receiving packets: {0}", errorCode.ToString()));
                    }
                }
            }
        }

        private static void Send(CommunicationObject comObj)
        {
            // Log the action
            Log("Sending...");

            // Prepare the complete message as the response
            byte[] response = Encoding.ASCII.GetBytes(comObj.message.ToString());

            // Start sending
            comObj.connection.BeginSend(response, 0, response.Length, 
                SocketFlags.None, out SocketError errorCode, 
                new AsyncCallback(Sent), comObj);

            // Log error if there's any
            if (errorCode != SocketError.Success)
            {
                Log(string.Format("Error sending packets: {0}", errorCode.ToString()));
            }
        }

        private static void Sent(IAsyncResult asyncResult)
        {
            // Log the action
            Log("Finalize sending...");

            // Cast back the communication object
            CommunicationObject comObj = (CommunicationObject)asyncResult.AsyncState;

            // Finish sending packets
            int bytesSent = comObj.connection.EndSend(asyncResult, out SocketError errorCode);
            Log(string.Format("Sent bytes: {0}", bytesSent));

            // Log error if there's any
            if (errorCode != SocketError.Success)
            {
                Log(string.Format("Error sending pending packets: {0}", errorCode.ToString()));
            }

            // Shutdown socket
            comObj.connection.Shutdown(SocketShutdown.Both);
            comObj.connection.Close();
            Log("Socket shutdown");
        }

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFile, string.Format("{0}\t{1}\n", DateTime.Now.ToString(), message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

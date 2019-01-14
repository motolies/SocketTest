using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerConsole
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client socket
        public Socket workSocket = null;
        // Size of receive buffer
        public const int BufferSize = 1024;
        // Receive buffer
        public byte[] buffer = new byte[BufferSize];
        // Received data string
        public StringBuilder sb = new StringBuilder();
    }

    class Program
    {
        // Thread signal
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static string TruncateLeft(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        // Get local IP
        public static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    return localIP;
                }
            }
            return "127.0.0.1";
        }

        private static void StartListening()
        {
            // Data buffer for incoming data
            byte[] bytes = new byte[1024];

            // Establish the local endpoint for the socket
            // running the listener is "192.168.0.12" 테스트 아이치
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 3333);


            // Create a TCP/IP socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connecions.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("\nWaiting for a connections...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before running
                    allDone.WaitOne();

                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("StartListening[SocketException] Error : {0} ", se.Message.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartListening[Exception] Error : {0} ", ex.Message.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue.....\n");
            Console.ReadLine();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client socket
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the socket object
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it it not there, read more data
                content = state.sb.ToString();
                if (content.IndexOf("<eof>") > -1)
                {
                    // All the data has been read from the
                    // client. DIsplay it on the console.
                    content = TruncateLeft(content, content.Length - 5);
                    Console.WriteLine("Read {0} bytes from socket \nData : {1}", content.Length, content);

                    // Echo the data back to the client                    
                    Send(handler, content);

                
                }
                else
                {
                    // Not all data received. Get more
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, string data)
        {
            // Convert the string data to bytes data using UTF8 encoding
            byte[] byteData = Encoding.UTF8.GetBytes(data + "<eof>");

            // Being sending the data to the remote device
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object
                Socket handler = ar.AsyncState as Socket;

                // Complete sending the data to the remote device
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (SocketException se)
            {
                Console.WriteLine("SendCallback[SocketException] Error : {0} ", se.Message.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendCallback[Exception] Error : {0} ", ex.Message.ToString());
            }
        }

        static void Main(string[] args)
        {
            StartListening();
        }
    }
}

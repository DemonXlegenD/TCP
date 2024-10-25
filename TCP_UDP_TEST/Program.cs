using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        Socket socket;
        int port = 11000;
        IPEndPoint localEP;
        Socket clientSocket;
        public Server()
        {
            //create server socket
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAdress = host.AddressList[0];
            localEP = new IPEndPoint(ipAdress, port);
            socket = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            Console.WriteLine("Starting Server");
            socket.Bind(localEP);
            socket.Listen(10);

            try
            {
                Console.WriteLine("Waiting for a connection...");
                // blocking instruction
                clientSocket = socket.Accept();

                Console.WriteLine("Accepted Client !");
            }
            catch (Exception e)
            {
                Console.WriteLine("error " + e.ToString());
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (clientSocket != null)
            {
                // shutdown client socket
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception e)
                {
                    Console.WriteLine("error " + e.ToString());
                }
                finally
                {
                    clientSocket.Close();
                }
            }

            if (socket != null)
            {
                // server socket : no shutdown necessary
                socket.Close();
            }
        }

        public void SendMessage(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            try
            {
                clientSocket.Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine("error sending message : " + e.ToString());
            }
        }

        public string ReceiveMessage()
        {
            try
            {
                byte[] messageReceived = new byte[1024];
                int nbBytes = clientSocket.Receive(messageReceived);
                return Encoding.ASCII.GetString(messageReceived, 0, nbBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("error receiving message : " + e.ToString());
            }
            return String.Empty;
        }
    }
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            string message = server.ReceiveMessage();
            Console.WriteLine("Server has received message : " + message);

            server.SendMessage("Hello from Server");

            server.Disconnect();
            Console.ReadKey();
        }
    }
}
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Client
    {
        Socket socket;
        int port = 11000;
        public bool Connected { get { return socket.Connected; } }
        public Client()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAdress = host.AddressList[0];
            socket = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Connect()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAdress = host.AddressList[0];
            IPEndPoint serverEP = new IPEndPoint(ipAdress, 11000);
            try
            {
                socket.Connect(serverEP);
                Console.WriteLine("Connected to server at " + ipAdress.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("error connecting to server " + e.ToString());
                Disconnect();
            }
        }
        public void Disconnect()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        public void SendMessage(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            try
            {
                socket.Send(msg);
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
                int nbBytes = socket.Receive(messageReceived);
                return Encoding.ASCII.GetString(messageReceived, 0, nbBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("error receiving message : " + e.ToString());
            }
            return String.Empty;
        }
    }
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect();

            string message = Console.ReadLine();
                
            client.SendMessage(message);
            Console.WriteLine("client has sent message " + message);

            message = client.ReceiveMessage();
            Console.WriteLine("client has received message " + message);

            client.Disconnect();
            Console.ReadKey();
        }
    }
}
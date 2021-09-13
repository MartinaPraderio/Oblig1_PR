using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtocolData;

namespace Server
{
    class ServerProgram
    {
        private const string ServerIpAdress = "127.0.0.1";
        private const int ServerPort = 6000;
        private const int Backlog = 100;



        static void Main(string[] args)
        {

            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress),ServerPort);

            serverSocket.Bind(serverIPEndPoint);
            serverSocket.Listen(Backlog);
            Console.WriteLine("Start listening 4 client");

            while (true)
            {
                Console.WriteLine("Esperando por conexiones....");
                var handler = serverSocket.Accept();
                new Thread(() => ProtocolData.Listen(handler)).Start();
                ProtocolData.Send(handler);
            }

            Console.WriteLine("Client connected to the server!");
            Console.ReadLine();
        }

       
    }
}
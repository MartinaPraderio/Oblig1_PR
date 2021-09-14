using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using ProtocolData;

namespace Server
{
    class ServerProgram
    {
        private static IConfiguration builder = new ConfigurationBuilder().AddJsonFile($"settings.json", true, true).Build();

        private static string ServerIpAdress = builder["ServerIpAdress"];

        private static int ServerPort = Int32.Parse(builder["ServerPort"]);

        private static int Backlog = Int32.Parse(builder["Backlog"]);

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
                new Thread(() => ProtocolDataProgram.Listen(handler)).Start();
                ProtocolDataProgram.Send(handler);
            }

            Console.WriteLine("Client connected to the server!");
            Console.ReadLine();
        }

       
    }
}
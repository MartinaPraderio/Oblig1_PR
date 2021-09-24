using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BusinessLogic;
using Microsoft.Extensions.Configuration;
using ProtocolData;
using System.Text.Json;
using Domain;
using Newtonsoft.Json;

namespace Server
{
    public class ServerProgram
    {
        //private static IConfiguration builder = new ConfigurationBuilder().AddJsonFile($"settings.json", true, true).Build();

        //private static string ServerIpAdress = builder["ServerIpAdress"];

        //private static int ServerPort = Int32.Parse(builder["ServerPort"]);

        //private static int Backlog = Int32.Parse(builder["Backlog"]);
        private static ServerServicesManager ServerServicesManager;


        private const int headerLength = 2;
        private const int dataLength = 4;
        private static bool exit = false;

        /*private static void ListenForConnections(Socket socketServer)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = socketServer.Accept();
                    _clients.Add(clientConnected);
                    var threacClient = new Thread(() => HandleClient(clientConnected));
                    threacClient.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;
                }
            }
            Console.WriteLine("Exiting....");
        }*/

        private static void HandleClient(Socket clientSocket)
        {
            

            while (!exit)
            {

                string command = ProtocolDataProgram.Listen(clientSocket);
                string message = ProtocolDataProgram.Listen(clientSocket);
                switch (command) 
                {
                    case "PublishGame":
                        {
                            Game aGame = JsonConvert.DeserializeObject<Game>(message);
                            string response = ServerServicesManager.PublishGame(aGame);
                            ProtocolDataProgram.Send(clientSocket,response);
                            break;
                        }
                        
                }

            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            string ServerIpAdress = "127.0.0.1";
            int ServerPort = 2000;
            int Backlog = 100;
            Catalogue gameCatalogue = new Catalogue();

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress),ServerPort);

            try
            {
                serverSocket.Bind(serverIPEndPoint);
                serverSocket.Listen(Backlog);
                Console.WriteLine("Start listening for client");

                while (true)
                {
                    Console.WriteLine("Esperando por conexiones....");
                    var handler = serverSocket.Accept();
                    new Thread(() => HandleClient(handler)).Start(); 
                    //new Thread(() => ProtocolDataProgram.Listen(handler)).Start();
                    ProtocolDataProgram.Send(handler,"hola soy cliene");
                }

            }

            catch (SocketException s)
            {
                Console.WriteLine("Excepcion: " + s.Message + ", Error Code: " + s.ErrorCode + ", Scoket Error Code: " + s.SocketErrorCode);
            }

        }

       
    }
}
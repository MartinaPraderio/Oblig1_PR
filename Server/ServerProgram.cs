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

        private static ServerServicesManager serverServicesManager;


        private const int headerLength = 2;
        private const int dataLength = 4;
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
            bool exit = false;
            while (!exit)
            {
                string command = ProtocolDataProgram.Listen(clientSocket);
                Console.WriteLine("Comando: "+command);
                string message = ProtocolDataProgram.Listen(clientSocket);
                switch (command) 
                {
                    case "PublishGame":
                        {
                            Game aGame = JsonConvert.DeserializeObject<Game>(message);
                            string response = serverServicesManager.PublishGame(aGame);
                            ProtocolDataProgram.Send(clientSocket,response);
                            break;
                        }
                    case "NotifyUsername":
                        {
                            User aUser = new User(message);
                            string response = serverServicesManager.AddUser(aUser);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "DeleteGame":
                        {
                            string response = serverServicesManager.DeleteGame(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "ModifyGame":
                        {
                            string response = serverServicesManager.ModifyGame(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "SearchGameByTitle":
                        {
                            string response = serverServicesManager.SearchGameByTitle(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "SearchGameByGender":
                        {
                            string response = serverServicesManager.SearchGameByGender(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "SearchGameByRating":
                        {
                            string response = serverServicesManager.SearchGameByRating(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "QualifyGame":
                        {
                            string response = serverServicesManager.QualifyGame(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "GameDetails":
                        {
                            string response = serverServicesManager.GameDetails(message);
                            ProtocolDataProgram.Send(clientSocket, response);
                            break;
                        }
                    case "EndConnection":
                        {
                            exit = true;
                            break;
                        }
                }

            }
        }

        static void Main(string[] args)
        {
            //IConfiguration builder = new ConfigurationBuilder().AddJsonFile("AppSettings.json", true, true).Build();

            //var ServerIpAdress = builder["Server:IP"];
            //var ServerPort = Int32.Parse(builder["Server:Port"]);
            //var ServerIpAdress = builder["ServerIpAdress"];
            //var ServerPort = Int32.Parse(builder["ServerPort"]);
            //var Backlog = Int32.Parse(builder["Server:Backlog"]);
            //Console.WriteLine("Soy server");
            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            string ServerIpAdress = "127.0.0.1";
            int ServerPort = 2000;
            int Backlog = 100;
            serverServicesManager = new ServerServicesManager();

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress),ServerPort);

            try
            {
                serverSocket.Bind(serverIPEndPoint);
                serverSocket.Listen(Backlog);
                int clientCounter = 0;
                while (true)
                {
                    PrintMenu();
                    string option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            {
                                clientCounter = AcceptClient(serverSocket, clientCounter);
                                Console.WriteLine("Cantidad de clientes ==> " + clientCounter);
                                break;
                            }
                        case "2":
                            {
                                serverServicesManager.PublishGame();
                                break;
                            }
                        case "3":
                            {
                                //serverServicesManager.ModifyGame();
                                break;
                            }
                        case "4":
                            {
                                serverServicesManager.SearchGame();
                                break;
                            }
                        case "5":
                            {
                                serverServicesManager.QualifyGame();
                                break;
                            }
                        case "6":
                            {
                                //serverServicesManager.GameDetails();
                                break;
                            }
                        case "7":
                            {
                                //serverServicesManager.DownloadGameCover();
                                break;
                            }
                        case "8":
                            {
                                //serverServicesManager.SendEmptyMessage();
                                //clientSocket.Shutdown(SocketShutdown.Both);
                                //clientSocket.Close();
                                Console.WriteLine("Cliente desconectado del servidor!");
                                break;
                            }
                    }
                    //new Thread(() => ProtocolDataProgram.Listen(handler)).Start();
                }

            }

            catch (SocketException s)
            {
                Console.WriteLine("Excepcion: " + s.Message + ", Error Code: " + s.ErrorCode + ", Scoket Error Code: " + s.SocketErrorCode);
            }

        }

        private static int AcceptClient(Socket serverSocket, int clientCounter)
        {
            Console.WriteLine("Esperando por conexiones....");
            var handler = serverSocket.Accept();
            Console.WriteLine("Cliente conectado!");
            new Thread(() => HandleClient(handler)).Start();
            clientCounter++;
            return clientCounter;
        }

        public static void PrintMenu()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(" Menu:");
            Console.WriteLine("1- Aceptar un cliente");
            Console.WriteLine("2- Publicar juego");
            Console.WriteLine("4- Buscar juego");
            Console.WriteLine("5- Calificar juego");
            Console.WriteLine("6- Detalle del juego");
            Console.WriteLine("7- Descargar caratula de un juego");
            Console.WriteLine("8- Ver catalogo de juegos");
            Console.WriteLine("-----------------------------------");
        }

       
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtocolData;
using BusinessLogic;
using Microsoft.Extensions.Configuration;

namespace Client
{
    class ClientProgram
    {

        private static IConfiguration builder = new ConfigurationBuilder().AddJsonFile($"settings.json",true, true).Build();

        private static string ServerIpAdress = builder["ServerIpAdress"];

        private static int ServerPort = Int32.Parse(builder["ServerPort"]);

        private static string ClientIpAdress = builder["ClientIpAdress"];

        private static int ClientPort = Int32.Parse(builder["ClientPort"]);

        private static Socket clientSocket;

        private static IPEndPoint clientEndPoint;

        private static IPEndPoint serverIPEndPoint;

        private static ClientServicesManager ClientServicesManager;

        public static void conectToServer()
        {
             clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIpAdress), ClientPort);

            clientSocket.Bind(clientEndPoint);

            Console.WriteLine("Tratando de conectarse al servidor...");

            serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);

            clientSocket.Connect(serverIPEndPoint);
            Console.WriteLine("Cliente conectado al servidor!");

        }

        public static void printMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1- Publicar juego");
            Console.WriteLine("2- Eliminar juego");
            Console.WriteLine("3- Modificar juego");
            Console.WriteLine("4- Buscar juego");
            Console.WriteLine("5- Calificar juego");
            Console.WriteLine("6- Detalle del juego");
            Console.WriteLine("7- Desconectarse del servidor");

        }

        static void Main(string[] args)
        {

            Console.WriteLine("¿Desea conectarse al servidor?");
            Console.WriteLine("Si (Digite 1)");
            Console.WriteLine("No (Digite 2)");

            string response = Console.ReadLine();

            if(response.Equals("1"))
            {
                try
                {
                    conectToServer();

                    Console.WriteLine("Ingrese su nombre de usuario");
                    string userName = Console.ReadLine();


                    //new Thread(() => ProtocolDataProgram.Listen(clientSocket)).Start();
                    Console.WriteLine("Connected to server");

                    //ProtocolDataProgram.Send(clientSocket);

                    Console.WriteLine("Bienvenido a la plataforma");

                    printMenu();

                    string menuOption = Console.ReadLine();

                    ClientServicesManager = new ClientServicesManager(userName);
                    ClientServicesManager.SendUserName(clientSocket,userName);

                    while (menuOption != "7")
                    {
                        switch (menuOption)
                        {
                            case "1":
                                {
                                    ClientServicesManager.PublishGame(clientSocket);
                                    break;
                                }
                            case "2":
                                {
                                    ClientServicesManager.DeleteGame();
                                    break;
                                }
                            case "3":
                                {
                                    ClientServicesManager.ModifyGame();
                                    break;
                                }
                            case "4":
                                {
                                    ClientServicesManager.SearchGame();
                                    break;
                                }
                            case "5":
                                {
                                    ClientServicesManager.QualifyGame();
                                    break;
                                }
                            case "6":
                                {
                                    ClientServicesManager.GameDetails();
                                    break;
                                }
                            case "7":
                                {
                                    clientSocket.Shutdown(SocketShutdown.Send);
                                    clientSocket.Close();
                                    Console.WriteLine("7- Cliente desconectado del servidor!");
                                    break;
                                }
                        }
                    }

                }

                catch (SocketException s)
                {
                    Console.WriteLine("Error: " + s.ErrorCode + ", Error Code: " + s.SocketErrorCode);
                }
            }
        }
    }
}

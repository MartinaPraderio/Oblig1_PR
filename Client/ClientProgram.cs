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

        private static string ServerIpAdress = "127.0.0.1"; //builder["ServerIpAdress"];

        private static int ServerPort = 2000; //Int32.Parse(builder["ServerPort"]);

        private static string ClientIpAdress = "127.0.0.1"; //builder["ClientIpAdress"];

        private static int ClientPort = 6001; //Int32.Parse(builder["ClientPort"]);

        private static Socket clientSocket;

        private static IPEndPoint clientEndPoint;

        private static IPEndPoint serverIPEndPoint;

        private static ClientServicesManager clientServicesManager;

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
            clientServicesManager = new ClientServicesManager();
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
                    clientServicesManager.SendMessage(clientSocket, userName,action.NotifyUsername);

                    //new Thread(() => ProtocolDataProgram.Listen(clientSocket)).Start();
                    Console.WriteLine("Connected to server");

                    //ProtocolDataProgram.Send(clientSocket);

                    Console.WriteLine("Bienvenido a la plataforma");
                    string menuOption="";
                    //ClientServicesManager.SendUserName(clientSocket,userName);

                    while (menuOption != "7")
                    {
                        printMenu();

                        menuOption = Console.ReadLine();

                        switch (menuOption)
                        {
                            case "1":
                                {
                                    clientServicesManager.PublishGame(clientSocket);
                                    break;
                                }
                            case "2":
                                {
                                    clientServicesManager.DeleteGame();
                                    break;
                                }
                            case "3":
                                {
                                    clientServicesManager.ModifyGame();
                                    break;
                                }
                            case "4":
                                {
                                    //ClientServicesManager.SearchGame();
                                    break;
                                }
                            case "5":
                                {
                                    //ClientServicesManager.QualifyGame();
                                    break;
                                }
                            case "6":
                                {
                                    //ClientServicesManager.GameDetails();
                                    break;
                                }
                            case "7":
                                {
                                    clientServicesManager.SendEmptyMessage(clientSocket);
                                    clientSocket.Shutdown(SocketShutdown.Both);
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

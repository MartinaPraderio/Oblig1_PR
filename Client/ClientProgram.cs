using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtocolData;
using Microsoft.Extensions.Configuration;

namespace Client
{
    class ClientProgram
    {
        private static Socket clientSocket;

        private static ClientServicesManager clientServicesManager;

        private string ServerIpAdress;

        public static void printMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("------- Menu:------------------------");
            Console.WriteLine("1 - Publicar juego");
            Console.WriteLine("2 - Eliminar juego");
            Console.WriteLine("3 - Modificar juego");
            Console.WriteLine("4 - Buscar juego");
            Console.WriteLine("5 - Calificar juego");
            Console.WriteLine("6 - Detalle del juego");
            Console.WriteLine("7 - Adquirir Juego");
            Console.WriteLine("8 - Desconectarse del servidor");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("");

        }

        static void Main(string[] args)
        {
            IConfiguration builder = new ConfigurationBuilder().AddJsonFile("Settings.json", true, true).Build();
            string ServerIpAdress = builder["Server:IP"];
            var ServerPort = Int32.Parse(builder["Server:ServerPort"]);
            string ClientIpAdress = builder["Server:IP"];
            var ClientPort = Int32.Parse(builder["Server:ClientPort"]);
            Console.WriteLine("Bienvenido a VAPOR");
            Console.WriteLine("");
            Console.WriteLine("Para conectarse al servidor ingrese 1");
            Console.WriteLine("");
            Console.WriteLine("-------------------------------------");

            string response = "";
            while (!response.Equals("1"))
            {
               response = Console.ReadLine();
            }

            
            try
            {

                
                ConnectToServer(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
                Console.WriteLine("Ingrese su nombre de usuario");
                string userName = Console.ReadLine();
                clientServicesManager = ClientServicesManager.Instance();
                clientServicesManager.SetSocket(clientSocket);
                clientServicesManager.SetUserName(userName);
                clientServicesManager.SendMessage(userName, action.NotifyUsername);
                string userCreatedResponse = ProtocolDataProgram.Listen(clientSocket);
                Console.WriteLine(userCreatedResponse);

                Console.WriteLine("Bienvenido a la plataforma, " + userName);
                string menuOption = "";
                bool success = true;
                while (menuOption != "8")
                {
                    if (!success)
                    {
                        Console.WriteLine("---------------/!\\-----------------");
                        Console.WriteLine("Se perdió la conexión con el servidor.");
                        success = ConnectToServerInterface(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
                        if (success)
                        {
                            Console.WriteLine("Ingrese su nombre de usuario");
                            userName = Console.ReadLine();
                            clientServicesManager.SetSocket(clientSocket);
                            clientServicesManager.SetUserName(userName);
                            clientServicesManager.SendMessage(userName, action.NotifyUsername);
                            userCreatedResponse = ProtocolDataProgram.Listen(clientSocket);
                            Console.WriteLine(userCreatedResponse);
                        }
                    }
                    else
                    {
                        printMenu();

                        menuOption = Console.ReadLine();

                        switch (menuOption)
                        {
                            case "1":
                                {
                                    success = clientServicesManager.PublishGame();
                                    break;
                                }
                            case "2":
                                {
                                    success = clientServicesManager.DeleteGame();
                                    break;
                                }
                            case "3":
                                {
                                    success = clientServicesManager.ModifyGame();
                                    break;
                                }
                            case "4":
                                {
                                    success = clientServicesManager.SearchGame();
                                    break;
                                }
                            case "5":
                                {
                                    success = clientServicesManager.QualifyGame();
                                    break;
                                }
                            case "6":
                                {
                                    success = clientServicesManager.GameDetails();
                                    break;
                                }
                            case "7":
                                {
                                    success = clientServicesManager.BuyGame();
                                    break;
                                }
                            case "8":
                                {
                                    clientSocket.Shutdown(SocketShutdown.Both);
                                    clientSocket.Close();
                                    break;
                                }
                        }
                    }
                }

            }

            catch (SocketException s)
            {
                Console.WriteLine("Error: " + s.ErrorCode + ", Error Code: " + s.SocketErrorCode);
            }
            
        }

        private static bool ConnectToServerInterface(string ClientIpAdress, int ClientPort, string ServerIpAdress, int ServerPort)
        {
            Console.WriteLine("Ingrese 1 para intentar la conexion.");
            string option = Console.ReadLine();
            while (!option.Equals("1"))
            {
                option = Console.ReadLine();
            }
            bool success = ConnectToServer(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
            return success;
        }

        private static bool ConnectToServer(string ClientIpAdress, int ClientPort, string ServerIpAdress, int ServerPort)
        {
            clientSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp
                    );
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIpAdress), ClientPort);
            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);
            Console.WriteLine("Tratando de conectarse al servidor...");
            try
            {
                clientSocket.Bind(clientEndPoint);
                clientSocket.Connect(serverIPEndPoint);
                Console.WriteLine("Cliente conectado al servidor!");
                return true;
            }catch(Exception e)
            {
                Console.WriteLine("Falla al intentar conectarse al server");
                return false;
            }
            
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("");
        }
    }
}

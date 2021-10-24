using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtocolData;
using Microsoft.Extensions.Configuration;

namespace Client
{
    public class ClientProgram
    {
        private static TcpClient _tcpClient;

        private static ClientServicesManager clientServicesManager;

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
            Console.WriteLine("7 - Adquirir un juego");
            Console.WriteLine("8 - Ver mis juegos");
            Console.WriteLine("9 - Desconectarse del servidor");
            Console.WriteLine("10 - Cerrar sesion");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("");

        }

        public static void Main(string[] args)
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
                
                string menuOption = "";
                bool success = true;
                bool logged = false;
                while (menuOption != "9")
                {
                    while (!logged) {
                        clientServicesManager = ClientServicesManager.Instance();
                        clientServicesManager.SetTcpClient(_tcpClient);
                        Console.WriteLine("Ingrese su nombre de usuario");
                        string userName = Console.ReadLine();
                        clientServicesManager.SendMessage(userName, action.NotifyUsername);
                        string userCreatedResponse = ProtocolDataProgram.Listen(_tcpClient.GetStream());
                        Console.WriteLine(userCreatedResponse);
                        if(userCreatedResponse.Equals("Login exitoso"))
                        {
                            clientServicesManager.SetUserName(userName);   
                            logged = true;
                        }
                    }

                    if (!success)
                    {
                        Console.WriteLine("---------------/!\\-----------------");
                        Console.WriteLine("Se perdió la conexión con el servidor.");
                        success = ConnectToServerInterface(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
                        if (success)
                        {
                            Console.WriteLine("Ingrese su nombre de usuario");
                            string userName = Console.ReadLine();
                            clientServicesManager.SetTcpClient(_tcpClient);
                            clientServicesManager.SetUserName(userName);
                            clientServicesManager.SendMessage(userName, action.NotifyUsername);
                            string userCreatedResponse = ProtocolDataProgram.Listen(_tcpClient.GetStream());
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
                                    success = clientServicesManager.ViewUserGames();
                                    break;
                                }
                            case "9":
                                {
                                    _tcpClient.GetStream().Close();
                                    _tcpClient.Close();
                                    break;
                                }
                            case "10":
                                {
                                    logged = false;
                                    Console.WriteLine("Se cerró la sesión correctamente");
                                    clientServicesManager.SetUserName("");
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
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIpAdress), ClientPort);
            IPEndPoint serverIPEndPoint  = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);
            Console.WriteLine("Tratando de conectarse al servidor...");
            try
            {
                _tcpClient = new TcpClient(clientEndPoint);
                _tcpClient.Connect(serverIPEndPoint);
                Console.WriteLine("Cliente conectado al servidor!");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Falla al intentar conectarse al server");
                return false;
            }
            
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("");
        }
    }
}

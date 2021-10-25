using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtocolData;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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

        public static async Task Main(string[] args)
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
                await ConnectToServerAsync(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
                
                string menuOption = "";
                bool success = true;
                bool logged = false;
                while (menuOption != "9")
                {
                    logged = await ServerLoginAsync(logged);

                    if (!success)
                    {
                        logged = false;
                        Console.WriteLine("---------------/!\\-----------------");
                        Console.WriteLine("Se perdió la conexión con el servidor.");
                        while (!success)
                        {
                            success = await ConnectToServerInterface(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
                            if (success)
                            {
                                logged = await ServerLoginAsync(logged);
                            }
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
                                    success = await clientServicesManager.PublishGameAsync();
                                    break;
                                }
                            case "2":
                                {
                                    success = await clientServicesManager.DeleteGameAsync();
                                    break;
                                }
                            case "3":
                                {
                                    success = await clientServicesManager.ModifyGameAsync();
                                    break;
                                }
                            case "4":
                                {
                                    success = await clientServicesManager.SearchGameAsync();
                                    break;
                                }
                            case "5":
                                {
                                    success = await clientServicesManager.QualifyGameAsync();
                                    break;
                                }
                            case "6":
                                {
                                    success = await clientServicesManager.GameDetailsAsync();
                                    break;
                                }
                            case "7":
                                {
                                    success = await clientServicesManager.BuyGameAsync();
                                    break;
                                }
                            case "8":
                                {
                                    success = await clientServicesManager.ViewUserGamesAsync();
                                    break;
                                }
                            case "9":
                                {
                                    success = await clientServicesManager.Logout();
                                    _tcpClient.GetStream().Close(1);
                                    _tcpClient.Close();
                                    break;
                                }
                            case "10":
                                {
                                    success = await clientServicesManager.Logout();
                                    logged = false;
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

        private static async Task<bool> ServerLoginAsync(bool logged)
        {
            while (!logged)
            {
                clientServicesManager = ClientServicesManager.Instance();
                clientServicesManager.SetTcpClient(_tcpClient);
                Console.WriteLine("Ingrese su nombre de usuario");
                string userName = Console.ReadLine();
                await clientServicesManager.SendMessageAsync(userName, action.NotifyUsername);
                string userCreatedResponse = await ProtocolDataProgram.ListenAsync(_tcpClient.GetStream());
                Console.WriteLine(userCreatedResponse);
                if (userCreatedResponse.Equals("Login exitoso"))
                {
                    clientServicesManager.SetUserName(userName);
                    logged = true;
                }
            }

            return logged;
        }

        private static async Task<bool> ConnectToServerInterface(string ClientIpAdress, int ClientPort, string ServerIpAdress, int ServerPort)
        {
            Console.WriteLine("Ingrese 1 para intentar la conexion.");
            string option = Console.ReadLine();
            while (!option.Equals("1"))
            {
                option = Console.ReadLine();
            }
            bool success = await ConnectToServerAsync(ClientIpAdress, ClientPort, ServerIpAdress, ServerPort);
            return success;
        }

        private static async Task<bool> ConnectToServerAsync(string ClientIpAdress, int ClientPort, string ServerIpAdress, int ServerPort)
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIpAdress), ClientPort);
            Console.WriteLine("Tratando de conectarse al servidor...");
            try
            {
                _tcpClient = new TcpClient(clientEndPoint);
                await _tcpClient.ConnectAsync(IPAddress.Parse(ServerIpAdress), ServerPort);
                Console.WriteLine("Cliente conectado al servidor!");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Falla al intentar conectarse al server");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("");
                return false;
            }
        }
    }
}

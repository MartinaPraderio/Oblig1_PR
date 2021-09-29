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

            Socket clientSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp
                    );
            try
            {
                    
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIpAdress), ClientPort);

                clientSocket.Bind(clientEndPoint);

                Console.WriteLine("Tratando de conectarse al servidor...");

                IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);

                clientSocket.Connect(serverIPEndPoint);

                Console.WriteLine("Cliente conectado al servidor!");
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("Ingrese su nombre de usuario");
                string userName = Console.ReadLine();
                clientServicesManager = ClientServicesManager.Instance();
                clientServicesManager.SetSocket(clientSocket);
                clientServicesManager.SetUserName(userName);
                clientServicesManager.SendMessage(userName,action.NotifyUsername);
                string userCreatedResponse = ProtocolDataProgram.Listen(clientSocket);
                Console.WriteLine(userCreatedResponse);

                Console.WriteLine("Bienvenido a la plataforma, "+userName);
                string menuOption="";
                    
                while (menuOption != "8")
                {

                        printMenu();

                        menuOption = Console.ReadLine();

                        switch (menuOption)
                        {
                            case "1":
                                {
                                    clientServicesManager.PublishGame();
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
                                    clientServicesManager.SearchGame();
                                    break;
                                }
                            case "5":
                                {
                                    clientServicesManager.QualifyGame();
                                    break;
                                }
                            case "6":
                                {
                                    clientServicesManager.GameDetails();
                                    break;
                                }
                            case "7":
                                {
                                    clientServicesManager.BuyGame();
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

            catch (SocketException s)
            {
                Console.WriteLine("Error: " + s.ErrorCode + ", Error Code: " + s.SocketErrorCode);
            }
            
        }
    }
}

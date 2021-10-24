using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using ProtocolData;
using Domain;
using System.Collections.Generic;

namespace Server
{
    public class ServerProgram
    {
        private static TcpListener _tcpListener;
        private static ServerServicesManager serverServicesManager;

        static bool _exit = false;
        static List<TcpClient> _clients = new List<TcpClient>();

        private static void ListenForConnections(TcpListener tcpListener) //ver si recibo eso o tcpClient y con lo de networkStteamHandler
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = tcpListener.AcceptTcpClient(); //ver si es este o AcceptTcpClientAsync
                    _clients.Add(clientConnected);
                    HandleClient(clientConnected);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;

                }
            }
            Console.WriteLine("Exiting....");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(" Menu:");
            Console.WriteLine("1- Ver catalogo de juegos");
            Console.WriteLine("2- Desconectar servidor");
            Console.WriteLine("3- Crear usuario");
            Console.WriteLine("4- Modificar usuario");
            Console.WriteLine("5- Eliminar usuario");
            Console.WriteLine("6- Ver lista de usuarios");
            Console.WriteLine("-----------------------------------");
        }
        private static void HandleClient(TcpClient tcpClient)
        {
            bool connected = true;
            while (connected)
            {
                try
                {
                    string command = ProtocolDataProgram.Listen(tcpClient.GetStream());
                    string message = ProtocolDataProgram.Listen(tcpClient.GetStream());
                    switch (command)
                    {
                        case "PublishGame":
                            {
                                Game aGame = ProtocolDataProgram.DeserializeGame(message);
                                string response = serverServicesManager.PublishGame(aGame, tcpClient);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "NotifyUsername":
                            {
                                string response = serverServicesManager.Login(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "DeleteGame":
                            {
                                string response = serverServicesManager.DeleteGame(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ModifyGame":
                            {
                                string response = serverServicesManager.ModifyGame(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByTitle":
                            {
                                string response = serverServicesManager.SearchGameByTitle(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByGender":
                            {
                                string response = serverServicesManager.SearchGameByGender(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByRating":
                            {
                                string response = serverServicesManager.SearchGameByRating(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "QualifyGame":
                            {
                                string response = serverServicesManager.QualifyGame(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "GameDetails":
                            {
                                string response = serverServicesManager.GameDetails(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "EndConnection":
                            {
                                _exit = true;
                                break;
                            }
                        case "GameCover":
                            {
                                string response = serverServicesManager.SendGameCover(message, tcpClient);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ViewCatalogue":
                            {
                                string response = serverServicesManager.ViewCatalogue();
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "BuyGame":
                            {
                                string response = serverServicesManager.BuyGame(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ViewUserGames":
                            {
                                string response = serverServicesManager.ShowUserGames(message);
                                ProtocolDataProgram.Send(tcpClient.GetStream(), response);
                                break;
                            }
                    }
                }
                catch 
                {
                    connected = false;

                }
                

            }
        }

        public static void Main(string[] args)
        {
            IConfiguration builder = new ConfigurationBuilder().AddJsonFile("Settings.json", true, true).Build();
            var ServerIpAdress = builder["Server:IP"];
            var ServerPort = Int32.Parse(builder["Server:Port"]);
            var Backlog = Int32.Parse(builder["Server:Backlog"]);

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);

            _tcpListener = new TcpListener(serverIPEndPoint);
            serverServicesManager = ServerServicesManager.Instance();
            serverServicesManager.SetTcpListener(_tcpListener);

            bool inicio = true;
            try
            {
                _tcpListener.Start(1);
                //serverSocket.Listen(Backlog);
                ListenForConnections(_tcpListener);
                 //es backlog ahi no?
                //o es Start() solo 
                while (!_exit)
                {
                    if (inicio)
                    {
                        Console.WriteLine("Si desea iniciar la aplicacion con datos de prueba ingrese 1");
                        Console.WriteLine("De lo contrario ingrese cualquier caracter");
                        string response = Console.ReadLine();
                        Console.WriteLine("");
                        if (response.Equals("1"))
                        {
                            serverServicesManager.CargarDatosDePrueba();
                            Console.WriteLine("Su aplicación se iniciara con datos de prueba");
                        }
                        else
                        {
                            Console.WriteLine("Su aplicación se iniciara vacía");
                        }
                        Console.WriteLine("---------------------------------------------");
                        Console.WriteLine("");
                        inicio = false;
                    }
                    PrintMenu();
                    string option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            {
                                string catalogue = serverServicesManager.ViewCatalogue();
                                Console.WriteLine(catalogue);
                                break;
                            }                            
                        case "2":
                            {
                                _exit = true;
                                _tcpListener.Stop();
                                foreach (var client in _clients)
                                {
                                    client.GetStream().Close();
                                    client.Close();
                                }
                                Console.WriteLine("Desconectando el servidor!");
                                break;
                            }
                        case "3":
                            {
                                Console.WriteLine("Ingrese el nombre del nuevo usuario");
                                String name = Console.ReadLine();
                                User newUser = new User(name);
                                serverServicesManager.AddUser(newUser);
                                Console.WriteLine("El usuario "+name+" fue registrado con exito.");
                                break;
                            }
                        case "4":
                            {
                                Console.WriteLine("Ingrese el nombre del usuario a modificar");
                                String name = Console.ReadLine();
                                serverServicesManager.ModifyUser(name);
                                break;
                            }
                        case "5":
                            {
                                Console.WriteLine("Ingrese el nombre del usuario a eliminar");
                                String name = Console.ReadLine();
                                serverServicesManager.DeleteUser(name);
                                break;
                            }
                        case "6":
                            {
                                serverServicesManager.ShowUsers();
                                break;
                            }
                    }
                }
            }
            catch (SocketException s)
            {
                Console.WriteLine("Excepcion: " + s.Message + ", Error Code: " + s.ErrorCode + ", Scoket Error Code: " + s.SocketErrorCode);
            }
        }
    }
}
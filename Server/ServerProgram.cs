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

        private static ServerServicesManager serverServicesManager;

        static bool _exit = false;
        static List<Socket> _clients = new List<Socket>();

        private static void ListenForConnections(Socket socketServer)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = socketServer.Accept();
                    _clients.Add(clientConnected);
                    var threadClient = new Thread(() => HandleClient(clientConnected));
                    threadClient.Start();
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
        private static void HandleClient(Socket clientSocket)
        {
            bool connected = true;
            while (connected)
            {
                try
                {
                    string command = ProtocolDataProgram.Listen(clientSocket);
                    string message = ProtocolDataProgram.Listen(clientSocket);
                    switch (command)
                    {
                        case "PublishGame":
                            {
                                Game aGame = ProtocolDataProgram.DeserializeGame(message);
                                string response = serverServicesManager.PublishGame(aGame, clientSocket);
                                ProtocolDataProgram.Send(clientSocket, response);
                                break;
                            }
                        case "NotifyUsername":
                            {
                                string response = serverServicesManager.Login(message);
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
                                _exit = true;
                                break;
                            }
                        case "GameCover":
                            {
                                string response = serverServicesManager.SendGameCover(message, clientSocket);
                                ProtocolDataProgram.Send(clientSocket, response);
                                break;
                            }
                        case "ViewCatalogue":
                            {
                                string response = serverServicesManager.ViewCatalogue();
                                ProtocolDataProgram.Send(clientSocket, response);
                                break;
                            }
                        case "BuyGame":
                            {
                                string response = serverServicesManager.BuyGame(message);
                                ProtocolDataProgram.Send(clientSocket, response);
                                break;
                            }
                        case "ViewUserGames":
                            {
                                string response = serverServicesManager.ShowUserGames(message);
                                ProtocolDataProgram.Send(clientSocket, response);
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

        static void Main(string[] args)
        {
            IConfiguration builder = new ConfigurationBuilder().AddJsonFile("Settings.json", true, true).Build();
            var ServerIpAdress = builder["Server:IP"];
            var ServerPort = Int32.Parse(builder["Server:Port"]);
            var Backlog = Int32.Parse(builder["Server:Backlog"]);
            Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            serverServicesManager = ServerServicesManager.Instance();
            serverServicesManager.SetSocket(serverSocket);

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress),ServerPort);
            bool inicio = true;
            try
            {
                serverSocket.Bind(serverIPEndPoint);
                serverSocket.Listen(Backlog);
                new Thread(() => ListenForConnections(serverSocket)).Start();
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
                                serverSocket.Close(0);
                                foreach (var client in _clients)
                                {
                                    client.Shutdown(SocketShutdown.Both);
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
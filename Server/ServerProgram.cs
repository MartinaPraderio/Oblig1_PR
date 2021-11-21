using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using ProtocolData;
using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace Server
{
    public class ServerProgram
    {
        private static TcpListener _tcpListener;
        private static ServerServicesManager serverServicesManager;

        static bool _exit = false;
        static List<TcpClient> _clients = new List<TcpClient>();

        private static async Task ListenForConnectionsAsync(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync(); 
                    Task userThread = new Task(async () => await HandleClientAsync(tcpClient));
                    userThread.Start();
                    _clients.Add(tcpClient);
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
        private static async Task HandleClientAsync(TcpClient tcpClient)
        {
            bool connected = true;
            while (connected)
            {
                try
                {
                    string command = await ProtocolDataProgram.ListenAsync(tcpClient.GetStream());
                    string message = await ProtocolDataProgram.ListenAsync(tcpClient.GetStream());
                    switch (command)
                    {
                        case "PublishGame":
                            {
                                Domain.Game aGame = ProtocolDataProgram.DeserializeGame(message);
                                string response = await serverServicesManager.PublishGameAsync(aGame, tcpClient);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "NotifyUsername":
                            {
                                string response = serverServicesManager.Login(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "DeleteGame":
                            {
                                string response = serverServicesManager.DeleteGame(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ModifyGame":
                            {
                                string response = serverServicesManager.ModifyGame(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByTitle":
                            {
                                string response = serverServicesManager.SearchGameByTitle(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByGender":
                            {
                                string response = serverServicesManager.SearchGameByGender(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "SearchGameByRating":
                            {
                                string response = serverServicesManager.SearchGameByRating(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "QualifyGame":
                            {
                                string response = serverServicesManager.QualifyGame(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "GameDetails":
                            {
                                string response = serverServicesManager.GameDetails(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "EndConnection":
                            {
                                _exit = true;
                                break;
                            }
                        case "GameCover":
                            {
                                string response = await serverServicesManager.SendGameCoverAsync(message, tcpClient);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ViewCatalogue":
                            {
                                string response = serverServicesManager.ViewCatalogue();
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "BuyGame":
                            {
                                string response = serverServicesManager.BuyGame(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "ViewUserGames":
                            {
                                string response = serverServicesManager.ShowUserGames(message);
                                await ProtocolDataProgram.SendAsync(tcpClient.GetStream(), response);
                                break;
                            }
                        case "LogOut":
                            {
                                serverServicesManager.LogOut(message);
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

        public static async Task Main(string[] args)
        {

            

            IConfiguration builder = new ConfigurationBuilder().AddJsonFile("Settings.json", true, true).Build();
            var ServerIpAdress = builder["Server:IP"];
            var ServerPort = Int32.Parse(builder["Server:Port"]);
            var Backlog = Int32.Parse(builder["Server:Backlog"]);

            IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAdress), ServerPort);

            _tcpListener = new TcpListener(serverIPEndPoint);
            serverServicesManager = ServerServicesManager.Instance();
            serverServicesManager.SetTcpListener(_tcpListener);

            //gRCP
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new Provider.ProviderClient(channel);
            serverServicesManager.SetGrpcClient(grpcClient);

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

            try
            {
                _tcpListener.Start(Backlog);
                var connectionTask = Task.Run(async() => await ListenForConnectionsAsync(_tcpListener));
                while (!_exit)
                {
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
                                await serverServicesManager.AddUser(name);
                                Console.WriteLine("El usuario "+name+" fue registrado con exito.");
                                break;
                            }
                        case "4":
                            {
                                Console.WriteLine("Ingrese el nombre del usuario a modificar");
                                String name = Console.ReadLine();
                                await serverServicesManager.ModifyUser(name);
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
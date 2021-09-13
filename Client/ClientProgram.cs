using System;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class ClientProgram
    {
        private const string ServerIpAdress = "127.0.0.1";

        private const int ServerPort = 6000;

        private const string ClientIpAdress = "127.0.0.1";

        private const int ClientPort = 6001;

        private static Socket clientSocket;

        private static IPEndPoint clientEndPoint;

        private static IPEndPoint serverIPEndPoint;

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

        static void Main(string[] args)
        {

            Console.WriteLine("¿Desea conectarse al servidor?");
            Console.WriteLine("Si (Digite 1)");
            Console.WriteLine("No (Digite 2)");

            string response = Console.ReadLine();

            if(response.Equals("1"))
            {
                conectToServer();

                Console.WriteLine("Ingrese su nombre de usuario");
                string userName = Console.ReadLine();


                new Thread(() => ProtocoloIntercambioProgram.Listen(clientSocket)).Start();
                Console.WriteLine("Connected to server");

                ProtocoloIntercambioProgram.Send(clientSocket);

                Console.WriteLine("Bienvenido a la plataforma");
                Console.WriteLine("Menu:");
                Console.WriteLine("1- Publicar juego");
                Console.WriteLine("2- Eliminar juego");
                Console.WriteLine("3- Modificar juego");
                Console.WriteLine("4- Buscar juego");
                Console.WriteLine("5- Calificar juego");
                Console.WriteLine("6- Detalle del juego");
                Console.WriteLine("7- Desconectarse del servidor");

                string menuOption = Console.ReadLine();

                switch (menuOption)
                {
                    case "1":
                        {

                            break;
                        }
                    case "2":
                        {
                            break;
                        }
                    case "3":
                        {
                            break;
                        }
                    case "4":
                        {
                            break;
                        }
                    case "5":
                        {
                            break;
                        }
                    case "6":
                        {
                            break;
                        }
                    case "7":
                        {
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                            Console.WriteLine("7- Cliente desconectado del servidor!");
                            break;
                        }
                }
            }
            
        }
    }
}

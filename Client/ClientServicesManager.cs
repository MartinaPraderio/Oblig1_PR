using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Text.Json;

namespace Client
{
    public class ClientServicesManager 
    {
        private ProtocolDataProgram protocolHandleData;

        private Socket clientSocket;
        //private ServerServicesManager serverServicesManager;

        public ClientServicesManager(Socket client)
        {
            this.protocolHandleData = new ProtocolDataProgram();
            this.clientSocket = client;
        }


        public void SendMessage(string message, action action)
        {
            Console.WriteLine("mandando accion "+action.ToString());
            ProtocolDataProgram.Send(clientSocket, action.ToString());
            ProtocolDataProgram.Send(clientSocket, message);
            //ProtocolDataProgram.Send(clientSocket, "");
        }


        //public void sendusername(Socket clientsocket, string username)
        //{
        //    //username = "[0]" + username;
        //    //llamar a otra funcion dentro de esta clase que
        //    Sendmessage(username, action.Sendusername);

        //}

        public void PublishGame()
        {
            Console.WriteLine("ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            GameGender genderGame = GameGender.Accion;
            Game game = new Game(title, genderGame, synopsis, "cover");

            string message = JsonSerializer.Serialize(game);
            Console.WriteLine(message);
            SendMessage(message, action.PublishGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }



        //public override void qualifygame() { }
        //public override game searchgame() { return new game(); }
        //public override void gamedetails() { }

        public void RequestServerConnection(){}
        public void EndServerConnection(){}
        public void ModifyGame(){}
        public void DeleteGame(){
            Console.WriteLine("ingrese el titulo del juego a eliminar");
            string title = Console.ReadLine();
            SendMessage(title, action.DeleteGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        internal void SendEmptyMessage(Socket clientSocket)
        {
            ProtocolDataProgram.Send(clientSocket, "");

        }
    }
}


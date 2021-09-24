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

        //private ServerServicesManager serverServicesManager;

        private string userName;

        public ClientServicesManager(string aUserName)
        {
            userName = aUserName;
            protocolHandleData = new ProtocolDataProgram();
        }


        private void SendMessage(Socket clientSocket, string message, action action)
        {
            Console.WriteLine("accion "+action.ToString());
            ProtocolDataProgram.Send(clientSocket, action.ToString());
            ProtocolDataProgram.Send(clientSocket, message);
        }


        //public void sendusername(Socket clientsocket, string username)
        //{
        //    //username = "[0]" + username;
        //    //llamar a otra funcion dentro de esta clase que
        //    Sendmessage(username, action.Sendusername);

        //}

        public void PublishGame(Socket clientsocket)
        {
            Console.WriteLine("ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            GameGender genderGame = GameGender.Accion;
            Game game = new Game(title, genderGame, synopsis, "cover");

            string message = JsonSerializer.Serialize(game);
            Console.WriteLine(message);
            SendMessage(clientsocket, message, action.PublishGame);
        }



        //public override void qualifygame() { }
        //public override game searchgame() { return new game(); }
        //public override void gamedetails() { }

        public void RequestServerConnection(){}
        public void EndServerConnection(){}
        public void ModifyGame(){}
        public void DeleteGame(){}

        internal void SendEmptyMessage(Socket clientSocket)
        {
            ProtocolDataProgram.Send(clientSocket, "");
        }
    }
}


using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;

namespace BusinessLogic

{
    public class ClientServicesManager : ServicesManager
    {
        private ProtocolDataProgram protocolHandleData;

        private ServerServicesManager serverServicesManager;

        private string userName;

        public ClientServicesManager(string aUserName)
        {
            userName = aUserName;
            protocolHandleData = new ProtocolDataProgram();
        }

        public void SendUserName(Socket clientSocket, string userName)
        {
            userName = "[0]" + userName;
            protocolHandleData.Send(clientSocket,userName);
        }

        public override void PublishGame(Socket clientSocket)
        {
            Console.WriteLine("Ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("Ingrese la descripción del juego");
            string description = Console.ReadLine();
            Console.WriteLine("Ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("Ingrese el genero del juego");
            string gender = Console.ReadLine();
            //Console.WriteLine("Ingrese las caratula del juego");
            //string cover = Console.ReadLine();

            string game = "REQ"+Environment.NewLine+"[1]"+Environment.NewLine + title +"/-"+ description + "/-" + synopsis + "/-" + gender;

            protocolHandleData.Send(clientSocket,game);
        }


        public override void QualifyGame() { }
        public override Game SearchGame() { return new Game(); }
        public override void GameDetails() { }

        public void RequestServerConnection(){}
        public void EndServerConnection(){}
        public void ModifyGame(){}
        public void DeleteGame(){}
    }
}


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

        private int ActionToInt(action theAction)
        {
            if (theAction.Equals(action.PublishGame))
            {
                return 1;
            }
            return 0;
        }
       

        private void SendMessage(Socket clientSocket, string message, action action)
        {
            protocolHandleData.Send(clientSocket, action.ToString());
            protocolHandleData.Send(clientSocket, message);
        }

        
        public void SendUserName(Socket clientSocket, string userName)
        {
            //userName = "[0]" + userName;
            //llamar a otra funcion dentro de esta clase que
            SendMessage(userName, action.SendUsername);
            
        }

        public void PublishGame(Socket clientSocket)
        {
            Console.WriteLine("Ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("Ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("Ingrese el genero del juego");
            string gender = Console.ReadLine();
            //Console.WriteLine("Ingrese las caratula del juego");
            //string cover = Console.ReadLine();
            GameGender genderGame = GameGender.Accion;
            Game game = new Game(title,genderGame,synopsis,"cover");

            string message = JsonSerializer.Serialize(game);

            SendMessage(socket, message,action.PublishGame);

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


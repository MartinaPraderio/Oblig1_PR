using System;
using System.Net.Sockets;
using Domain;

namespace BusinessLogic
{
    public class ServerServicesManager: ServicesManager
    {
        public override void PublishGame(Socket clientSocket) {


        }
        public override void QualifyGame() { }
        public override Game SearchGame() { return new Game(); }
        public override void GameDetails() { }

        public void AcceptClient(){}
        public void ViewCatalogue(){}
        public void BuyGame(){}
    }
}

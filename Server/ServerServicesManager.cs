using System;
using System.Net.Sockets;
using Domain;

namespace BusinessLogic
{
    public class ServerServicesManager
    {
        public void PublishGame(Socket clientSocket) {


        }
        public void QualifyGame() { }
        public Game SearchGame() { return new Game("titulo",GameGender.Accion,"sino","cover"); }
        public void GameDetails() { }

        public void AcceptClient(){}
        public void ViewCatalogue(){}
        public void BuyGame(){}
    }
}

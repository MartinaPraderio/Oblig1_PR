using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;

namespace BusinessLogic
{
    public class ServerServicesManager
    {
        private Catalogue gameCatalogue;
        public ServerServicesManager()
        {
            this.gameCatalogue = new Catalogue();
        }

        public string PublishGame(Game newGame) {
            this.gameCatalogue.AddGame(newGame);
            return "Juego agegado con exito";
        }
        public void QualifyGame() { }
        public Game SearchGame() { return new Game("titulo",GameGender.Accion,"sino","cover"); }
        public void GameDetails() { }

        public void AcceptClient(){}
        public void ViewCatalogue(){}
        public void BuyGame(){}
    }
}

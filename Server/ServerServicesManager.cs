using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;

namespace BusinessLogic
{
    public class ServerServicesManager
    {

        public void PublishGame(Socket clientSocket, string message) {

            //pasar de string a juego
            //agregar el juego a la lista
            //si ya existe mandar mensaje ya existe, sino mandar juego agregado
        }
        public void QualifyGame() { }
        public Game SearchGame() { return new Game("titulo",GameGender.Accion,"sino","cover"); }
        public void GameDetails() { }

        public void AcceptClient(){}
        public void ViewCatalogue(){}
        public void BuyGame(){}
    }
}

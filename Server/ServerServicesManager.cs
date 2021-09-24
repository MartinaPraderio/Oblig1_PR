using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class ServerServicesManager
    {
        private Catalogue gameCatalogue;
        private List<User> users;
        public ServerServicesManager()
        {
            this.gameCatalogue = new Catalogue();
            this.users = new List<User>();
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

        public string AddUser(User newUser) {
            Console.WriteLine("cant usuarios "+users.Count);
            this.users.Add(newUser);
            Console.WriteLine(users.Count);
            return "Usuario agragado con exito";
        }

        internal string DeleteGame(string message)
        {
            Game aGame = gameCatalogue.Games.Find(x => x.Title.Equals(message));
            string response = "";
            if (aGame!=null)
            {
                this.gameCatalogue.Games.Remove(aGame);
                response = "El juego fue eliminado.";
            }
            else
            {
                response = "El juego que quiere eliminar no existe";
            }
            return response;
        }
    }
}

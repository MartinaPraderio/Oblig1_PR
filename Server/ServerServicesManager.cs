using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;

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

        internal string ModifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            Game aGame = gameCatalogue.Games.Find(x => x.Title.Equals(info[0]));
            string response = "";
            if (aGame != null)
            {
                Console.WriteLine("titulo viejo: " + aGame.Title);
                Game newGameValues = JsonConvert.DeserializeObject<Game>(info[1]);
                aGame.Title = newGameValues.Title;
                aGame.Gender = newGameValues.Gender;
                aGame.Synopsis = newGameValues.Synopsis;
                response = "El juego fue modificado.";

                foreach (Game g in gameCatalogue.Games)
                {
                    Console.WriteLine(g.Title);
                }
            }
            else
            {
                response = "El juego que quiere modificar no existe";
            }
            return response;
        }

        internal string SearchGameByTitle(string message)
        {
            
            List<Game> games = gameCatalogue.Games.FindAll(x => x.Title.Contains(message));
            string response = "";
            if (games.Count != 0)
            {
                response = "E"+Environment.NewLine+System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N"+Environment.NewLine+"El juego que busca no existe";
            }
            return response;
        }

        internal string SearchGameByGender(string message)
        {
            List<Game> games = gameCatalogue.Games.FindAll(x => x.Gender.ToString().Equals(message));
            string response = "";
            if (games.Count != 0)
            {
                response = "E" + Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        internal string SearchGameByRating(string calification)
        {
            GameCalification calif = (GameCalification)Enum.Parse(typeof(GameCalification),calification);
            List<Game> games = gameCatalogue.Games.FindAll(x => Math.Truncate(x.RatingAverage).Equals(x.CalificationToInt(calif)));
            string response = "";
            if (games.Count != 0)
            {
                response = "E" + Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        internal string QualifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            string gameTitle = info[0];
            string calification = info[1];
            string review = info[2];
            Game gameTitle = //buscar juego
            string addRatingResponse = gameToQualify.AddRating(actualUser, calification, review);
            return addRatingResponse;
        }
    }
}

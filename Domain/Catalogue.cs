using System;
using System.Collections.Generic;

namespace Domain
{
    public class Catalogue
    {
        private static Catalogue instance = new Catalogue();
        public List<Game> Games { get; set; }

        public static Catalogue Instance
        {
            get { return instance; }
        }
        private Catalogue()
        {
            this.Games = new List<Game>();
        }
        public string DisplayGames()
        {
            string catalogueView = "";
            catalogueView += "Catalogo de juegos: " + Environment.NewLine;
            catalogueView += "" + Environment.NewLine;


            foreach (Game game in Games)
            {
                catalogueView += "Titulo: " + game.Title + Environment.NewLine;
                catalogueView += "Sinopsis: " + game.Synopsis + Environment.NewLine;
                catalogueView += "Categoría: " + game.Gender + Environment.NewLine;
                GameCalification calification = GameCalification.Sin_Calificaciones;
                switch (Math.Truncate(game.RatingAverage))
                {
                    case 1:
                        calification = GameCalification.Muy_Malo;
                        break;
                    case 2:
                        calification = GameCalification.Malo;
                        break;
                    case 3:
                        calification = GameCalification.Medio;
                        break;
                    case 4:
                        calification = GameCalification.Bueno;
                        break;
                    case 5:
                        calification = GameCalification.Muy_Bueno;
                        break;
                }
                catalogueView += "Calificacion media: " + calification.ToString() + Environment.NewLine;
                catalogueView += "" + Environment.NewLine;
            }
            return catalogueView;
        }
        public void AddGame(Game aGame)
        {
            this.Games.Add(aGame);
        }

        public Game FindGame(string gameTitle)
        {
            Game aGame = Games.Find(x => x.Title.Equals(gameTitle));
            return aGame;
        }

        public List<Game> FindAllGames(GameCalification calif)
        {
            List<Game> result = Games.FindAll(x => Math.Truncate(x.RatingAverage).Equals(x.CalificationToInt(calif)));
            return result;
        }

        public List<Game> FindAllGamesContaining(string message)
        {
            List<Game> result = Games.FindAll(x => x.Title.Contains(message));
            return result;
        }

        public List<Game> FindAllGamesByGender(string message)
        {
            List<Game> result = Games.FindAll(x => x.Gender.ToString().Equals(message));
            return result;
        }
    }
}

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
        public void DisplayGames()
        {
            Console.WriteLine("Catalogo de juegos: ");
            Console.WriteLine("");

            foreach (Game game in Games)
            {
                Console.WriteLine("Titulo: " + game.Title);
                Console.WriteLine("Sinopsis: " + game.Synopsis);
                Console.WriteLine("Categoría: " + game.Gender);
                GameCalification calification = GameCalification.PorDefecto;
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
                Console.WriteLine("Calificacion media: " + calification.ToString());
                Console.WriteLine("");
            }
        }
        public void AddGame(Game aGame)
        {
            this.Games.Add(aGame);
        }
    }
}

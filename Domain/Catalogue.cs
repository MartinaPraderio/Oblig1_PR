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

        public void AddGame(Game aGame)
        {
            this.Games.Add(aGame);
        }
    }
}

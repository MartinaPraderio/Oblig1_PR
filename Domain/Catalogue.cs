using System;
using System.Collections.Generic;

namespace Domain
{
    public class Catalogue
    {
        private List<Game> Games { get; set; }

        public Catalogue()
        {
            this.Games = new List<Game>();
        }

        public void AddGame(Game aGame)
        {
            this.Games.Add(aGame);
        }
    }
}

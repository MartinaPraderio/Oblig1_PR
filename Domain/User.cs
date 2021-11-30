    using System;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public List<Game> Games { get; set; }
        public string UserName { get; set; }

        public User() { }
        public User(string username)
        {
            this.UserName = username;
            this.Games = new List<Game>();
        }

        public void AddGame(Game aGame)
        {
            this.Games.Add(aGame);
        }
    }
}

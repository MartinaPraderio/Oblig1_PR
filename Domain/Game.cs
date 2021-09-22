using System;
using System.Collections.Generic;

namespace Domain
{
    public enum GameGender
    {
        Aventura,
        Accion,
        Estrategia
    }

    public class Game
    {
        public string Title { get; set; }
        public  GameGender Gender{ get; set; }
        public  List<UserRating> UserRatings { get; set; }
        public string Synopsis { get; set; }
        public string Cover { get; set; }

        public Game(string title, GameGender gender,string synopsis, string cover)
        {
            this.Title = title;
            this.Gender = gender;
            this.UserRatings = new List<UserRating>();
            this.Synopsis = synopsis;
            this.Cover = cover;
        }
    }
}

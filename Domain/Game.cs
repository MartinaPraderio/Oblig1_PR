using System;
using System.Collections.Generic;

namespace Domain
{
    public enum GameGender
    {
        Otros,
        Aventura,
        Accion,
        Estrategia,
        Infantil,
        Deporte
    }

    public class Game
    {
        public string Title { get; set; }
        public  GameGender Gender{ get; set; }
        public  List<UserRating> UserRatings { get; set; }
        public string Synopsis { get; set; }
        public string Cover { get; set; }

        public float RatingAverage { get; set; }

        public Game(string title, GameGender gender,string synopsis, string cover)
        {
            this.Title = title;
            this.Gender = gender;
            this.UserRatings = new List<UserRating>();
            this.Synopsis = synopsis;
            this.Cover = cover;
            UpdateRatingAverage();
        }

        public Game(string title, GameGender gender, string synopsis, string cover,List<UserRating> ratings)
        {
            this.Title = title;
            this.Gender = gender;
            this.UserRatings = ratings;
            this.Synopsis = synopsis;
            this.Cover = cover;
            UpdateRatingAverage();
        }

        public void AddRating(UserRating rating)
        {
            this.UserRatings.Add(rating);
            UpdateRatingAverage();
        }

        private void UpdateRatingAverage()
        {
            if(this.UserRatings.Count > 0) {
                int totalRating = 0;
                foreach (UserRating aRating in this.UserRatings)
                {
                    totalRating += CalificationToInt(aRating.Calification);
                }
                this.RatingAverage = totalRating / this.UserRatings.Count;
            }
        }

        public int CalificationToInt(GameCalification calification)
        {
            int result = 0;
            switch (calification.ToString())
            {
                case "Muy_Malo":
                    result = 1;
                    break;
                case "Malo":
                    result = 2;
                    break;
                case "Medio":
                    result = 3;
                    break;
                case "Bueno":
                    result = 4;
                    break;
                case "Muy_Bueno":
                    result = 5;
                    break;
            }
            return result;
        }
    }
}

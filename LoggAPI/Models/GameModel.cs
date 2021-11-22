using System;
using System.Collections.Generic;
using Domain;

namespace LoggAPI.Models
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

    public class GameModel : Mapper<Game, GameModel>
    {
        public string Title { get; set; }
        public GameGender Gender { get; set; }
        public List<UserRating> UserRatings { get; set; }
        public string Synopsis { get; set; }
        public string Cover { get; set; }

        public float RatingAverage { get; set; }

        public GameModel(){}

        public GameModel(Game entity)
        {
            MapToDto(entity);
        }

        public override Game MapToDomainObject()
        {
            Game game = new Game 
            {
                Title = this.Title,
                Gender = (Domain.GameGender)this.Gender,
                UserRatings = this.UserRatings,
                Synopsis = this.Synopsis,
                Cover = this.Cover,
                RatingAverage = this.RatingAverage
            };
            return game;
        }
        public override GameModel MapToDto(Game gameObject)
        {
            Title = gameObject.Title;
            Gender = (GameGender)gameObject.Gender;
            UserRatings = gameObject.UserRatings;
            Synopsis = gameObject.Synopsis;
            Cover = gameObject.Cover;
            RatingAverage = gameObject.RatingAverage;
            return this;
        }
    }
}

using System;
using Domain;

namespace LoggAPI.Models
{
    public enum GameCalification
    {
        Sin_Calificaciones,
        Muy_Malo,
        Malo,
        Medio,
        Bueno,
        Muy_Bueno
    }

    public class UserRatingModel : Mapper<UserRating, UserRatingModel>
    {
        public string Review { get; set; }
        public GameCalification Calification { get; set; }
        public User User { get; set; }

        public UserRatingModel() { }

        public UserRatingModel(UserRating entity)
        {
            MapToDto(entity);
        }

        public override UserRating MapToDomainObject()
        {
            UserRating userRating = new UserRating
            {
                Review = this.Review,
                Calification = (Domain.GameCalification)this.Calification,
                User = this.User,
            };
            return userRating;
        }
        public override UserRatingModel MapToDto(UserRating userRatingObject)
        {
            Review = userRatingObject.Review;
            Calification = (GameCalification)userRatingObject.Calification;
            User = userRatingObject.User;
            return this;
        }
    }
}



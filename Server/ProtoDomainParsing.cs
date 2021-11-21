using Domain;
using Google.Protobuf.Collections;
using ProtocolData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ProtoDomainParsing
    {
        public static Domain.Game ParseProtoGame(Game pGame)
        {
            string title = pGame.Title;
            GameGender gender = ProtocolDataProgram.ParseGameGender(pGame.Gender.ToString());
            string synopsis = pGame.Synopsis;
            string cover = pGame.Cover;
            List<Domain.UserRating> ratings = ParseProtoUserRating(pGame);
            return new Domain.Game(title, gender, synopsis, cover, ratings);
        }

        public static Game ParseDomainGame(Domain.Game dGame)
        {
            Game.Types.GameGender gdr = ParseDomainGameGender(dGame.Gender);
            RepeatedField<UserRating> pUserRatings = ParseDomainUserRatingList(dGame.UserRatings);
            return new Game {
                Title = dGame.Title,
                Gender = gdr,
                Synopsis = dGame.Synopsis,
                Cover = dGame.Cover,
                UserRatings = { pUserRatings }
            };
        }

        private static RepeatedField<UserRating> ParseDomainUserRatingList(List<Domain.UserRating> userRatings)
        {
            RepeatedField<UserRating> pUserRatingList = new RepeatedField<UserRating>();
            foreach (Domain.UserRating rating in userRatings)
            {
                pUserRatingList.Add(new UserRating
                {
                    Calification = ParseDomainGameCalification(rating.Calification),
                    Review = rating.Review,
                    User = new User { 
                        UserName = rating.User.UserName,
                        Games = { ParseDomainGameList(rating.User.Games) }
                    }
                });
            }
            return pUserRatingList;
        }

        private static RepeatedField<Game> ParseDomainGameList(List<Domain.Game> games)
        {
            RepeatedField<Game> pGames = new RepeatedField<Game>();
            foreach (Domain.Game dGame in games)
            {
                pGames.Add(ParseDomainGame(dGame));
            }
            return pGames;
        }

        private static UserRating.Types.GameCalification ParseDomainGameCalification(GameCalification calification)
        {
            List<UserRating.Types.GameCalification> pCalifications = new List<UserRating.Types.GameCalification>();
            pCalifications.Add(UserRating.Types.GameCalification.MuyMalo);
            pCalifications.Add(UserRating.Types.GameCalification.Malo);
            pCalifications.Add(UserRating.Types.GameCalification.Medio);
            pCalifications.Add(UserRating.Types.GameCalification.Bueno);
            pCalifications.Add(UserRating.Types.GameCalification.MuyBueno);
            pCalifications.Add(UserRating.Types.GameCalification.SinCalificaciones);
            UserRating.Types.GameCalification result = UserRating.Types.GameCalification.SinCalificaciones;
            foreach (UserRating.Types.GameCalification calif in pCalifications)
            {
                if (calif.ToString().Equals(calification.ToString()))
                {
                    result = calif;
                }
            }
            return result;
        }

        private static Game.Types.GameGender ParseDomainGameGender(GameGender gender)
        {
            List<Game.Types.GameGender> pGenders = new List<Game.Types.GameGender>();
            pGenders.Add(Game.Types.GameGender.Accion);
            pGenders.Add(Game.Types.GameGender.Aventura);
            pGenders.Add(Game.Types.GameGender.Deporte);
            pGenders.Add(Game.Types.GameGender.Estrategia);
            pGenders.Add(Game.Types.GameGender.Infantil);
            pGenders.Add(Game.Types.GameGender.Otros);
            Game.Types.GameGender result = Game.Types.GameGender.Otros;
            foreach (Game.Types.GameGender gdr in pGenders)
            {
                if (gdr.ToString().Equals(gender.ToString()))
                {
                    result = gdr;
                }
            }
            return result;
        }

        public static List<Domain.UserRating> ParseProtoUserRating(Game pGame)
        {
            List<Domain.UserRating> ratings = new List<Domain.UserRating>();
            foreach (UserRating pRating in pGame.UserRatings)
            {
                string review = pRating.Review;
                GameCalification parsedCalification = ProtocolDataProgram.ParseGameCalification(pRating.Calification.ToString());
                Domain.User reviewer = new Domain.User(pRating.User.UserName);
                ratings.Add(new Domain.UserRating(review, parsedCalification, reviewer));
            }

            return ratings;
        }
    }
}

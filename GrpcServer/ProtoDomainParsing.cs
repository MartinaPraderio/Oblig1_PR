using Domain;
using Google.Protobuf.Collections;
using ProtocolData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcServer
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
            if (dGame != null) {
                Game.Types.GameGender gdr = ParseDomainGameGender(dGame.Gender);
                RepeatedField<UserRating> pUserRatings = ParseDomainUserRatingList(dGame.UserRatings);
                return new Game
                {
                    Title = dGame.Title,
                    Gender = gdr,
                    Synopsis = dGame.Synopsis,
                    Cover = dGame.Cover,
                    UserRatings = { pUserRatings }
                };
            }
            else
            {
                return null;
            }
        }

        public static RepeatedField<UserRating> ParseDomainUserRatingList(List<Domain.UserRating> userRatings)
        {
            RepeatedField<UserRating> pUserRatingList = new RepeatedField<UserRating>();
            foreach (Domain.UserRating rating in userRatings)
            {
                pUserRatingList.Add(new UserRating
                {
                    Calification = ParseDomainGameCalification(rating.Calification),
                    Review = rating.Review,
                    User = new User
                    {
                        UserName = rating.User.UserName,
                        Games = { ParseDomainGameList(rating.User.Games) }
                    }
                });
            }
            return pUserRatingList;
        }

        public static RepeatedField<Game> ParseDomainGameList(List<Domain.Game> games)
        {
            RepeatedField<Game> pGames = new RepeatedField<Game>();
            foreach (Domain.Game dGame in games)
            {
                pGames.Add(ParseDomainGame(dGame));
            }
            return pGames;
        }

        public static UserRating.Types.GameCalification ParseDomainGameCalification(GameCalification calification)
        {
            UserRating.Types.GameCalification result = UserRating.Types.GameCalification.SinCalificaciones;
            switch (calification)
            {
                case GameCalification.Muy_Malo:
                    result = UserRating.Types.GameCalification.MuyMalo;
                    break;
                case GameCalification.Malo:
                    result = UserRating.Types.GameCalification.Malo;
                    break;
                case GameCalification.Medio:
                    result = UserRating.Types.GameCalification.Medio;
                    break;
                case GameCalification.Bueno:
                    result = UserRating.Types.GameCalification.Bueno;
                    break;
                case GameCalification.Muy_Bueno:
                    result = UserRating.Types.GameCalification.MuyBueno;
                    break;
                default:
                    result = UserRating.Types.GameCalification.SinCalificaciones;
                    break;
            }
            return result;
        }


        public static Game.Types.GameGender ParseDomainGameGender(GameGender gender)
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

        public static Domain.User ParseProtoUser(User user)
        {
            Domain.User u = new Domain.User(user.UserName);
            u.Games = ParseProtoGamesList(user.Games);
            return u;
        }

        public static List<Domain.Game> ParseProtoGamesList(RepeatedField<Game> games)
        {
            List<Domain.Game> dGames = new List<Domain.Game>();
            foreach (Game g in games)
            {
                dGames.Add(ParseProtoGame(g));
            }
            return dGames;
        }

        public static Catalogue ParseDomainCatalogue(List<Domain.Game> gameCatalogue)
        {
            RepeatedField<Game> pGames = new RepeatedField<Game>();
            foreach (Domain.Game dGame in gameCatalogue)
            {
                pGames.Add(ParseDomainGame(dGame));
            }
            return new Catalogue { Games = { pGames } };
        }

        public static RepeatedField<User> ParseDomainUserList(List<Domain.User> users)
        {
            RepeatedField<User> pUsers = new RepeatedField<User>();
            foreach (Domain.User dUser in users)
            {
                pUsers.Add(ParseDomainUser(dUser));
            }

            return pUsers;
        }

        public static User ParseDomainUser(Domain.User dUser)
        {
            return new User
            {
                UserName = dUser.UserName,
                Games = { ParseDomainGameList(dUser.Games) }
            };
        }
    }
}

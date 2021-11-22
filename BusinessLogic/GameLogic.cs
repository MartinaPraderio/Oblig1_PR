using BusinessLogic.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic
{
    public class GameLogic : IGameLogic
    {
        public void AddGame(Game game)
        {
            Repository.Lists.gameCatalogue.Add(game);
        }
        public void EditGame(string title, Game newGame)
        {
            Game gameToModify = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(title));
            gameToModify.Title = newGame.Title;
            gameToModify.Cover = newGame.Cover;
            gameToModify.Gender = newGame.Gender;
            gameToModify.RatingAverage = newGame.RatingAverage;
            gameToModify.Synopsis = newGame.Synopsis;
            gameToModify.UserRatings = newGame.UserRatings;
        }
        public void DeleteGame(string title)
        {
            Game game = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(title));
            Repository.Lists.gameCatalogue.Remove(game);
        }
    }
}

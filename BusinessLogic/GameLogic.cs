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
            Repository.Lists.games.Add(game);
        }
        public void EditGame(string title, Game newGame)
        {
            Game gameToModify = Repository.List.games.Find(x => x.Title.Equals(newGame));
            gameToModify.Title = newGame.Title;
            gameToModify.Cover = newGame.Cover;
            gameToModify.Gender = newGame.Gender;
            gameToModify.RatingAverage = newGame.RatingAverage;
            gameToModify.Synopsis = newGame.Synopsis;
            gameToModify.UserRatings = newGame.UserRatings;
        }
        public void DeleteGame(string title)
        {
            Game game = Repository.List.games.Find(x => x.Title.Equals(title));
            Repository.List.games.Remove(game);
        }
    }
}

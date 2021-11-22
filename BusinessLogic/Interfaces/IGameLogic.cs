using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Interfaces
{
    public interface IGameLogic
    {
        void AddGame(Game game);
        void EditGame(string title, Game newGame);
        void DeleteGame(string title);
    }
}

using BusinessLogic.Interfaces;
using Domain;
using System;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class UserLogic : IUserLogic
    {
        public void AddUser(User user)
        {
            if(user.Games == null)
            {
                user.Games = new List<Game>();
            }
            Repository.Lists.users.Add(user);
        }
        public void EditUser(string name, User newUser)
        {
            User userToModify = Repository.Lists.users.Find(x => x.UserName.Equals(name));
            userToModify.UserName = newUser.UserName;
            userToModify.Games = newUser.Games;
        }
        public void DeleteUser(string name)
        {
            User user = Repository.Lists.users.Find(x => x.UserName.Equals(name));
            Repository.Lists.users.Remove(user);
        }
        public void AssociateGame(string name, string gameTitle)
        {
            User user = Repository.Lists.users.Find(x => x.UserName.Equals(name));
            Game game = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(gameTitle));
            user.Games.Add(game);
        }
        public void DesassociateGame(string name, string gameTitle)
        {
            User user = Repository.Lists.users.Find(x => x.UserName.Equals(name));
            Game game = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(gameTitle));
            user.Games.Remove(game);
        }
    }
}

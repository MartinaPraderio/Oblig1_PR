using BusinessLogic.Interfaces;
using Domain;
using System;

namespace BusinessLogic
{
    public class UserLogic : IUserLogic
    {
        public void AddUser(User user)
        {
            //Repository.Lists.users.Add(user);
        }
        public void EditUser(string name, User newUser)
        {
            //User userToModify = Repository.List.users.Find(x => x.UserName.Equals(name));
            //userToModify.UserName = newUser.UserName;
            //userToModify.Games = newUser.Games;
        }
        public void DeleteUser(string name)
        {
            //User user = Repository.List.users.Find(x => x.UserName.Equals(name));
            //Repository.List.users.Remove(user);
        }
        public void AssociateGame(string name, string gameTitle)
        {
            //User user = Repository.List.users.Find(x => x.UserName.Equals(name));
            //Game game = Repository.List.games.Find(x => x.Title.Equals(gameTitle));
            //user.Games.Add(game);
        }
        public void DesassociateGame(string name, string gameTitle)
        {
            //User user = Repository.List.users.Find(x => x.UserName.Equals(name));
            //Game game = Repository.List.games.Find(x => x.Title.Equals(gameTitle));
            //user.Games.Remove(game);
        }
    }
}

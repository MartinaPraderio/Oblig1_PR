using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Interfaces
{
    public interface IUserLogic
    {
        void AddUser(User user);
        void EditUser(string name, User newUser);
        void DeleteUser(string name);
        void AssociateGame(string name, string gameTitle);
        void DesassociateGame(string name, string gameTitle);
    }
}

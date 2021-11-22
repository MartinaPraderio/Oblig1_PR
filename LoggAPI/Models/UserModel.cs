using System;
using System.Collections.Generic;
using Domain;

namespace LoggAPI.Models
{
    public class UserModel : Mapper<User, UserModel>
    {
       
        public List<Game> Games { get; set; }
        public string UserName { get; set; }

        public UserModel() { }

        public UserModel(User entity)
        {
            MapToDto(entity);
        }

        public override User MapToDomainObject()
        {
            User user = new User
            {
                Games = this.Games,
                UserName = this.UserName,
            };
            return user;
        }
        public override UserModel MapToDto(User userObject)
        {
            Games = userObject.Games;
            UserName = userObject.UserName;
            return this;
        }
    }
}

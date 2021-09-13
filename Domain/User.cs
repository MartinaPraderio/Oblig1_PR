using System;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public List<Game> Games { get; set; }
        public string UserName { get; set; }

        public User()
        {
        }
    }
}

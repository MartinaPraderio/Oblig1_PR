using System;
using System.Collections.Generic;

namespace Repository
{
    public static class Lists
    {
        public static List<Domain.Game> gameCatalogue = new List<Domain.Game>();
        //private Users users = new Users();
        //private RepeatedField<User> users = new RepeatedField<User>();
        public static List<Domain.User> users = new List<Domain.User>();
        public static List<Domain.User> loggedUsers = new List<Domain.User>();

    }
}

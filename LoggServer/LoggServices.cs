using System;
using System.Collections.Generic;

namespace LoggServer
{
    public class LoggServices
    {
        private List<Logg> loggs = new List<Logg>();

        public IEnumerable<Logg> GetAll(string user, string game, string date)
        {
            if (user != null)
                loggs = loggs.FindAll(x => x.User.Equals(user));
            if (game != null)
                loggs = loggs.FindAll(x => x.Game.Equals(user));
            if (date != null)
                loggs = loggs.FindAll(x => x.Date.Equals(user));
            return loggs;
        }
    }
}

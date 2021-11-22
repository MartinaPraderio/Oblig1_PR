using Domain;
using LoggAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggAPI.Services
{
    public class LoggServices : ILoggServices
    {
        private readonly static LoggServices _instance = new LoggServices();
        public static LoggServices Instance()
        {
            return _instance;
        }
        //public List<Logg> loggs = new List<Logg>();

        public void AddLogg(Logg logg)
        {
            Repository.Lists.loggs.Add(logg);
        }
        public IEnumerable<Logg> GetAll(string user, string game, DateTime date)
        {
            List<Logg> result = new List<Logg>();
            Messages.ReceiveMessages();
            if (user != null)
                ConcatLists(result, Repository.Lists.loggs.FindAll(x => (x.User != null) && x.User.Equals(user)));
            if (game != null)
                ConcatLists(result, Repository.Lists.loggs.FindAll(x => (x.Game != null) && x.Game.Equals(game)));
            if (date != null)
                ConcatLists(result, Repository.Lists.loggs.FindAll(x => (x.Date != null)&& x.Date.Date.Equals(date)));
            return result;
        }

        private void ConcatLists(List<Logg> a, List<Logg> b)
        {
            foreach (Logg logg in b)
            {
                a.Add(logg);
            }
        }
    }
}

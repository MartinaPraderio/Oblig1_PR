using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggAPI.Interfaces
{
    public interface ILoggServices
    {
        public void AddLogg(Logg logg);
        IEnumerable<Logg> GetAll(string user, string game, DateTime date);
    }
}

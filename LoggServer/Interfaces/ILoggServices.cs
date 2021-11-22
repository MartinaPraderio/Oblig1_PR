using System;
using System.Collections.Generic;
using System.Text;

namespace LoggServer.Interfaces
{
    public interface ILoggServices
    {
        public void AddLogg(Logg logg);
        IEnumerable<Logg> GetAll(string user, string game, string date);
    }
}


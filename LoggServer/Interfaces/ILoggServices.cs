using System;
using System.Collections.Generic;
using System.Text;

namespace LoggServer.Interfaces
{
    public interface ILoggServices
    {
        IEnumerable<Logg> GetAll(string user, string game, string date);
    }
}

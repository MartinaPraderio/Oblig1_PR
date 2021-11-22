using LoggServer.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LoggServer
{
    public class LoggServices : ILoggServices
    {
        private readonly static LoggServices _instance = new LoggServices();
        public static LoggServices Instance()
        {
            return _instance;
        }
        public List<Logg> loggs = new List<Logg>();

        public void AddLogg(Logg logg)
        {
            loggs.Add(logg);
        }
        public IEnumerable<Logg> GetAll(string user, string game, string date)
        {
            LoggProgram.ReceiveMessages();
            if (user != null)
                loggs = loggs.FindAll(x => x.User.Equals(user));
            if (game != null)
                loggs = loggs.FindAll(x => x.Game.Equals(user));
            if (date != null)
                //DateTime.Now.ToString("MM/dd/yyyy HH:mm") El .Date es para agarrar solo la fecha no la hora
                loggs = loggs.FindAll(x => x.Date.Date.ToString().Equals(user));
            return loggs;
        }
    }
}

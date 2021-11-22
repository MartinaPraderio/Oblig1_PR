using  Domain;
using System;

namespace LoggAPI.Models
{
    public class LoggModel : Mapper<Logg, LoggModel>
    {
        public string Game { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }

        public LoggModel() { }

        public LoggModel(Logg entity)
        {
            MapToDto(entity);
        }
        public override Logg MapToDomainObject()
        {
            Logg logg = new Logg
            {
                Game = this.Game,
                User = this.User,
                Date = this.Date,
                Action = this.Action
            };
            return logg;
        }
        public override LoggModel MapToDto(Logg loggObject)
        {
            Game = loggObject.Game;
            User = loggObject.User;
            Date = loggObject.Date;
            Action = loggObject.Action;
            return this;
        }
    }
}

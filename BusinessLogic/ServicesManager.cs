using System;
using Domain;

namespace BusinessLogic
{
    public abstract class ServicesManager
    {
        public abstract void PublishGame();
        public abstract void QualifyGame();
        public abstract Game SearchGame();
        public abstract void GameDetails();
    }
}

using System;
using System.Net.Sockets;
using Domain;

namespace BusinessLogic
{
    public abstract class ServicesManager
    {
        public abstract void PublishGame(Socket socket);
        public abstract void QualifyGame();
        public abstract Game SearchGame();
        public abstract void GameDetails();
    }
}

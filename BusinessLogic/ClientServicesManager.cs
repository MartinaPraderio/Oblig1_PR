using System;
using Domain;

namespace BusinessLogic

{
    public class ClientServicesManager : ServicesManager
    {
        public override void PublishGame() { }
        public override void QualifyGame() { }
        public override Game SearchGame() { return new Game(); }
        public override void GameDetails() { }

        public void RequestServerConnection(){}
        public void EndServerConnection(){}
        public void ModifyGame(){}
        public void DeleteGame(){}
    }
}


using System;
using Domain;

namespace BusinessLogic
{
    public class ServerServicesManager: ServicesManager
    {
        public override void PublishGame() { }
        public override void QualifyGame() { }
        public override Game SearchGame() { return new Game(); }
        public override void GameDetails() { }

        public void AcceptClient(){}
        public void ViewCatalogue(){}
        public void BuyGame(){}
    }
}

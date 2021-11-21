using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class ProviderService : Provider.ProviderBase
    {
        private ILogger<ProviderService> _logger;

        private Catalogue gameCatalogue = new Catalogue();
        private List<User> users = new List<User>();
        private List<User> loggedUsers = new List<User>();
        public ProviderService(ILogger<ProviderService> logger)
        {
            _logger = logger;
        }
        public override Task<User> SendUser(User user, Grpc.Core.ServerCallContext context)
        {
            lock (this.users)
            {
                this.users.Add(user);
            }
            Console.WriteLine("user agregado " + users.First().ToString());
            RepeatedField<GrpcServer.ListOfGames> userGames = user.Games;
            return Task.FromResult(new User
            {
                UserName = user.UserName,
                Games = { userGames }
            });
        }
        public override Task<User> GetUser(InfoRequest userName, Grpc.Core.ServerCallContext context)
        {
            User aUser;
            lock (this.users)
            {
              aUser = this.users.Find(x => x.UserName.Equals(userName));
            }
            RepeatedField<GrpcServer.ListOfGames> userGames = aUser.Games;
            return Task.FromResult(new User
            {
                UserName = aUser.UserName,
                Games = { userGames }
            });
        }

        public override Task<InfoRequest> ModifyUser(InfoRequest userName, Grpc.Core.ServerCallContext context)
        {
            User aUser;
            lock (this.users)
            {
                aUser = this.users.Find(x => x.UserName.Equals(userName));
                aUser.UserName = userName.Info;
            }
            return Task.FromResult(new InfoRequest
            {
                Info = "El usuario fue modificado con exito."
            });
        }
        public override Task<Game> SendGame(Game game, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.ListOfUserRating> gameUserRatings = game.UserRatings;
            return Task.FromResult(new Game
            {
                Title = game.Title,
                Gender = game.Gender,
                UserRatings = { gameUserRatings },
                Synopsis = game.Synopsis,
                Cover = game.Cover
            }); 
        }
    }
}
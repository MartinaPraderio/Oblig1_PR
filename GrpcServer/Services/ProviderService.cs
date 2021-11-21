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
        private Users users = new Users();
        private Users loggedUsers = new Users();
        public ProviderService(ILogger<ProviderService> logger)
        {
            _logger = logger;
        }
        public override Task<User> SendUser(User user, Grpc.Core.ServerCallContext context)
        {
            lock (this.users)
            {
                this.users.Users_.Add(user);
            }
            Console.WriteLine("user agregado " + users.Users_.First().ToString());
            RepeatedField<GrpcServer.Game> userGames = user.Games;
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
                aUser = this.users.Users_.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
            }
            RepeatedField<GrpcServer.Game> userGames = aUser.Games;
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
                aUser = this.users.Users_.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
                aUser.UserName = userName.Info;
            }
            return Task.FromResult(new InfoRequest
            {
                Info = "El usuario fue modificado con exito."
            });
        }

        public override Task<InfoRequest> DeleteUser(InfoRequest userName, Grpc.Core.ServerCallContext context)
        {
            string response = "";
            User aUser;
            lock (this.users)
            {
                aUser = this.users.Users_.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
            }
            User userLogged;
            lock (this.loggedUsers)
            {
                 userLogged = this.loggedUsers.Users_.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
            }
            if (aUser != null)
            {
                if (userLogged != null)
                {
                    response = "No es posible eliminar este usuario ya que se encuentra loggeado";
                }
                else
                {
                    this.users.Users_.Remove(aUser);
                    response = "El usuario fue eliminado con exito.";
                }
            }
            else
            {
                response = "El usuario que intenta eliminar no existe.";
            }           
            return Task.FromResult(new InfoRequest
            {
                Info = response
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

        public override Task<InfoRequest> DeleteGame(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            lock (this.gameCatalogue.Games)
            {
                Game aGame = gameCatalogue.Games.Where(x => x.Title.Equals(request.Info)).FirstOrDefault();

                string response = "";
                if (aGame != null)
                {

                    this.gameCatalogue.Games.Remove(aGame);
                    response = "El juego fue eliminado.";
                }
                else
                {
                    response = "El juego que quiere eliminar no existe";
                }
                return Task.FromResult(new InfoRequest { Info = response });
            }
        }
        public override Task<Catalogue> GetCatalogue(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.Game> catalogueGames = this.gameCatalogue.Games;
            return Task.FromResult(new Catalogue
            {
                Games = { catalogueGames }
            });
        }
        public override Task<Users> GetUsers(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.User> users = this.users.Users_;
            return Task.FromResult(new Users
            {
                Users_ = { users }
            });
        }
        
    }
}
using Google.Protobuf.Collections;
using Grpc.Core;
using LoggServer;
using Microsoft.Extensions.Logging;
using ProtocolData;
using RabbitMQ.Client;
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
        public override Task<InfoRequest> LoadTestData(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            UserRating userRating1 = new UserRating
            {
                Review = "Muy buen juego",
                Calification = UserRating.Types.GameCalification.Bueno,
                User = new User
                {
                    UserName = "JuanPrueba"
                }
            };
            UserRating userRating2 = new UserRating
            {
                Review = "No es compatible con mi pc",
                Calification = UserRating.Types.GameCalification.MuyMalo,
                User = new User
                {
                    UserName = "PacoPrueba"
                }
            };
            RepeatedField<GrpcServer.UserRating> gameUserRatings = new RepeatedField<UserRating>();
            gameUserRatings.Add(userRating1);
            gameUserRatings.Add(userRating2);

            Game fifa = new Game
            {
                Title = "Fifa 21 Prueba",
                Gender = Game.Types.GameGender.Deporte,
                UserRatings = { gameUserRatings },
                Synopsis = "Futbol actual",
                Cover = "fifa.jpg"
            };
            Game callOfDuty = new Game
            {
                Title = "Call of duty Prueba",
                Gender = Game.Types.GameGender.Accion,
                Synopsis = "Shooter",
                Cover = "COD.jpg"
            };
            Game MarioBros = new Game
            {
                Title = "Mario Bros",
                Gender = Game.Types.GameGender.Aventura,
                UserRatings = { gameUserRatings },
                Synopsis = "juego de nintendo",
                Cover = "mario.jpg"
            };
            lock (this.gameCatalogue.Games)
            {
                gameCatalogue.Games.Add(fifa);
                gameCatalogue.Games.Add(callOfDuty);
                gameCatalogue.Games.Add(MarioBros);
            }
            return Task.FromResult(new InfoRequest
            {
                Info = "Datos de prueba cargados correctamente"
            });
        }
        public override Task<InfoRequest> Login( InfoRequest username, Grpc.Core.ServerCallContext context)
        {
            string response = "";
            lock (this.users.Users_)
            {
                User user = this.users.Users_.Where(x => x.UserName.Equals(username)).FirstOrDefault();

                if (user != null)
                {
                    lock (this.loggedUsers)
                    {
                        if (this.loggedUsers.Users_.Where(x => x.UserName.Equals(username)).FirstOrDefault() != null)
                        {
                            response = "Ya existe una sesion con este usuario ";
                        }
                        else
                        {
                            response = "Login exitoso";
                            this.loggedUsers.Users_.Add(user);
                        }
                    }
                }
                else
                {
                    response = "El usuario que ingresó no existe";
                }
            }
            Logg logg = new Logg
            {
                User = username.Info,
                Action = response,
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new InfoRequest
            {
                Info = response
            });
        }
        public override Task<InfoRequest> Logout (InfoRequest username, Grpc.Core.ServerCallContext context)
        {
            string response = "";
            lock (this.loggedUsers.Users_)
            {
                User user = this.loggedUsers.Users_.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                if (user != null)
                {
                    loggedUsers.Users_.Remove(user);
                    response = "Sesion terminada exitosamente";
                }
                else
                {
                    response = "Este usuario no existe en el sistema";
                }
            }
            Logg logg = new Logg
            {
                User = username.Info,
                Action = response,
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new InfoRequest
            {
                Info = response
            });
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
            lock (this.gameCatalogue.Games)
            {
                this.gameCatalogue.Games.Add(game);
            }
            RepeatedField<GrpcServer.UserRating> gameUserRatings = game.UserRatings;
            return Task.FromResult(new Game
            {
                Title = game.Title,
                Gender = game.Gender,
                UserRatings = { gameUserRatings },
                Synopsis = game.Synopsis,
                Cover = game.Cover
            });
        }

        public override Task<Game> GetGame(InfoRequest gameTitle, Grpc.Core.ServerCallContext context)
        {
            Game aGame;
            lock (this.gameCatalogue.Games)
            {
                aGame = this.gameCatalogue.Games.Where(x => x.Title.Equals(gameTitle)).FirstOrDefault();
            }
            RepeatedField<GrpcServer.UserRating> gameUserRatings = aGame.UserRatings;
            return Task.FromResult(new Game
            {
                Title = aGame.Title,
                Gender = aGame.Gender,
                UserRatings = { gameUserRatings },
                Synopsis = aGame.Synopsis,
                Cover = aGame.Cover
            });
        }

        public override Task<InfoRequest> ModifyGame(InfoRequest game, Grpc.Core.ServerCallContext context)
        {
            string[] info = game.Info.Split(Environment.NewLine);

            lock (this.gameCatalogue.Games)
            {
                Game aGame = gameCatalogue.Games.Where(x => x.Title.Equals(info[0])).FirstOrDefault();
                string response = "";
                if (aGame != null)
                {
                    Game newGameValues; // = JSON.stringify(info[1]);
                    //Game newGameValues = ProtocolDataProgram.DeserializeGame(info[1]);

                    //aGame.Title = newGameValues.Title;
                    //aGame.Gender = newGameValues.Gender;
                    //aGame.Synopsis = newGameValues.Synopsis;
                    //response = "El juego fue modificado.";
                }
                else
                {
                    response = "El juego que quiere modificar no existe";
                }
                return Task.FromResult(new InfoRequest
                {
                    Info = response
                });
            }
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
        public override Task<InfoRequest> BuyGame(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            string[] info = request.Info.Split(Environment.NewLine);
            string buyerName = info[0];
            string gameName = info[1];
            lock (this.gameCatalogue.Games)
            {
                string response = "";

                Game requestedGame = gameCatalogue.Games.Where(x => x.Title.Equals(gameName)).FirstOrDefault();
                if (requestedGame != null)
                {
                    lock (this.users)
                    {
                        User buyer = this.users.Users_.Where(x => x.UserName.Equals(buyerName)).FirstOrDefault();
                        buyer.Games.Add(requestedGame);
                    }
                    response = "Su compra ha finalizado con exito";
                }
                else
                {
                    response = "El juego que desea adquirir no existe." + Environment.NewLine +
                        "Asegurese de ingresar el nombre correctamente.";
                }
                return Task.FromResult(new InfoRequest
                {
                    Info = response
                });
            }
        }
        public override Task<InfoRequest> QualifyGame (InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            string[] info = request.Info.Split(Environment.NewLine);
            string userName = info[0];
            string gameTitle = info[1];

            UserRating.Types.GameCalification calification;
            if(info[2] == "Bueno")
            {
                calification = UserRating.Types.GameCalification.Bueno;
            }
            else if (info[2] == "Malo")
            {
                calification = UserRating.Types.GameCalification.Malo;
            }
            else if (info[2] == "Medio")
            {
                calification = UserRating.Types.GameCalification.Medio;
            }
            else if (info[2] == "MuyBueno")
            {
                calification = UserRating.Types.GameCalification.MuyBueno;
            }
            else if (info[2] == "MuyMalo")
            {
                calification = UserRating.Types.GameCalification.MuyMalo;
            } else
            {
                calification = UserRating.Types.GameCalification.SinCalificaciones;
            }
            string review = info[3];
            lock (this.gameCatalogue.Games)
            {
                Game gameToQualify = gameCatalogue.Games.Where(x => x.Title.Equals(gameTitle)).FirstOrDefault();
                string addRatingResponse = "";
                if (gameToQualify != null)
                {
                    User reviewer = this.users.Users_.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
                    UserRating newRating = new UserRating
                    {
                        Review = review,
                        Calification = calification,
                        User = reviewer
                    };
                    gameToQualify.UserRatings.Add(newRating);
                    addRatingResponse = "Su review fue publicada con exito.";
                }
                else
                {
                    addRatingResponse = "El juego que desea calificar no existe." + Environment.NewLine +
                        "Asegurese de ingresar el nombre correctamente.";
                }
                return Task.FromResult(new InfoRequest
                {
                    Info = addRatingResponse
                });
            }
        }
        public override Task<Games> GetGamesContaining(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.Game> games = (RepeatedField<GrpcServer.Game>)gameCatalogue.Games.Where(x => x.Title.Equals(request.Info));
            return Task.FromResult(new Games
            {
                Games_ = { games }
            });
        }
        public override Task<Games> GetGamesByGender(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.Game> games = (RepeatedField<GrpcServer.Game>)gameCatalogue.Games.Where(x => x.Gender.ToString().Equals(request.Info));
            return Task.FromResult(new Games
            {
                Games_ = { games }
            });
        }
        public override Task<Games> GetGamesByCalification(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.Game> games = (RepeatedField<GrpcServer.Game>)gameCatalogue.Games.Where((x => Math.Truncate(x.RatingAverage).ToString().Equals(request.Info)));
            return Task.FromResult(new Games
            {
                Games_ = { games }
            });
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
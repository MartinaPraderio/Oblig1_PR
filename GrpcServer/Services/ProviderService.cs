using Google.Protobuf.Collections;
using Grpc.Core;
using Domain;
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

        public ProviderService(ILogger<ProviderService> logger)
        {
            _logger = logger;
        }
        public override Task<InfoRequest> LoadTestData(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Domain.User juan = new Domain.User("JuanPrueba");
            Domain.User paco = new Domain.User("PacoPrueba");
            Domain.UserRating userRating1 = new Domain.UserRating("Muy buen juego", Domain.GameCalification.Bueno, juan);
            Domain.UserRating userRating2 = new Domain.UserRating("No es compatible con mi pc", Domain.GameCalification.Muy_Malo, paco);
            List<Domain.UserRating> gameUserRatings = new List<Domain.UserRating>();
            gameUserRatings.Add(userRating1);
            gameUserRatings.Add(userRating2);

            Domain.Game fifa = new Domain.Game("Fifa 21 Prueba", Domain.GameGender.Deporte, "Futbol actual", "fifa.jpg", gameUserRatings);
            Domain.Game callOfDuty = new Domain.Game("Call of duty Prueba", Domain.GameGender.Accion, "Shooter", "COD.jpg", gameUserRatings);
            Domain.Game MarioBros = new Domain.Game("Mario Bros", Domain.GameGender.Aventura, "juego de nintendo", "mario.jpg", gameUserRatings);
            lock (Repository.Lists.gameCatalogue)
            {
                Repository.Lists.gameCatalogue.Add(fifa);
                Repository.Lists.gameCatalogue.Add(callOfDuty);
                Repository.Lists.gameCatalogue.Add(MarioBros);
            }
            return Task.FromResult(new InfoRequest
            {
                Info = "Datos de prueba cargados correctamente"
            });
        }
        public override Task<InfoRequest> Login( InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            lock (Repository.Lists.users)
            {
                Domain.User user = Repository.Lists.users.Find(x => x.UserName.Equals(request.Info));

                string response = "";
                if (user != null)
                {
                    lock (Repository.Lists.loggedUsers)
                    {
                        if (Repository.Lists.loggedUsers.Find(x => x.UserName.Equals(request.Info)) != null)
                        {
                            response = "Ya existe una sesion con este usuario ";
                        }
                        else
                        {
                            response = "Login exitoso";
                            Repository.Lists.loggedUsers.Add(user);
                        }
                    }
                }
                else
                {
                    response = "El usuario que ingresó no existe";
                }
                Logg logg = new Logg
                {
                    User = request.Info,
                    Action = response,
                    Date = DateTime.Now
                };
                Program.PublishMessage(ChannelComunication._channel, logg);
                return Task.FromResult(new InfoRequest
                {
                    Info = response
                });
            }
        }
        public override Task<InfoRequest> Logout (InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            string response = "";
            lock (Repository.Lists.loggedUsers)
            {
                Domain.User user = Repository.Lists.loggedUsers.Find(x => x.UserName.Equals(request.Info));
                if (user != null)
                {
                    Repository.Lists.loggedUsers.Remove(user);
                    response = "Sesion terminada exitosamente";
                }
                else
                {
                    response = "Este usuario no existe en el sistema";
                }
            }
            Logg logg = new Logg
            {
                User = request.Info,
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
            lock (Repository.Lists.users)
            {
                Repository.Lists.users.Add(ProtoDomainParsing.ParseProtoUser(user));
            }
            RepeatedField<GrpcServer.Game> userGames = user.Games;
            Logg logg = new Logg
            {
                User = user.UserName,
                Game = "",
                Action = "Usuario agregado al sistema",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new User
            {
                UserName = user.UserName,
                Games = { userGames }
            });
        }       
        public override Task<User> GetUser(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Domain.User aUser;
            lock (Repository.Lists.users)
            {
                aUser = Repository.Lists.users.Find(x => x.UserName.Equals(request.Info));
            }
            
            RepeatedField<GrpcServer.Game> userGames = ProtoDomainParsing.ParseDomainGameList(aUser.Games);
            Logg logg = new Logg
            {
                User = request.Info,
                Action = "Obtencion de usuario del sistema",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new User
            {
                UserName = aUser.UserName,
                Games = { userGames }
            });
        }

        public override Task<InfoRequest> ModifyUser(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Domain.User aUser;
            string[] names = request.Info.Split(Environment.NewLine);
            lock (Repository.Lists.users)
            {
                aUser = Repository.Lists.users.Find(x => x.UserName.Equals(names[0]));
                aUser.UserName = names[1];
            }
            Logg logg = new Logg
            {
                User = request.Info,
                Action = "Modificacion del usuario del sistema",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new InfoRequest
            {
                Info = "El usuario fue modificado con exito."
            });
        }

        public override Task<InfoRequest> DeleteUser(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            string response = "";
            Domain.User aUser;
            lock (Repository.Lists.users)
            {
                aUser = Repository.Lists.users.Find(x => x.UserName.Equals(request.Info));
            }
            Domain.User userLogged;
            lock (Repository.Lists.loggedUsers)
            {
                 userLogged = Repository.Lists.loggedUsers.Find(x => x.UserName.Equals(request.Info));
            }
            if (aUser != null)
            {
                if (userLogged != null)
                {
                    response = "No es posible eliminar este usuario ya que se encuentra loggeado";
                }
                else
                {
                    Repository.Lists.users.Remove(aUser);
                    response = "El usuario fue eliminado con exito.";
                }
            }
            else
            {
                response = "El usuario que intenta eliminar no existe.";
            }
            Logg logg = new Logg
            {
                User = request.Info,
                Action = response,
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new InfoRequest
            {
                Info = response
            });
        }
        public override Task<Game> SendGame(Game game, Grpc.Core.ServerCallContext context)
        {
            lock (Repository.Lists.gameCatalogue)
            {
                Repository.Lists.gameCatalogue.Add(ProtoDomainParsing.ParseProtoGame(game));
            }
            RepeatedField<GrpcServer.UserRating> gameUserRatings = game.UserRatings;
            Logg logg = new Logg
            {
                Game = game.Title,
                Action = "Juego agregado al sistema",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new Game
            {
                Title = game.Title,
                Gender = game.Gender,
                UserRatings = { gameUserRatings },
                Synopsis = game.Synopsis,
                Cover = game.Cover
            });
        }

        public override Task<Game> GetGame(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Domain.Game aGame;
            lock (Repository.Lists.gameCatalogue)
            {
                aGame = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(request.Info));
            }
            if (aGame == null)
            { 
                return Task.FromResult(new Game
                {

                });
            }
            else
            {
                Logg logg = new Logg
                {
                    Game = aGame.Title,
                    Action = "Juego obtenido del sistema",
                    Date = DateTime.Now
                };
                Program.PublishMessage(ChannelComunication._channel, logg);
                return Task.FromResult(ProtoDomainParsing.ParseDomainGame(aGame));
            }
        }

        public override Task<InfoRequest> ModifyGame(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            string[] info = request.Info.Split(Environment.NewLine);

            lock (Repository.Lists.gameCatalogue)
            {
                Domain.Game aGame = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(info[0]));
                string response = "";
                if (aGame != null)
                {
                    //Game newGameValues; // = JSON.stringify(info[1]);
                    Domain.Game newGameValues = ProtocolDataProgram.DeserializeGame(info[1]);

                    aGame.Title = newGameValues.Title;
                    aGame.Gender = newGameValues.Gender;
                    aGame.Synopsis = newGameValues.Synopsis;
                    response = "El juego fue modificado.";
                    Logg logg = new Logg
                    {
                        Game = aGame.Title,
                        Action = response,
                        Date = DateTime.Now
                    };
                    Program.PublishMessage(ChannelComunication._channel, logg);
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
            lock (Repository.Lists.gameCatalogue)
            {
                Domain.Game aGame = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(request.Info));

                string response = "";
                if (aGame != null)
                {
                    Repository.Lists.gameCatalogue.Remove(aGame);
                    response = "El juego fue eliminado.";
                    Logg logg = new Logg
                    {
                        Game = aGame.Title,
                        Action = response,
                        Date = DateTime.Now
                    };
                    Program.PublishMessage(ChannelComunication._channel, logg);
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
            lock (Repository.Lists.gameCatalogue)
            {
                string response = "";

                Domain.Game requestedGame = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(gameName));
                if (requestedGame != null)
                {
                    lock (Repository.Lists.users)
                    {
                        Domain.User buyer = Repository.Lists.users.Find(x => x.UserName.Equals(buyerName));
                        buyer.Games.Add(requestedGame);
                    }
                    response = "Su compra ha finalizado con exito";
                }
                else
                {
                    response = "El juego que desea adquirir no existe." + Environment.NewLine +
                        "Asegurese de ingresar el nombre correctamente.";
                }
                Logg logg = new Logg
                {
                    Game = gameName,
                    User = buyerName,
                    Action = response,
                    Date = DateTime.Now
                };
                Program.PublishMessage(ChannelComunication._channel, logg);
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

            Domain.GameCalification calification;
            if(info[2] == "Bueno")
            {
                calification = Domain.GameCalification.Bueno;
            }
            else if (info[2] == "Malo")
            {
                calification = Domain.GameCalification.Malo;
            }
            else if (info[2] == "Medio")
            {
                calification = Domain.GameCalification.Medio;
            }
            else if (info[2] == "MuyBueno")
            {
                calification = Domain.GameCalification.Muy_Bueno;
            }
            else if (info[2] == "MuyMalo")
            {
                calification = Domain.GameCalification.Muy_Malo;
            } else
            {
                calification = Domain.GameCalification.Sin_Calificaciones;
            }
            string review = info[3];
            lock (Repository.Lists.gameCatalogue)
            {
                Domain.Game gameToQualify = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(gameTitle));
                string addRatingResponse = "";
                if (gameToQualify != null)
                {
                    Domain.User reviewer = Repository.Lists.users.Find(x => x.UserName.Equals(userName));
                    Domain.UserRating newRating = new Domain.UserRating(review, calification,reviewer);
                    gameToQualify.UserRatings.Add(newRating);
                    addRatingResponse = "Su review fue publicada con exito.";
                    Logg logg = new Logg
                    {
                        Game = gameToQualify.Title,
                        User = reviewer.UserName,
                        Action = addRatingResponse,
                        Date = DateTime.Now
                    };
                    Program.PublishMessage(ChannelComunication._channel, logg);
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
            List<Domain.Game> games = Repository.Lists.gameCatalogue.FindAll(x => x.Title.Equals(request.Info));
            Games pGames = new Games
            {
                Games_ = { ProtoDomainParsing.ParseDomainGameList(games) }
            };
            Logg logg = new Logg
            {
                Game = request.Info,
                Action = "Obtencion de lista de juegos por titulo",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(pGames);
        }
        public override Task<Games> GetGamesByGender(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            List<Domain.Game> games = Repository.Lists.gameCatalogue.FindAll(x => x.Gender.ToString().Equals(request.Info));
            Logg logg = new Logg
            {
                Game = request.Info,
                Action = "Obtencion de la lista de juegos por genero",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new Games
            {
                Games_ = { ProtoDomainParsing.ParseDomainGameList(games)}
            });
        }
        public override Task<Games> GetGamesByCalification(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            List<Domain.Game> games = Repository.Lists.gameCatalogue.FindAll((x => Math.Truncate(x.RatingAverage).ToString().Equals(request.Info)));
            Logg logg = new Logg
            {
                Game = request.Info,
                Action = "Obtencion de la lista de juegos por calificacion",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new Games
            {
                Games_ = { ProtoDomainParsing.ParseDomainGameList(games)}
            });
        }
        public override Task<Catalogue> GetCatalogue(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Logg logg = new Logg
            {
                Action = "Obtencion del catalogo de juegos",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(ProtoDomainParsing.ParseDomainCatalogue(Repository.Lists.gameCatalogue));
        }
        public override Task<Users> GetUsers(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            RepeatedField<GrpcServer.User> users = ProtoDomainParsing.ParseDomainUserList(Repository.Lists.users);
            Logg logg = new Logg
            {
                Action = "Obtencion de la lista de usuarios",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new Users
            {
                Users_ = { users }
            });
        }

        public override Task<InfoRequest> ExistsUser(InfoRequest request, Grpc.Core.ServerCallContext context)
        {
            Domain.User aUser = new Domain.User();
            lock (Repository.Lists.users)
            {
                aUser = Repository.Lists.users.Find(x => x.UserName.Equals(request.Info));
            }
            string response = "";
            if(aUser == null)
            {
                response = "false";
            }
            else
            {
                Logg logg = new Logg
                {
                    User = request.Info,
                    Action = "Existencia del usuario",
                    Date = DateTime.Now
                };
                Program.PublishMessage(ChannelComunication._channel, logg);
                response = "true";
            }
            return Task.FromResult(new InfoRequest
            {
                Info = response
            }) ;
        }
        public override Task<InfoRequest> UpdateGame(Game game, Grpc.Core.ServerCallContext context)
        {
            lock (Repository.Lists.gameCatalogue)
            {
                Domain.Game gameToUpdate = Repository.Lists.gameCatalogue.Find(x => x.Title.Equals(game.Title));
                Repository.Lists.gameCatalogue.Remove(gameToUpdate);
                Repository.Lists.gameCatalogue.Add(ProtoDomainParsing.ParseProtoGame(game));
            }
            Logg logg = new Logg
            {
                Game = game.Title,
                Action = "Juego modificado",
                Date = DateTime.Now
            };
            Program.PublishMessage(ChannelComunication._channel, logg);
            return Task.FromResult(new InfoRequest
            {
                Info = "Actualizado"
            });
        }

    }
}
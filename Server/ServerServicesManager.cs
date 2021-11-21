using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf.Collections;

namespace Server
{
    public class ServerServicesManager
    {
        private readonly static ServerServicesManager _instance = new ServerServicesManager();
        private Domain.Catalogue gameCatalogue = Domain.Catalogue.Instance;
        private List<Domain.User> users = new List<Domain.User>();
        private List<Domain.User> loggedUsers = new List<Domain.User>();
        private TcpListener _tcpListener;
        FileCommunicationHandler fileCommunication;
        private Provider.ProviderClient grpcClient;

        public static ServerServicesManager Instance()
        {
            return _instance;
        }

        public void SetTcpListener(TcpListener tcpListener)
        {
            _tcpListener = tcpListener;
        }

        public async Task<string> PublishGameAsync(Game newGame, TcpClient tcpClient) {
            this.fileCommunication = new FileCommunicationHandler(tcpClient);
            await fileCommunication.ReceiveFileAsync();

            RepeatedField<UserRating> gameUserRatings = newGame.UserRatings;

            var reply = await grpcClient.SendGameAsync(new Game
            {
                Title = newGame.Title,
                Gender = newGame.Gender,
                UserRatings = { gameUserRatings },
                Synopsis = newGame.Synopsis,
                Cover = newGame.Cover
            });
            return "Juego agregado con exito";
        }

        public async Task<string> GameDetails(string gameTitle) {
            string response = "";

            var game = await grpcClient.GetGameAsync(new InfoRequest
            {
                Info = gameTitle
            });

            if (game != null)
            {
               // response = "E" + Environment.NewLine + ProtocolDataProgram.SerializeGame(game);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
             return response;
        }

        public async Task<string> ViewCatalogue(){
            var catalogue =  await grpcClient.GetCatalogueAsync(new InfoRequest
            {
                Info = "Ver Catalogo"
            });

            string catalogueView = "";
            catalogueView += "Catalogo de juegos: " + Environment.NewLine;
            catalogueView += "" + Environment.NewLine;

            foreach (Game game in catalogue.Games)
            {
                catalogueView += "Titulo: " + game.Title + Environment.NewLine;
                catalogueView += "Sinopsis: " + game.Synopsis + Environment.NewLine;
                catalogueView += "Categoría: " + game.Gender + Environment.NewLine;
                GameCalification calification = GameCalification.Sin_Calificaciones;
                switch (Math.Truncate(game.RatingAverage))
                {
                    case 1:
                        calification = GameCalification.Muy_Malo;
                        break;
                    case 2:
                        calification = GameCalification.Malo;
                        break;
                    case 3:
                        calification = GameCalification.Medio;
                        break;
                    case 4:
                        calification = GameCalification.Bueno;
                        break;
                    case 5:
                        calification = GameCalification.Muy_Bueno;
                        break;
                }
                catalogueView += "Calificacion media: " + calification.ToString() + Environment.NewLine;
                catalogueView += "" + Environment.NewLine;
            }
            return catalogueView;
        }
  

        public async Task<string> AddUser(string name) {
            var reply = await grpcClient.SendUserAsync(new User
            {
                UserName = name
            });
            return reply.ToString();
        }

        public async Task<string> DeleteGame(string message)
        {
            var reply = await grpcClient.DeleteGameAsync(new InfoRequest
            {
                Info = "Borrar juego"
            });
            return reply.Info;
        }

        public async Task<string> Login(string message)
        {
            var reply = await grpcClient.LoginAsync(new InfoRequest
            {
                Info = message
            });
            return reply.Info;
        }

        public async Task<string> ModifyGame(string message)
        {
            var reply = await grpcClient.ModifyGameAsync(new InfoRequest
            {
                Info = message
            });
            return reply.Info;
        }

        public async Task<string> SearchGameByTitle(string message)
        {

            var games = await grpcClient.GetGamesContainingAsync(new InfoRequest
            {
                Info = message
            });
            string response = "";
                if (games.Games_.Count != 0)
                {
                   // string serializedList = ProtocolDataProgram.SerializeGameList(games.Games_);
                   // response = "E" + Environment.NewLine + serializedList;
                }
                else
                {
                    response = "N" + Environment.NewLine + "El juego que busca no existe";
                }
                return response;
        }
        

        public async Task<string> SearchGameByGender(string message)
        {
            var games = await grpcClient.GetGamesByGenderAsync(new InfoRequest
            {
                Info = message
            });
             string response = "";
            if (games.Games_.Count != 0)
            {
                //string serializedList = ProtocolDataProgram.SerializeGameList(games);
                //response = "E" + Environment.NewLine + serializedList;
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }
        public async Task<string> SearchGameByRating(string calification)
        {
            Domain.Game aGame = new Domain.Game();
            GameCalification calif = ProtocolDataProgram.ParseGameCalification(calification);
            int calificationNumber = aGame.CalificationToInt(calif);
            var games = await grpcClient.GetGamesByCalificationAsync(new InfoRequest
            {
                Info = calificationNumber.ToString()
            });
            string response = "";
            if (games.Games_.Count != 0)
            {
                List<Domain.Game> result = new List<Domain.Game>();
                foreach (Game pGame in games.Games_)
                {
                    result.Add(ProtoDomainParsing.ParseProtoGame(pGame));
                }
                string serializedList = ProtocolDataProgram.SerializeGameList(result);
                response = "E" + Environment.NewLine + serializedList;
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }


        public string QualifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            string userName = info[0];
            string gameTitle = info[1];
            GameCalification calification = ProtocolDataProgram.ParseGameCalification(info[2]);
            string review = info[3];
            lock (this.gameCatalogue.Games)
            {
                Domain.Game gameToQualify = gameCatalogue.FindGame(gameTitle);
                string addRatingResponse = "";
                if (gameToQualify != null)
                {
                    Domain.User reviewer = users.Find(x => x.UserName.Equals(userName));
                    Domain.UserRating newRating = new Domain.UserRating(review, calification, reviewer);
                    gameToQualify.AddRating(newRating);
                    addRatingResponse = "Su review fue publicada con exito.";
                }
                else
                {
                    addRatingResponse = "El juego que desea calificar no existe." + Environment.NewLine +
                        "Asegurese de ingresar el nombre correctamente.";
                }
                return addRatingResponse;
            }
        }

        internal void SetGrpcClient(Provider.ProviderClient grpcClient)
        {
            this.grpcClient = grpcClient;
        }

        public async Task LogOut(string message)
        {
            var response = await grpcClient.LogoutAsync(new InfoRequest
            {
                Info = message
            });
        }

        public async Task<string> ShowUserGames(string message)
        {
            string response = "Lista de juegos:"+ Environment.NewLine;

            var user = await grpcClient.GetUserAsync(new InfoRequest
            {
                Info = message
            });
            //foreach (Game aGame in aUser.Games)
            //{
            //    response += "-------------------"+ Environment.NewLine;
            //    response += "Nombre: " +aGame.Title+ Environment.NewLine;
            //    response += "Genero: " + aGame.Gender + Environment.NewLine;
            //    response += "Calificacion: " + aGame.CalculateAverageCalification() + Environment.NewLine;
            //}

            return response;
        }
        public async Task <string> BuyGame(string message)
        {
            var response = await grpcClient.BuyGameAsync(new InfoRequest
            {
                Info = message
            });
            return response.Info;
        }
        public async Task<string> SendGameCoverAsync(string gameCover,TcpClient tcpClient)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + gameCover;
            if (File.Exists(path))
            {
                Console.WriteLine("Enviando imagen al cliente...");
                var fileCommunication = new FileCommunicationHandler(tcpClient);
                await fileCommunication.SendFileAsync(path);
                Console.WriteLine("Imagen enviada!");
            }
            return "La imagen solicitada fue recibida";
        }

        public void CargarDatosDePrueba()
        {
            lock (this.gameCatalogue.Games)
            {
                Domain.Game fifa = new Domain.Game("Fifa 21 Prueba", GameGender.Deporte, "Futbol actual", "fifa.jpg");
                fifa.AddRating(new Domain.UserRating("Muy buen juego", GameCalification.Bueno, new Domain.User("PacoPrueba")));
                fifa.AddRating(new Domain.UserRating("No es compatible con mi pc", GameCalification.Muy_Malo, new Domain.User("JuanPrueba")));
                gameCatalogue.AddGame(fifa);
                gameCatalogue.AddGame(new Domain.Game("Call of duty Prueba", GameGender.Accion, "Shooter", "COD.jpg"));
                gameCatalogue.AddGame(new Domain.Game("Mario Bros", GameGender.Aventura, "juego de nintendo", "mario.jpg"));
            }
        }
        public async Task ModifyUser(string name)
        {
            var reply = await grpcClient.GetUserAsync(new InfoRequest
            {
                Info = name
            });

            if (reply != null)
            {
                Console.WriteLine("Ingrese el nuevo nombre de usuario");
                string newName = Console.ReadLine();
                var replyModify = await grpcClient.ModifyUserAsync(new InfoRequest
                {
                    Info = newName
                });
                Console.WriteLine(replyModify);
            }
            else
            {
                Console.WriteLine("El usuario que intenta modificar no existe.");
            }
        }
        public async Task DeleteUser(string name)
        {
            var reply = await grpcClient.DeleteUserAsync(new InfoRequest
            {
                Info = name
            });
            Console.WriteLine(reply.Info);
        }
        public async Task ShowUsers()
        {
            var reply = await grpcClient.GetUsersAsync(new InfoRequest
            {
                Info = "Ver Usuarios"
            });

            Console.WriteLine("Usuarios registrados en el sistema:");
            Console.WriteLine("");
            foreach (User user in reply.Users_)
            {
                Console.WriteLine(user.UserName);
            }
            Console.WriteLine("-----------------------------------");
        }

    }
}


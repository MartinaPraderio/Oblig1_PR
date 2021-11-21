using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Server
{
    public class ServerServicesManager
    {
        private readonly static ServerServicesManager _instance = new ServerServicesManager();
        private Catalogue gameCatalogue = Catalogue.Instance;
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

        public async Task<string> PublishGameAsync(Domain.Game newGame, TcpClient tcpClient) {
            this.fileCommunication = new FileCommunicationHandler(tcpClient);
            await fileCommunication.ReceiveFileAsync();
            lock (this.gameCatalogue.Games)
            {
                this.gameCatalogue.AddGame(newGame);
            }
            return "Juego agregado con exito";
        }

        public string GameDetails(string message) {
            string response = "";
            lock (this.gameCatalogue.Games)
            {
                Domain.Game game = gameCatalogue.ga.Find(x => x.Title.Equals(message));


                if (game != null)
                {
                    response = "E" + Environment.NewLine + ProtocolDataProgram.SerializeGame(game);
                }
                else
                {
                    response = "N" + Environment.NewLine + "El juego que busca no existe";
                }
            }
            return response;
        }

        public string ViewCatalogue(){
            lock (this.gameCatalogue.Games)
            {
                return gameCatalogue.DisplayGames();
            }
        }
  

        public async Task<string> AddUser(string name) {
            var reply = await grpcClient.SendUserAsync(new User
            {
                UserName = name
            });
            Console.WriteLine(reply.UserName);

            //lock (this.users)
            //{
            //    this.users.Add(newUser);
            //}
            return reply.ToString();
        }

        public string DeleteGame(string message)
        {
            var reply = await grpcClient.DeleteGameAsync(new InfoRequest
            {
                Info = message
            });
            Console.WriteLine(reply.info);
            

            
        }

        public string Login(string message)
        {
            lock (this.users)
            {
                Domain.User user = this.users.Find(x => x.UserName.Equals(message));

                string response = "";
                if (user != null)
                {
                    lock (this.loggedUsers)
                    {
                        if (this.loggedUsers.Find(x => x.UserName.Equals(message)) != null)
                        {
                            response = "Ya existe una sesion con este usuario ";
                        }
                        else
                        {
                            response = "Login exitoso";
                            this.loggedUsers.Add(user);
                        }
                    }
                }
                else
                {
                    response = "El usuario que ingresó no existe";
                }
                return response;
            }
        }

        public string ModifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            
            lock (this.gameCatalogue.Games)
            {
                Domain.Game aGame = gameCatalogue.FindGame(info[0]);
                string response = "";
                if (aGame != null)
                {
                    Domain.Game newGameValues = ProtocolDataProgram.DeserializeGame(info[1]);

                    aGame.Title = newGameValues.Title;
                    aGame.Gender = newGameValues.Gender;
                    aGame.Synopsis = newGameValues.Synopsis;
                    response = "El juego fue modificado.";
                }
                else
                {
                    response = "El juego que quiere modificar no existe";
                }
                return response;
            }
        }

        public string SearchGameByTitle(string message)
        {
            lock (this.gameCatalogue.Games)
            {
                List<Domain.Game> games = gameCatalogue.FindAllGamesContaining(message);
                string response = "";
                if (games.Count != 0)
                {
                    string serializedList = ProtocolDataProgram.SerializeGameList(games);
                    response = "E" + Environment.NewLine + serializedList;
                }
                else
                {
                    response = "N" + Environment.NewLine + "El juego que busca no existe";
                }
                return response;
            }
        }

        public string SearchGameByGender(string message)
        {
            lock (this.gameCatalogue.Games)
            {
                List<Domain.Game> games = gameCatalogue.FindAllGamesByGender(message);
                string response = "";
                if (games.Count != 0)
                {
                    string serializedList = ProtocolDataProgram.SerializeGameList(games);
                    response = "E" + Environment.NewLine + serializedList;
                }
                else
                {
                    response = "N" + Environment.NewLine + "El juego que busca no existe";
                }
                return response;
            }
        }

        public string SearchGameByRating(string calification)
        {
            GameCalification calif = ProtocolDataProgram.ParseGameCalification(calification);
            lock (this.gameCatalogue.Games)
            {
                List<Domain.Game> games = gameCatalogue.FindAllGames(calif);
                string response = "";
                if (games.Count != 0)
                {
                    string serializedList = ProtocolDataProgram.SerializeGameList(games);
                    response = "E" + Environment.NewLine + serializedList;
                }
                else
                {
                    response = "N" + Environment.NewLine + "El juego que busca no existe";
                }
                return response;
            }
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

        public void LogOut(string message)
        {
            lock (this.loggedUsers)
            {
                Domain.User user = this.loggedUsers.Find(x => x.UserName.Equals(message));
                if(user != null)
                {
                    loggedUsers.Remove(user);
                }
            }
        }

        public string ShowUserGames(string message)
        {
            string response = "Lista de juegos:"+ Environment.NewLine;
            lock (this.users)
            {
                Domain.User aUser = this.users.Find(x => x.UserName.Equals(message));
                foreach (Domain.Game aGame in aUser.Games)
                {
                    response += "-------------------"+ Environment.NewLine;
                    response += "Nombre: " +aGame.Title+ Environment.NewLine;
                    response += "Genero: " + aGame.Gender + Environment.NewLine;
                    response += "Calificacion: " + aGame.CalculateAverageCalification() + Environment.NewLine;
                }
            }
            return response;
        }

        public string BuyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            string buyerName = info[0];
            string gameName = info[1];
            lock (this.gameCatalogue.Games)
            {
                string response = "";
                

                Domain.Game requestedGame = gameCatalogue.FindGame(gameName);
                if (requestedGame != null)
                {
                    lock (this.users)
                    {
                        Domain.User buyer = this.users.Find(x => x.UserName.Equals(buyerName));
                        buyer.AddGame(requestedGame);
                    }
                    response = "Su compra ha finalizado con exito";
                }
                else
                {
                    response = "El juego que desea adquirir no existe." + Environment.NewLine +
                        "Asegurese de ingresar el nombre correctamente.";
                }
                return response;
            }
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
                var reply =  await grpcClient.GetUserAsync(new InfoRequest
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
        }

        public void DeleteUser(string name)
        {
            lock (this.users)
            {
                Domain.User aUser = this.users.Find(x => x.UserName.Equals(name));
                lock (this.loggedUsers)
                {
                    Domain.User userLogged = this.loggedUsers.Find(x => x.UserName.Equals(name));
                    if (aUser != null)
                    {
                        if (userLogged != null)
                        {
                            Console.WriteLine("No es posible eliminar este usuario ya que se encuentra loggeado");
                        }
                        else
                        {
                            this.users.Remove(aUser);
                            Console.WriteLine("El usuario fue eliminado con exito.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("El usuario que intenta eliminar no existe.");
                    }
                }
            }
        }

        public async Task ShowUsers()
        {
            
                Console.WriteLine("Usuarios registrados en el sistema:");
                Console.WriteLine("");
                var reply = await this.grpcClient.GetUsers();
                foreach (User user in reply)
                {
                    Console.WriteLine(user.UserName);
                }
                Console.WriteLine("-----------------------------------");
    
        }
    }
}

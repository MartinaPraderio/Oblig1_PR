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
        private List<User> users = new List<User>();
        private TcpListener _tcpListener;
        FileCommunicationHandler fileCommunication;

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
                Game game = gameCatalogue.FindGame(message);
            
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
  

        public string AddUser(User newUser) {
            lock (this.users)
            {
                this.users.Add(newUser);
            }
            return "Usuario agragado con exito";
        }

        public string DeleteGame(string message)
        {
            lock (this.gameCatalogue.Games)
            {
                Game aGame = gameCatalogue.FindGame(message);
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
                return response;
            }
        }

        public string Login(string message)
        {
            string response = "";
            if(this.users.Find(x => x.UserName.Equals(message)) != null)
            {
                response = "Login exitoso";
            }
            else
            {
                response = "El usuario que ingresó no existe";
            }
            return response;
        }

        public string ModifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            
            lock (this.gameCatalogue.Games)
            {
                Game aGame = gameCatalogue.FindGame(info[0]);
                string response = "";
                if (aGame != null)
                {
                    Game newGameValues = ProtocolDataProgram.DeserializeGame(info[1]);

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
                List<Game> games = gameCatalogue.FindAllGamesContaining(message);
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
                List<Game> games = gameCatalogue.FindAllGamesByGender(message);
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
                List<Game> games = gameCatalogue.FindAllGames(calif);
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
                Game gameToQualify = gameCatalogue.FindGame(gameTitle);
                string addRatingResponse = "";
                if (gameToQualify != null)
                {
                    User reviewer = this.users.Find(x => x.UserName.Equals(userName));
                    UserRating newRating = new UserRating(review, calification, reviewer);
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

        public string ShowUserGames(string message)
        {
            string response = "Lista de juegos:"+ Environment.NewLine;
            lock (this.users)
            {
                User aUser = this.users.Find(x => x.UserName.Equals(message));
                foreach (Game aGame in aUser.Games)
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
                

                Game requestedGame = gameCatalogue.FindGame(gameName);
                if (requestedGame != null)
                {
                    lock (this.users)
                    {
                        User buyer = this.users.Find(x => x.UserName.Equals(buyerName));
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
                Game fifa = new Game("Fifa 21 Prueba", GameGender.Deporte, "Futbol actual", "fifa.jpg");
                fifa.AddRating(new UserRating("Muy buen juego", GameCalification.Bueno, new User("PacoPrueba")));
                fifa.AddRating(new UserRating("No es compatible con mi pc", GameCalification.Muy_Malo, new User("JuanPrueba")));
                gameCatalogue.AddGame(fifa);
                gameCatalogue.AddGame(new Game("Call of duty Prueba", GameGender.Accion, "Shooter", "COD.jpg"));
                gameCatalogue.AddGame(new Game("Mario Bros", GameGender.Aventura, "juego de nintendo", "mario.jpg"));
            }
        }

        public void ModifyUser(string name)
        {
            User aUser = this.users.Find(x => x.UserName.Equals(name));
            if (aUser!= null)
            {
                Console.WriteLine("Ingrese el nuevo nombre de usuario");
                string newName = Console.ReadLine();
                aUser.UserName = newName;
                Console.WriteLine("El usuario fue modificado con exito.");
            }
            else
            {
                Console.WriteLine("El usuario que intenta modificar no existe.");
            }
        }

        public void DeleteUser(string name)
        {
            User aUser = this.users.Find(x => x.UserName.Equals(name));
            if (aUser != null)
            {
                this.users.Remove(aUser);
                Console.WriteLine("El usuario fue eliminado con exito.");
            }
            else
            {
                Console.WriteLine("El usuario que intenta eliminar no existe.");
            }
        }

        public void ShowUsers()
        {
            Console.WriteLine("Usuarios registrados en el sistema:");
            Console.WriteLine("");
            foreach (User user in this.users)
            {
                Console.WriteLine(user.UserName);
            }
            Console.WriteLine("-----------------------------------");
        }
    }
}

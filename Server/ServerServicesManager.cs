using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class ServerServicesManager
    {
        private readonly static ServerServicesManager _instance = new ServerServicesManager();
        private Catalogue gameCatalogue = Catalogue.Instance;
        private List<User> users = new List<User>();
        private User loggedUser;
        private Socket serverSocket;
        FileCommunicationHandler fileCommunication;

        public static ServerServicesManager Instance()
        {
            return _instance;
        }

        public void SetSocket(Socket socket)
        {
            serverSocket = socket;
        }

        public string PublishGame(Game newGame, Socket clientSocket) {
                this.fileCommunication = new FileCommunicationHandler(clientSocket);
                fileCommunication.ReceiveFile();
                this.gameCatalogue.AddGame(newGame);
                return "Juego agregado con exito";
        }
        public string GameDetails(string message) {
            Game game = gameCatalogue.FindGame(message);
            string response = "";
            if (game != null)
            {
                response = "E" + Environment.NewLine + ProtocolDataProgram.SerializeGame(game);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        public string ViewCatalogue(){
            return gameCatalogue.DisplayGames();
        }
  

        public string AddUser(User newUser) {
            this.users.Add(newUser);
            this.loggedUser = newUser;
            return "Usuario agragado con exito";
        }

        public string DeleteGame(string message)
        {
            Game aGame = gameCatalogue.FindGame(message);
            lock (this.gameCatalogue.Games)
            {
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

        public string ModifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            Game aGame = gameCatalogue.FindGame(info[0]);
            lock (aGame)
            {
                
                string response = "";
                if (aGame != null)
                {
                    Console.WriteLine("titulo viejo: " + aGame.Title);
                    Game newGameValues = ProtocolDataProgram.DeserializeGame(info[1]);

                    aGame.Title = newGameValues.Title;
                    aGame.Gender = newGameValues.Gender;
                    aGame.Synopsis = newGameValues.Synopsis;
                    response = "El juego fue modificado.";

                    foreach (Game g in gameCatalogue.Games)
                    {
                        Console.WriteLine(g.Title);
                    }
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
            
            List<Game> games = gameCatalogue.FindAllGamesContaining(message);
            string response = "";
            if (games.Count != 0)
            {
                string serializedList = ProtocolDataProgram.SerializeGameList(games);
                response = "E"+Environment.NewLine+serializedList;
            }
            else
            {
                response = "N"+Environment.NewLine+"El juego que busca no existe";
            }
            return response;
        }

        public string SearchGameByGender(string message)
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

        public string SearchGameByRating(string calification)
        {
            GameCalification calif = ProtocolDataProgram.ParseGameCalification(calification);
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

        public string QualifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            string gameTitle = info[0];
            GameCalification calification = ProtocolDataProgram.ParseGameCalification(info[1]);
            string review = info[2];
            
            Game gameToQualify = gameCatalogue.FindGame(gameTitle);
            string addRatingResponse = "";
            if (gameToQualify != null)
            {
                UserRating newRating = new UserRating(review, calification, loggedUser);
                gameToQualify.AddRating(newRating);
                addRatingResponse = "Su review fue publicada con exito.";
            }
            else
            {
                addRatingResponse = "El juego que desea calificar no existe."+Environment.NewLine+
                    "Asegurese de ingresar el nombre correctamente.";
            }
            return addRatingResponse;
        }

        public string BuyGame(string message)
        {
            string response = "";
            Game requestedGame = gameCatalogue.FindGame(message);
            if(requestedGame != null)
            {
                loggedUser.AddGame(requestedGame);
                response = "Su compra ha finalizado con exito";
            }
            else
            {
                response = "El juego que desea adquirir no existe." + Environment.NewLine +
                    "Asegurese de ingresar el nombre correctamente.";
            }
            return response;
        }

        public string SendGameCover(string gameCover,Socket clientSocket)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + gameCover;
            if (File.Exists(path))
            {
                Console.WriteLine("Enviando imagen al cliente...");
                var fileCommunication = new FileCommunicationHandler(clientSocket);
                fileCommunication.SendFile(path);
                Console.WriteLine("Imagen enviada!");
            }
            return "La imagen solicitada fue recibida";
        }

        internal void CargarDatosDePrueba()
        {
            Game fifa = new Game("Fifa 21 Prueba", GameGender.Deporte, "Futbol actual", "fifa.jpg");
            fifa.AddRating(new UserRating("Muy buen juego", GameCalification.Bueno, new User("PacoPrueba")));
            fifa.AddRating(new UserRating("No es compatible con mi pc", GameCalification.Muy_Malo, new User("JuanPrueba")));
            gameCatalogue.AddGame(fifa);
            gameCatalogue.AddGame(new Game("Call of duty Prueba", GameGender.Accion, "Shooter", "COD.jpg"));
            gameCatalogue.AddGame(new Game("Mario Bros", GameGender.Aventura, "juego de nintendo", "mario.jpg"));

        }
    }
}

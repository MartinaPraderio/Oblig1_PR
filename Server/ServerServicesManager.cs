using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;
using System.IO;

namespace BusinessLogic
{
    public class ServerServicesManager
    {
        private Catalogue gameCatalogue;
        private List<User> users;
        private Socket serverSocket;
        FileCommunicationHandler fileCommunication;
        
        public ServerServicesManager(Socket serverSocket)
        {
            this.gameCatalogue = Catalogue.Instance;
            this.users = new List<User>();
            this.serverSocket = serverSocket;
        }

        public string PublishGame(Game newGame, Socket clientSocket) {
            this.fileCommunication = new FileCommunicationHandler(clientSocket);
            fileCommunication.ReceiveFile();
            this.gameCatalogue.AddGame(newGame);
            return "Juego agregado con exito";
        }
        public string GameDetails(string message) {
            Game game = gameCatalogue.Games.Find(x => x.Title.Equals(message));
            string response = "";
            if (game != null)
            {
                response = "E" + Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(game);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        public void AcceptClient(){
        }
        public void ViewCatalogue(){
            gameCatalogue.DisplayGames();
        }
  

        public string AddUser(User newUser) {
            Console.WriteLine("cant usuarios "+users.Count);
            this.users.Add(newUser);
            Console.WriteLine(users.Count);
            return "Usuario agragado con exito";
        }

        internal string DeleteGame(string message)
        {
            Game aGame = gameCatalogue.Games.Find(x => x.Title.Equals(message));
            string response = "";
            if (aGame!=null)
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

        internal string ModifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            Game aGame = gameCatalogue.Games.Find(x => x.Title.Equals(info[0]));
            string response = "";
            if (aGame != null)
            {
                Console.WriteLine("titulo viejo: " + aGame.Title);
                Game newGameValues = JsonConvert.DeserializeObject<Game>(info[1]);
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

        internal string SearchGameByTitle(string message)
        {
            
            List<Game> games = gameCatalogue.Games.FindAll(x => x.Title.Contains(message));
            string response = "";
            if (games.Count != 0)
            {
                response = "E"+Environment.NewLine+System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N"+Environment.NewLine+"El juego que busca no existe";
            }
            return response;
        }

        internal string SearchGameByGender(string message)
        {
            List<Game> games = gameCatalogue.Games.FindAll(x => x.Gender.ToString().Equals(message));
            string response = "";
            if (games.Count != 0)
            {
                response = "E" + Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        internal string SearchGameByRating(string calification)
        {
            GameCalification calif = (GameCalification)Enum.Parse(typeof(GameCalification),calification);
            List<Game> games = gameCatalogue.Games.FindAll(x => Math.Truncate(x.RatingAverage).Equals(x.CalificationToInt(calif)));
            string response = "";
            if (games.Count != 0)
            {
                response = "E" + Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(games);
            }
            else
            {
                response = "N" + Environment.NewLine + "El juego que busca no existe";
            }
            return response;
        }

        internal string QualifyGame(string message)
        {
            string[] info = message.Split(Environment.NewLine);
            string gameTitle = info[0];
            GameCalification calification = (GameCalification)Enum.Parse(typeof(GameCalification), info[1]);
            string review = info[2];
            User actualUser = this.users.Find(x => x.UserName.Equals(info[3])); // esto despues hay q sacarlo, deberia ser parte del protocolo.
            Game gameToQualify = gameCatalogue.Games.Find(x => x.Title.Equals(gameTitle));
            UserRating newRating = new UserRating(review,calification,actualUser);
            gameToQualify.AddRating(newRating);
            string addRatingResponse = "Su review fue publicada con exito.";
            return addRatingResponse;
        }

        public string SendGameCover(string gameCover,Socket clientSocket)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + gameCover;
         //Console.WriteLine("Path de cover: "+path);
            if (File.Exists(path))
            {
                Console.WriteLine("Enviando imagen al cliente...");
                var fileCommunication = new FileCommunicationHandler(clientSocket);
                fileCommunication.SendFile(path);
                Console.WriteLine("Imagen enviada!");
            }
            return "La imagen solicitada fue recibida";
        }

        //internal void PublishGame()
        //{
        //    Console.WriteLine("ingrese el titulo del juego");
        //    string title = Console.ReadLine();
        //    Console.WriteLine("ingrese el synopsis del juego");
        //    string synopsis = Console.ReadLine();
        //    Console.WriteLine("ingrese el genero del juego");
        //    Console.WriteLine("Accion (Ingrese 1)");
        //    Console.WriteLine("Aventura (Ingrese 2)");
        //    Console.WriteLine("Estrategia (Ingrese 3)");
        //    Console.WriteLine("Infantil (Ingrese 4)");
        //    string gender = Console.ReadLine();
        //    GameGender genderGame = GameGender.PorDefecto;
        //    switch (gender)
        //    {
        //        case "1":
        //            genderGame = GameGender.Accion;
        //            break;
        //        case "2":
        //            genderGame = GameGender.Aventura;
        //            break;
        //        case "3":
        //            genderGame = GameGender.Estrategia;
        //            break;
        //        case "4":
        //            genderGame = GameGender.Infantil;
        //            break;
        //    }
        //    Game game = new Game(title, genderGame, synopsis, "cover");
        //    this.gameCatalogue.AddGame(game);
        //    Console.WriteLine("Juego publicado!");
        //}
    }
}

using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.IO;

namespace Client
{
    public class ClientServicesManager
    {
        private readonly static ClientServicesManager _instance = new ClientServicesManager();
        private ProtocolDataProgram protocolHandleData = new ProtocolDataProgram();

        private Socket clientSocket;

        private string userName;
        private FileCommunicationHandler fileCommunication;

        public static ClientServicesManager Instance()
        {
            return _instance;
        }

        public void SetSocket(Socket socket)
        {
            clientSocket = socket;
        }


        public void SetUserName(string user)
        {
            userName = user;
        }
        public void SendMessage(string message, action action)
        {
            ProtocolDataProgram.Send(clientSocket, action.ToString());
            ProtocolDataProgram.Send(clientSocket, message);
        }

        public void PublishGame()
        {
            Console.WriteLine("ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("ingrese el genero del juego");
            Console.WriteLine("Accion (Ingrese 1)");
            Console.WriteLine("Aventura (Ingrese 2)");
            Console.WriteLine("Estrategia (Ingrese 3)");
            Console.WriteLine("Infantil (Ingrese 4)");
            string gender = Console.ReadLine();
            GameGender genderGame = GameGender.PorDefecto;
            switch (gender)
            {
                case "1":
                    genderGame = GameGender.Accion;
                    break;
                case "2":
                    genderGame = GameGender.Aventura;
                    break;
                case "3":
                    genderGame = GameGender.Estrategia;
                    break;
                case "4":
                    genderGame = GameGender.Infantil;
                    break;
            }

            Console.WriteLine("Ingrese el path de la caratula del juego");
            string path = Console.ReadLine();
            var fileInfo = new FileInfo(path);
            string fileName = fileInfo.Name;

            Game game = new Game(title, genderGame, synopsis, fileName);

            string message = ProtocolDataProgram.SerializeGame(game);
            SendMessage(message, action.PublishGame);

            if (File.Exists(path))
            {
                Console.WriteLine("Mandando imagen al servidor");
                var fileCommunication = new FileCommunicationHandler(clientSocket);
                fileCommunication.SendFile(path);
                Console.WriteLine("Se termino de mandar la imagen al servidor");
            }

            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        public void ModifyGame()
        {
            Console.WriteLine("ingrese el titulo del juego a modificar");
            string title = Console.ReadLine();
            Console.WriteLine("ingrese el nuevo titulo del juego");
            string newTitle = Console.ReadLine();
            Console.WriteLine("ingrese la nueva synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("ingrese el nuevo genero del juego");
            Console.WriteLine("Accion (Ingrese 1)");
            Console.WriteLine("Aventura (Ingrese 2)");
            Console.WriteLine("Estrategia (Ingrese 3)");
            Console.WriteLine("Infantil (Ingrese 4)");
            string gender = Console.ReadLine();
            GameGender genderGame = GameGender.PorDefecto;
            switch (gender)
            {
                case "1":
                    genderGame = GameGender.Accion;
                    break;
                case "2":
                    genderGame = GameGender.Aventura;
                    break;
                case "3":
                    genderGame = GameGender.Estrategia;
                    break;
                case "4":
                    genderGame = GameGender.Infantil;
                    break;
            }
            Game game = new Game(newTitle, genderGame, synopsis, "cover");
            string message = title +Environment.NewLine+ ProtocolDataProgram.SerializeGame(game);
            SendMessage(message, action.ModifyGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }
        public void DeleteGame()
        {
            Console.WriteLine("ingrese el titulo del juego a eliminar");
            string title = Console.ReadLine();
            SendMessage(title, action.DeleteGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        public void BuyGame()
        {
            SendMessage("dummy", action.ViewCatalogue);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("Ingrese el titulo del juego que desea comprar");
            string title = Console.ReadLine();
            SendMessage(title, action.BuyGame);
            response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        public void QualifyGame()
        {
            Console.WriteLine("Ingrese el titulo del juego a calificar: ");
            string gameTitleToQualify = Console.ReadLine();

            Console.WriteLine("Ingrese calificacion: ");
            Console.WriteLine("Muy Malo: (ingrese 1)");
            Console.WriteLine("Malo: (ingrese 2)");
            Console.WriteLine("Medio: (ingrese 3)");
            Console.WriteLine("Bueno: (ingrese 4)");
            Console.WriteLine("Muy Bueno: (ingrese 5)");
            string searchQuery = Console.ReadLine();
            GameCalification calification = GameCalification.PorDefecto;
            switch (searchQuery)
            {
                case "1":
                    calification = GameCalification.Muy_Malo;
                    break;
                case "2":
                    calification = GameCalification.Malo;
                    break;
                case "3":
                    calification = GameCalification.Medio;
                    break;
                case "4":
                    calification = GameCalification.Bueno;
                    break;
                case "5":
                    calification = GameCalification.Muy_Bueno;
                    break;
            }
            Console.WriteLine("Ingrese comentario: ");
            string review = Console.ReadLine();

            string message = gameTitleToQualify + Environment.NewLine + calification.ToString() + Environment.NewLine + review;
            SendMessage(message, action.QualifyGame);

            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        public void GameDetails()
        {
            Console.WriteLine("Ingrese el titulo del juego para ver los detalles");
            string title = Console.ReadLine();
            SendMessage(title, action.GameDetails);
            string response = ProtocolDataProgram.Listen(clientSocket);
            string[] info = response.Split(Environment.NewLine);
            if (info[0].Equals("N"))
            {
                Console.WriteLine(info[1]);
            }
            else
            {
                Game aGame = ProtocolDataProgram.DeserializeGame(info[1]);
                Console.WriteLine("Los detalles del juego buscado son:");
                Console.WriteLine("Titulo: " + aGame.Title);
                Console.WriteLine("Sinopsis: " + aGame.Synopsis);
                Console.WriteLine("Categoría: " + aGame.Gender);
                GameCalification calification = GameCalification.PorDefecto;
                switch (Math.Truncate(aGame.RatingAverage))
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
                Console.WriteLine("Calificacion media: " + calification.ToString());
                Console.WriteLine("===> Desea descargar la caratula?");
                Console.WriteLine("Si (Digite 1)");
                Console.WriteLine("No (Digite 2)");
                string option = Console.ReadLine();
                if (option.Equals("1"))
                {
                    SendMessage(aGame.Cover, action.GameCover);
                    this.fileCommunication = new FileCommunicationHandler(clientSocket);
                    fileCommunication.ReceiveFile();
                    string responseCover = ProtocolDataProgram.Listen(clientSocket);
                    Console.WriteLine(responseCover);
                }
                Console.WriteLine("---------------------------");
            }
        }

        public void SearchGame()
        {
            Console.WriteLine("Busqueda por: ");
            Console.WriteLine("Titulo: (ingrese 1)");
            Console.WriteLine("Caegoria: (ingrese 2)");
            Console.WriteLine("Rating: (ingrese 3)");
            string searchBy = Console.ReadLine();
            action searchAction = action.Default;
            string searchQuery = "";
            switch (searchBy)
            {
                case "1":
                    Console.WriteLine("Ingrese el titulo del juego buscado");
                    searchAction = action.SearchGameByTitle;
                    searchQuery = Console.ReadLine();
                    SendMessage(searchQuery, searchAction);
                    break;
                case "2":
                    Console.WriteLine("Ingrese el genero del juego buscado");
                    Console.WriteLine("Accion (Ingrese 1)");
                    Console.WriteLine("Aventura (Ingrese 2)");
                    Console.WriteLine("Estrategia (Ingrese 3)");
                    Console.WriteLine("Infantil (Ingrese 4)");
                    string gender = Console.ReadLine();
                    GameGender genderGame = GameGender.PorDefecto;
                    switch (gender)
                    {
                        case "1":
                            genderGame = GameGender.Accion;
                            break;
                        case "2":
                            genderGame = GameGender.Aventura;
                            break;
                        case "3":
                            genderGame = GameGender.Estrategia;
                            break;
                        case "4":
                            genderGame = GameGender.Infantil;
                            break;
                    }
                    searchAction = action.SearchGameByGender;
                    SendMessage(genderGame.ToString(), searchAction);
                    break;
                case "3":
                    Console.WriteLine("Ingrese el raiting del juego buscado");
                    Console.WriteLine("Muy Malo (Ingrese 1)");
                    Console.WriteLine("Malo (Ingrese 2)");
                    Console.WriteLine("Medio (Ingrese 3)");
                    Console.WriteLine("Bueno (Ingrese 4)");
                    Console.WriteLine("Muy Bueno (Ingrese 5)");
                    searchQuery = Console.ReadLine();
                    GameCalification calification = GameCalification.PorDefecto;
                    switch (searchQuery)
                    {
                        case "1":
                            calification = GameCalification.Muy_Malo;
                            break;
                        case "2":
                            calification = GameCalification.Malo;
                            break;
                        case "3":
                            calification = GameCalification.Medio;
                            break;
                        case "4":
                            calification = GameCalification.Bueno;
                            break;
                        case "5":
                            calification = GameCalification.Muy_Bueno;
                            break;
                    }
                    searchAction = action.SearchGameByRating;
                    SendMessage(calification.ToString(), searchAction);
                    break;
            }
            string response = ProtocolDataProgram.Listen(clientSocket);
            string[] info = response.Split(Environment.NewLine);
            if (info[0].Equals("N"))
            {
                Console.WriteLine(info[1]);
            }
            else
            {

                List<Game> games = ProtocolDataProgram.DeserializeGameList(info[1]);
                Console.WriteLine("Los siguientes juegos cumplen con su busqueda");
                int count = 1;
                foreach (Game game in games)
                {
                    Console.WriteLine("Juego " + count + ":");
                    Console.WriteLine("Titulo: " + game.Title);
                    Console.WriteLine("Sinopsis: " + game.Synopsis);
                    Console.WriteLine("Categoría: " + game.Gender);
                    Console.WriteLine("---------------------------");
                    count++;
                }
            }

        }
    }
}
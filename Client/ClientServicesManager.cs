using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

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
        public bool SendMessage(string message, action action)
        {
            bool ConnectionState = clientSocket.Connected;
            if (clientSocket.Poll(5000, SelectMode.SelectRead) && (clientSocket.Available == 0))
            {
                ConnectionState = false;
            }

            if (ConnectionState)
            {
                ProtocolDataProgram.Send(clientSocket, action.ToString());
                ProtocolDataProgram.Send(clientSocket, message);
            }
            return ConnectionState;
        }


        public bool PublishGame()
        {
            Console.WriteLine("-----------PUBLICAR JUEGO------------");
            Console.WriteLine("Ingrese el titulo del juego");
            string title = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Ingrese el synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Ingrese el genero del juego");
            Console.WriteLine("==> Accion       (Ingrese 1)");
            Console.WriteLine("==> Aventura     (Ingrese 2)");
            Console.WriteLine("==> Estrategia   (Ingrese 3)");
            Console.WriteLine("==> Infantil     (Ingrese 4)");
            Console.WriteLine("==> Deporte      (Ingrese 5)");
            Console.WriteLine("==> Otros        (Ingrese 6)");
            string gender = Console.ReadLine();
            Console.WriteLine("");
            GameGender genderGame = GameGender.Otros;
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
                case "5":
                    genderGame = GameGender.Deporte;
                    break;
                case "6":
                    genderGame = GameGender.Otros;
                    break;
            }

            Console.WriteLine("Ingrese el path de la caratula del juego");
            string path = Console.ReadLine();
            Console.WriteLine("");
            var fileInfo = new FileInfo(path);
            string fileName = fileInfo.Name;

            Game game = new Game(title, genderGame, synopsis, fileName);

            string message = ProtocolDataProgram.SerializeGame(game);
            

            bool success = SendMessage(message, action.PublishGame); ;
            if (!success) { return false; }

            if (File.Exists(path))
            {
                Console.WriteLine("Mandando imagen al servidor...");
                Console.WriteLine("");
                var fileCommunication = new FileCommunicationHandler(clientSocket);
                fileCommunication.SendFile(path);
                Console.WriteLine("Se termino de mandar la imagen al servidor");
            }
            Console.WriteLine("");
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("-------------------------------------");
            return true;
        }

        public bool ModifyGame()
        {
            Console.WriteLine("-----------MODIFICAR JUEGO-----------");
            Console.WriteLine("Ingrese el titulo del juego a modificar");
            string title = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Ingrese el nuevo titulo del juego");
            string newTitle = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("ingrese la nueva synopsis del juego");
            string synopsis = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Ingrese el nuevo genero del juego");
            Console.WriteLine("==> Accion       (Ingrese 1)");
            Console.WriteLine("==> Aventura     (Ingrese 2)");
            Console.WriteLine("==> Estrategia   (Ingrese 3)");
            Console.WriteLine("==> Infantil     (Ingrese 4)");
            Console.WriteLine("==> Deporte      (Ingrese 5)");
            Console.WriteLine("==> Otros        (Ingrese 6)");
            string gender = Console.ReadLine();
            GameGender genderGame = GameGender.Otros;
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
                case "5":
                    genderGame = GameGender.Deporte;
                    break;
                case "6":
                    genderGame = GameGender.Otros;
                    break;
            }
            Game game = new Game(newTitle, genderGame, synopsis, "cover");
            string message = title +Environment.NewLine+ ProtocolDataProgram.SerializeGame(game);
            bool success = SendMessage(message, action.ModifyGame); ;
            if (!success) { return false; }
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("-------------------------------------");
            return true;
        }
        public bool DeleteGame()
        {
            Console.WriteLine("------------ELIMINAR JUEGO------------");
            Console.WriteLine("Ingrese el titulo del juego a eliminar");
            string title = Console.ReadLine();
            bool success = SendMessage(title, action.DeleteGame); ;
            if (!success) { return false; }
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("-------------------------------------");
            return true;
        }

        public bool BuyGame()
        {
            Console.WriteLine("-----------COMPRAR UN JUEGO-----------");
            
            bool success = SendMessage("Comprar", action.ViewCatalogue);
            if (!success) { return false; }
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine(" ");
            Console.WriteLine("Ingrese el titulo del juego que desea comprar");
            string title = Console.ReadLine();
            Console.WriteLine("");
            success = SendMessage(title, action.BuyGame);
            if (!success) { return false; }
            response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("-------------------------------------");
            return true;
        }

        public bool QualifyGame()
        {
            Console.WriteLine("---------CALIFICAR UN JUEGO----------");
            Console.WriteLine("Ingrese el titulo del juego a calificar: ");
            string gameTitleToQualify = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Ingrese su calificacion: ");
            Console.WriteLine("==> Muy Malo:    (Ingrese 1)");
            Console.WriteLine("==> Malo:        (Ingrese 2)");
            Console.WriteLine("==> Medio:       (Ingrese 3)");
            Console.WriteLine("==> Bueno:       (Ingrese 4)");
            Console.WriteLine("==> Muy Bueno:   (Ingrese 5)");
            string searchQuery = Console.ReadLine();
            Console.WriteLine("");
            GameCalification calification = GameCalification.Sin_Calificaciones;
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
            Console.WriteLine("Ingrese un comentario: ");
            string review = Console.ReadLine();
            Console.WriteLine("");
            string message = gameTitleToQualify + Environment.NewLine + calification.ToString() + Environment.NewLine + review;
            bool success = SendMessage(message, action.QualifyGame);
            if (!success) { return false; }
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
            Console.WriteLine("-------------------------------------");
            return true;
        }

        public bool GameDetails()
        {
            Console.WriteLine("-------DETALLES DE UN JUEGO----------");
            Console.WriteLine("Ingrese el titulo del juego para ver los detalles");
            string title = Console.ReadLine();
            Console.WriteLine("");
            bool success = SendMessage(title, action.GameDetails);
            if (!success) { return false; }
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
                Console.WriteLine("Titulo:          " + aGame.Title);
                Console.WriteLine("Sinopsis:        " + aGame.Synopsis);
                Console.WriteLine("Categoría:       " + aGame.Gender);
                GameCalification calification = GameCalification.Sin_Calificaciones;
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
                Console.WriteLine("");
                Console.WriteLine("Listado de calificaciones: ");
                foreach (UserRating rating in aGame.UserRatings)
                {
                    Console.WriteLine("");
                    Console.WriteLine("=> Usuario:       "+ rating.User.UserName);
                    Console.WriteLine("=> Calificación:  "+ rating.Calification.ToString());
                    Console.WriteLine("=> Comentario:    "+ rating.Review);
                    Console.WriteLine("-------------------------");
                }
                Console.WriteLine("¿Desea descargar la carátula?");
                Console.WriteLine("==> Si   (Digite 1)");
                Console.WriteLine("==> No   (Digite 2)");
                string option = Console.ReadLine();
                Console.WriteLine("");
                if (option.Equals("1"))
                {
                    success = SendMessage(aGame.Cover, action.GameCover);
                    if (!success) { return false; }
                    this.fileCommunication = new FileCommunicationHandler(clientSocket);
                    fileCommunication.ReceiveFile();
                    string responseCover = ProtocolDataProgram.Listen(clientSocket);
                    Console.WriteLine(responseCover);
                    Console.WriteLine("");
                }
                Console.WriteLine("-------------------------------------");
            }
            return true;
        }

        public bool SearchGame()
        {
            Console.WriteLine("----------BUSCAR UN JUEGO------------");
            Console.WriteLine(" Busqueda por: ");
            Console.WriteLine("==> Titulo:      (ingrese 1)");
            Console.WriteLine("==> Categoria:    (ingrese 2)");
            Console.WriteLine("==> Rating:      (ingrese 3)");
            string searchBy = Console.ReadLine();
            Console.WriteLine("");
            action searchAction = action.Default;
            string searchQuery = "";
            bool success = true;
            switch (searchBy)
            {
                case "1":
                    Console.WriteLine("Ingrese el titulo del juego buscado");
                    searchAction = action.SearchGameByTitle;
                    searchQuery = Console.ReadLine();
                    Console.WriteLine("");
                    success = SendMessage(searchQuery, searchAction);
                    if (!success) { return false; }
                    break;
                case "2":
                    Console.WriteLine("Ingrese el genero del juego buscado");
                    Console.WriteLine("==> Accion       (Ingrese 1)");
                    Console.WriteLine("==> Aventura     (Ingrese 2)");
                    Console.WriteLine("==> Estrategia   (Ingrese 3)");
                    Console.WriteLine("==> Infantil     (Ingrese 4)");
                    Console.WriteLine("==> Deporte      (Ingrese 5)");
                    Console.WriteLine("==> Otros        (Ingrese 6)");
                    string gender = Console.ReadLine();
                    Console.WriteLine("");
                    GameGender genderGame = GameGender.Otros;
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
                        case "5":
                            genderGame = GameGender.Deporte;
                            break;
                        case "6":
                            genderGame = GameGender.Otros;
                            break;
                    }
                    searchAction = action.SearchGameByGender;
                    success = SendMessage(genderGame.ToString(), searchAction);
                    if (!success) { return false; }
                    break;
                case "3":
                    Console.WriteLine("Ingrese el raiting del juego buscado");
                    Console.WriteLine("==> Muy Malo:    (Ingrese 1)");
                    Console.WriteLine("==> Malo:        (Ingrese 2)");
                    Console.WriteLine("==> Medio:       (Ingrese 3)");
                    Console.WriteLine("==> Bueno:       (Ingrese 4)");
                    Console.WriteLine("==> Muy Bueno:   (Ingrese 5)");
                    searchQuery = Console.ReadLine();
                    Console.WriteLine("");
                    GameCalification calification = GameCalification.Sin_Calificaciones;
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
                    success = SendMessage(calification.ToString(), searchAction);
                    if (!success) { return false; }
                    break;
            }
            string response = ProtocolDataProgram.Listen(clientSocket);
            string[] info = response.Split(Environment.NewLine);
            if (info[0].Equals("N"))
            {
                Console.WriteLine(info[1]);
                Console.WriteLine("");
            }
            else
            {

                List<Game> games = ProtocolDataProgram.DeserializeGameList(info[1]);
                Console.WriteLine("Los siguientes juegos cumplen con su busqueda:");
                Console.WriteLine("");
                int count = 1;
                foreach (Game game in games)
                {
                    Console.WriteLine(" Juego " + count + ":");
                    Console.WriteLine("=> Titulo:       " + game.Title);
                    Console.WriteLine("=> Sinopsis:     " + game.Synopsis);
                    Console.WriteLine("=> Categoría:    "  + game.Gender);
                    Console.WriteLine("==============================");
                    Console.WriteLine("");
                    count++;
                }
            }
            return true;
        }
    }
}
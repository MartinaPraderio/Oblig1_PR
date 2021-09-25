using System;
using System.Net.Sockets;
using Domain;
using ProtocolData;
using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Client
{
    public class ClientServicesManager 
    {
        private ProtocolDataProgram protocolHandleData;

        private Socket clientSocket;

        private string userName;

        public ClientServicesManager(Socket client, string userName)
        {
            this.protocolHandleData = new ProtocolDataProgram();
            this.clientSocket = client;
            this.userName = userName;
        }


        public void SendMessage(string message, action action)
        {
            ProtocolDataProgram.Send(clientSocket, action.ToString());
            ProtocolDataProgram.Send(clientSocket, message);
        }


        //public void sendusername(Socket clientsocket, string username)
        //{
        //    //username = "[0]" + username;
        //    //llamar a otra funcion dentro de esta clase que
        //    Sendmessage(username, action.Sendusername);

        //}

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
            Game game = new Game(title, genderGame, synopsis, "cover");
            string message = System.Text.Json.JsonSerializer.Serialize(game);
            SendMessage(message, action.PublishGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
        }



        //public override void qualifygame() { }
        //public override game searchgame() { return new game(); }
        //public override void gamedetails() { }

        public void RequestServerConnection(){}
        public void EndServerConnection(){}
        public void ModifyGame(){
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
            string message = title+ Environment.NewLine + System.Text.Json.JsonSerializer.Serialize(game);
            SendMessage(message, action.ModifyGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }
        public void DeleteGame(){
            Console.WriteLine("ingrese el titulo del juego a eliminar");
            string title = Console.ReadLine();
            SendMessage(title, action.DeleteGame);
            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
        }

        internal void QualifyGame()
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


            // Le mando el username al server en el message, pero de forma provisoria. Me parece mejor hacer q sea
            // parte del protocolo, osea que el client para cualquier accion q quiera hacer se identifique mandando
            // su username. Despues, del lado del server a veces es necesario ver que user nos mando la request y a
            // veces no. X ej en este caso lo necesitamos para saber de q user es la rating
            string message = gameTitleToQualify + Environment.NewLine + calification.ToString() + Environment.NewLine + review + Environment.NewLine + userName ;
            SendMessage(message, action.QualifyGame);

            string response = ProtocolDataProgram.Listen(clientSocket);
            Console.WriteLine(response);
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
                
                List<Game> games = JsonConvert.DeserializeObject<List<Game>>(info[1]);
                Console.WriteLine("Los siguientes juegos cumplen con su busqueda");
                int count = 1;
                foreach (Game game in games)
                {
                    Console.WriteLine("Juego "+ count + ":");
                    Console.WriteLine("Titulo: " + game.Title);
                    Console.WriteLine("Sinopsis: " + game.Synopsis);
                    Console.WriteLine("Categoría: " + game.Gender);
                    Console.WriteLine("---------------------------");
                    count++;
                }
            }
            
        }

        internal void SendEmptyMessage()
        {
            ProtocolDataProgram.Send(clientSocket, "");
        }
    }
}


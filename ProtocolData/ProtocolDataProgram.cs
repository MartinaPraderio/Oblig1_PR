using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Domain;

namespace ProtocolData
{
    public enum action {
        Default,
        PublishGame,
        NotifyUsername,
        DeleteGame,
        ModifyGame,
        SearchGameByTitle,
        SearchGameByGender,
        SearchGameByRating,
        QualifyGame,
        GameDetails,
        GameCover,
        ViewCatalogue,
        BuyGame,
        LogOut,
        ViewUserGames
    }

    public class ProtocolDataProgram
    {

        public ProtocolDataProgram() { }
        private const int ProtocolFixedSize = 4;

        public static void Send(NetworkStream networkStream, string message)
        {    
                //2 Codifico el mensaje a bytes
                byte[] data = Encoding.UTF8.GetBytes(message);
                //3 Leo el largo del mensaje codificado
                //    (largo codificado <> largo mensaje)
                int length = data.Length;
                //4 Codifico dicho largo a bytes
                byte[] dataLength = BitConverter.GetBytes(length);

                //5 Envío el largo en bytes
                SendBatch(networkStream, dataLength);

                //6 Envío el mensaje
                SendBatch(networkStream, data);
            
        }

        public static string Listen(NetworkStream networkStream)
        {
            string dataString = "";
            //1 Creo la parte fija del protocolo
            byte[] dataLength = new byte[ProtocolFixedSize];
            //2 Recibo los datos fijos

            //tcpClient.Receive(dataLength); //ver como hacer esto
            networkStream.Read(dataLength);

            //3 Interpreto dichos bytes para obtener cuanto serán los datos variables
            int length = BitConverter.ToInt32(dataLength);
            //4 Creo que el buffer del tamaño exacto de el mensaje que va a venir
            byte[] data = new byte[length];
            //5 Recibo el mensaje (largo variable, que ahora se que es length)

            //tcpClient.Receive(data);  //ver como hacer esto
            networkStream.Read(data);

            //6 Convierto los datos a string
            string message = Encoding.UTF8.GetString(data);
            //7 Uso los datos
            dataString = message;
            return dataString;
        }

        private static void SendBatch(NetworkStream networkStream , byte[] data)
        {
            /* int totalDataSent = 0;

             while (totalDataSent < data.Length)
             {
                 int sent = tcpClient.Send(data, totalDataSent, data.Length - totalDataSent, SocketFlags.None); //ver como hacer esto con tcp
                 if (sent == 0)
                 {
                     throw new SocketException();
                 }
                 //totalDataSent += sent;
             }*/
            networkStream.WriteAsync(data, 0, data.Length);

        }

        public static string SerializeGame(Game game)
        {
            return  game.Title + ProtocolIdentifiers.GameAtributeSeparator +
                    game.Gender.ToString() + ProtocolIdentifiers.GameAtributeSeparator +
                    game.Synopsis + ProtocolIdentifiers.GameAtributeSeparator +
                    game.Cover + ProtocolIdentifiers.GameAtributeSeparator +
                    SerializeUserRatingList(game.UserRatings);
        }

        private static string SerializeUserRatingList(List<UserRating> userRatings)
        {
            string serialized = "";
            foreach (UserRating rating in userRatings)
            {
                serialized += ProtocolIdentifiers.UserRatingSeparator+ SerializeUserRating(rating);
            }
            return serialized;
        }
        private static List<UserRating> DeserializeUserRatingList(string serialized)
        {
            List<UserRating> ratings = new List<UserRating>();
            string[] serializedRatings = serialized.Split(ProtocolIdentifiers.UserRatingSeparator);
            if (serializedRatings.Length > 1)
            {
                for (int i = 1; i < serializedRatings.Length; i++)
                {
                    ratings.Add(DeserializeUserRating(serializedRatings[i]));
                }
            }
            
            return ratings;
        }

        private static UserRating DeserializeUserRating(string serializedRating)
        {
            string[] ratingInfo = serializedRating.Split(ProtocolIdentifiers.UserRatingAtributeSeparator);
            User aUser = new User(ratingInfo[2]);
            string review = ratingInfo[1].Split(ProtocolIdentifiers.UserRatingSeparator)[0];
            UserRating aRating = new UserRating(review, ParseGameCalification(ratingInfo[0]), aUser);
            return aRating;
        }

        public static GameCalification ParseGameCalification(string calification)
        {
            List<GameCalification> califications = new List<GameCalification>();
            califications.Add(GameCalification.Muy_Malo);
            califications.Add(GameCalification.Malo);
            califications.Add(GameCalification.Medio);
            califications.Add(GameCalification.Bueno);
            califications.Add(GameCalification.Muy_Bueno);
            GameCalification parsed = GameCalification.Sin_Calificaciones;
            foreach (GameCalification aCalification in califications)
            {
                if (aCalification.ToString().Equals(calification))
                {
                    parsed = aCalification;
                }
            }
            return parsed;
        }

        private static string SerializeUserRating(UserRating rating)
        {
            return rating.Calification + ProtocolIdentifiers.UserRatingAtributeSeparator + rating.Review +
                ProtocolIdentifiers.UserRatingAtributeSeparator + rating.User.UserName;
        }

        public static Game DeserializeGame(string serialized)
        {
            string[] parseInfo = serialized.Split(ProtocolIdentifiers.GameAtributeSeparator);
            string title = parseInfo[0];
            GameGender gender = ParseGameGender(parseInfo[1]);
            string synopsis = parseInfo[2];
            string fileName = parseInfo[3];
            List<UserRating> ratings = DeserializeUserRatingList(parseInfo[4]);
            Game deserialized = new Game(title, gender, synopsis, fileName,ratings);
            return deserialized;
        }

        public static GameGender ParseGameGender(string gender)
        {
            List<GameGender> genders = new List<GameGender>();
            genders.Add(GameGender.Accion);
            genders.Add(GameGender.Aventura);
            genders.Add(GameGender.Estrategia);
            genders.Add(GameGender.Infantil);
            genders.Add(GameGender.Deporte);
            genders.Add(GameGender.Otros);
            GameGender parsed = GameGender.Otros;
            foreach (GameGender aGender in genders)
            {
                if (aGender.ToString().Equals(gender))
                {
                    parsed = aGender;
                }
            }
            return parsed;
        }

        public static string SerializeGameList(List<Game> games)
        {
            string serialized = "";
            foreach (Game game in games)
            {
                serialized += ProtocolIdentifiers.GamesSeparator + SerializeGame(game);
            }
            return serialized;
        }

        public static List<Game> DeserializeGameList(string games)
        {
            List<Game> deserialized = new List<Game>();
            string[] serializedGames = games.Split(ProtocolIdentifiers.GamesSeparator);

            for (int i = 1; i < serializedGames.Length; i++)
            {
                Game aGame = DeserializeGame(serializedGames[i]);
                deserialized.Add(aGame);
            }
            return deserialized;
        }
    }
}
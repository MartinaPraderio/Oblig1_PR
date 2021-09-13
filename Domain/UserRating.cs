namespace Domain
{

    public enum GameCalification{
        Muy_Malo,
        Malo,
        Medio,
        Bueno,
        Muy_Bueno
    }

    public class UserRating
    {
        public string Review { get; set; }
        public GameCalification Calification { get; set; }
        public User User { get; set; }

        public UserRating()
        {
        }
    }
}


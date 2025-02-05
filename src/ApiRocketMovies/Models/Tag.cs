namespace ApiRocketMovies.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        public Movie Movie { get; set; }
        public User User { get; set; }
    }
}

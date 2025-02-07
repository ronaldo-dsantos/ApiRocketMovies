using System.Text.Json.Serialization;

namespace ApiRocketMovies.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public Movie Movie { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace ApiRocketMovies.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 

        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; }
        [JsonIgnore]
        public ICollection<Tag> Tags { get; set; }
    }
}

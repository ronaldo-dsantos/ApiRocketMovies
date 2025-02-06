using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs
{
    public class CreateMovieDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Rating { get; set; }        
        public ICollection<string> Tags { get; set; }
    }
}

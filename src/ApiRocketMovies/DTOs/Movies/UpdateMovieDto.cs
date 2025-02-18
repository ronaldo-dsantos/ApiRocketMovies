using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs.Movies
{
    public class UpdateMovieDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória.")]        
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "As tags são obrigatórias.")]
        public ICollection<string> Tags { get; set; }
    }
}

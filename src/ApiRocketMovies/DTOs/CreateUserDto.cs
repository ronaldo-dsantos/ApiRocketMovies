using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório.")]
        public string Password { get; set; }
    }
}

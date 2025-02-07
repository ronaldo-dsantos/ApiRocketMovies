using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs
{
    public class UpdateUserDto
    {
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}

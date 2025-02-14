using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "Senha inválida. A senha deve ter no mínimo 6 caracteres, incluir pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial.")]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ApiRocketMovies.DTOs.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]        
        public string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$", ErrorMessage = "Senha inválida. A senha deve ter no mínimo 6 caracteres, incluir pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial.")]
        public string Password { get; set; }
    }
}

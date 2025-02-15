using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ApiRocketMovies.DTOs
{
    public class AvatarDto
    {
        [NotMapped]
        [DisplayName("Avatar do Usuário")]
        public IFormFile AvatarFile { get; set; }
    }
}

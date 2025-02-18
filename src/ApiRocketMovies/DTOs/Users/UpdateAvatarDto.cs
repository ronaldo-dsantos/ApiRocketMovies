using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ApiRocketMovies.DTOs.Users
{
    public class UpdateAvatarDto
    {
        [NotMapped]
        [DisplayName("Avatar do Usuário")]
        public IFormFile AvatarFile { get; set; }
    }
}

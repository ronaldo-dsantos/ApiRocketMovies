using ApiRocketMovies.DTOs;
using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiRocketMovies.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class UserAvatarController : ControllerBase
    {        
        private readonly UserManager<User> _userManager;

        public UserAvatarController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult<UserDto>> Update([FromForm] AvatarDto avatarDto)
        {
            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Buscar usuário no banco de dados
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }

            // Verifica se o arquivo foi enviado
            if (avatarDto.AvatarFile == null || avatarDto.AvatarFile.Length == 0)
            {
                return BadRequest(new { Message = "Nenhum arquivo foi enviado." });
            }

            // Define o diretório de upload e verifica se ele existe, senão cria
            var uploadPath = Path.Combine("wwwroot", "images");
                       
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Verifica a extensão do arquivo
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(avatarDto.AvatarFile.FileName).ToLower();

            if (!extensoesPermitidas.Contains(fileExtension))
            {
                return BadRequest(new { Message = "Formato de imagem inválido. Apenas .jpg, .jpeg, .png e .gif são permitidos." });
            }


            // Sanitiza o nome do arquivo
            var safeFileName = Path.GetFileNameWithoutExtension(avatarDto.AvatarFile.FileName);
            safeFileName = string.Join("_", safeFileName.Split(Path.GetInvalidFileNameChars())); // Remove caracteres inválidos
            var fileName = $"{Guid.NewGuid()}_{safeFileName}{fileExtension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await avatarDto.AvatarFile.CopyToAsync(stream);
                }

                // Remove o avatar antigo se existir
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    var oldAvatarPath = Path.Combine(uploadPath, user.Avatar);
                    if (System.IO.File.Exists(oldAvatarPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldAvatarPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao excluir avatar antigo: {ex.Message}");
                        }
                    }
                }

                // Atualiza o avatar do usuário
                user.Avatar = fileName;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return BadRequest(new { Message = "Falha ao atualizar o avatar do usuário." });
                }

                // Criar resposta estruturada
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Ok(new { User = userDto, Message = "Avatar atualizado com sucesso." });                
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Erro interno ao processar o upload do avatar." });
            }
        }
    }
}















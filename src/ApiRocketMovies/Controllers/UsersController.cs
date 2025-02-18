using ApiRocketMovies.DTOs.Users;
using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiRocketMovies.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;        

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;            
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserDto createUserDto)
        {
            // Validar o modelo
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Verificar se o e-mail já está em uso            
            if (await _userManager.FindByEmailAsync(createUserDto.Email) != null)
            {
                return BadRequest(new { Message = "Este e-mail já está em uso." });
            }

            var user = new User
            {
                Name = createUserDto.Name,
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Criar usuário no Identity
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Falha ao registrar o usuário." });
            }

            return Created();
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update(UpdateUserDto updateUserDto)
        {
            // Validar o modelo
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Obter o usuário a ser atualizado
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }

            // Verifica se o e-mail já está em uso por outro usuário
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var emailExists = await _userManager.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != user.Id);
                if (emailExists)
                {
                    return BadRequest(new { Message = "Este e-mail já está em uso por outro usuário." });
                }

                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email;
            }

            // Atualiza o nome, se fornecido
            user.Name = updateUserDto.Name ?? user.Name;
            user.UpdatedAt = DateTime.Now;

            // Atualiza a senha, se fornecida
            if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                if (string.IsNullOrEmpty(updateUserDto.OldPassword))
                {
                    return BadRequest(new { Message = "Para alterar a senha, informe a senha antiga." });
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, updateUserDto.OldPassword, updateUserDto.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(new { Message = "Erro ao alterar a senha." });
                }
            }         

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { Message = "Falha ao atualizar os dados do usuário." });
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

            return Ok(new { User = userDto, Message = "Usuário atualizado com sucesso." });
        }
    }
}

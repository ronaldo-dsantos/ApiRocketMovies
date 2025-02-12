using ApiRocketMovies.DTOs;
using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ApiRocketMovies.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public UsersController(SignInManager<User> signInManager,
                              UserManager<User> userManager,
                              IOptions<JwtSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var checkEmailExists = await _userManager.Users.AnyAsync(u => u.Email == createUserDto.Email);
            if (checkEmailExists)
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

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Created();
            }

            return BadRequest(new { Message = "Falha ao registrar o usuário" });
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Update(UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                var emailExists = await _userManager.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != userId);
                if (emailExists)
                {
                    return BadRequest(new { Message = "Este e-mail já está em uso por outro usuário." });
                }
            }

            user.Name = updateUserDto.Name ?? user.Name;

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email; 
            }

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                if (string.IsNullOrEmpty(updateUserDto.OldPassword))
                {
                    return BadRequest(new { Message = "Para alterar a senha, informe a senha antiga." });
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, updateUserDto.OldPassword);
                if (!passwordValid)
                {
                    return BadRequest(new { Message = "Senha antiga incorreta." });
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, updateUserDto.OldPassword, updateUserDto.Password);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(new { Message = "Falha ao atualizar a senha." });
                }
            }

            user.UpdatedAt = DateTime.Now;            
         
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { Message = "Falha ao atualizar os dados do usuário." });
            }

            return Ok(new { Message = "Usuário atualizado com sucesso." });
        }
    }
}

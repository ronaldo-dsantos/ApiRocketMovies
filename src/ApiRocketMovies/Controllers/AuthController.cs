using ApiRocketMovies.DTOs;
using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiFuncional.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(SignInManager<User> signInManager,
                              UserManager<User> userManager,
                              IOptions<JwtSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginUserDto loginUser)
        {
            // Validar modelo
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Autenticar usuário
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Email ou senha incorretos" });
            }

            // Buscar usuário
            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Erro inesperado ao buscar usuário." });
            }

            // Criar resposta estruturada
            var response = new AuthResponseDto
            {
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                },
                Token = GerarJwt(user)
            };

            return Ok(response);
        }

        private string GerarJwt(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Emissor,
                Audience = _jwtSettings.Audiencia,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}




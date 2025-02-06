﻿using ApiRocketMovies.Data;
using ApiRocketMovies.DTOs;
using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRocketMovies.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                return BadRequest(new { Message = "Dados inválidos." });
            }

            var checkEmailExists = await _context.Users.AnyAsync(u => u.Email == createUserDto.Email);
            if (checkEmailExists)
            {           
                return BadRequest(new { Message = "Este e-mail já está em uso." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Name, user.Email });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<User>> Update(int id, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {             
                return BadRequest(new { Message = "Dados inválidos." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {                
                return BadRequest(new { Message = "Usuário não encontrado." });
            }

            var emailExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateUserDto.Email);
            if (emailExists != null && emailExists.Id != user.Id)
            {
                return BadRequest(new { Message = "Este e-mail já está em uso." });
            }

            user.Name = updateUserDto.Name ?? user.Name;
            user.Email = updateUserDto.Email ?? user.Email;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                if (string.IsNullOrEmpty(updateUserDto.OldPassword))
                {
                    return BadRequest(new { Message = "Para alterar a senha, informe a senha antiga." });
                }

                var isOldPasswordValid = BCrypt.Net.BCrypt.Verify(updateUserDto.OldPassword, user.Password);
                if (!isOldPasswordValid)
                {
                    return BadRequest(new { Message = "Senha antiga incorreta." });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Name, user.Email });
        }
    }
}

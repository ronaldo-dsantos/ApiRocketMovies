using ApiRocketMovies.Data;
using Microsoft.AspNetCore.Mvc;
using ApiRocketMovies.Models;
using ApiRocketMovies.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiRocketMovies.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieDto createMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            if (createMovieDto.Tags.Any(tag => string.IsNullOrWhiteSpace(tag)))
            {
                return BadRequest(new { Message = "As tags não podem ser vazias." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var movie = new Movie
                {
                    Title = createMovieDto.Title,
                    Description = createMovieDto.Description,
                    Rating = createMovieDto.Rating,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                foreach (var tagName in createMovieDto.Tags)
                {
                    var newTag = new Tag
                    {
                        MovieId = movie.Id,
                        UserId = userId,
                        Name = tagName,
                    };
                    _context.Tags.Add(newTag);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Filme criado com sucesso." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocorreu um erro ao criar o filme." });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShowMovieDto>> Show(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            var movie = await _context.Movies
                .Include(m => m.Tags)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (movie == null)
            {
                return NotFound(new { Message = "Filme não encontrado ou você não tem permissão para visualizá-lo." });
            }

            var showMovieDto = new ShowMovieDto
            {
                UserName = movie.User.Name,
                UserAvatar = movie.User.Avatar,
                Movie = new MovieDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Rating = movie.Rating,
                    UserId = movie.UserId,
                    CreatedAt = movie.CreatedAt,
                    UpdatedAt = movie.UpdatedAt,
                    Tags = movie.Tags
                }
            };

            return Ok(showMovieDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> Index(string title = null)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            var query = _context.Movies
                .Include(m => m.Tags)
                .Where(m => m.UserId == userId);

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{title}%"));
            }

            var movies = await query
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Rating = m.Rating,
                    UserId = m.UserId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Tags = m.Tags
                })
                .ToListAsync();

            return Ok(movies);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (movie == null)
            {
                return NotFound(new { Message = "Filme não encontrado ou você não tem permissão para excluí-lo." });
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Filme deletado com sucesso." });
        }
    }
}

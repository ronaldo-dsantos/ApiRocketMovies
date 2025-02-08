using ApiRocketMovies.Data;
using Microsoft.AspNetCore.Mvc;
using ApiRocketMovies.Models;
using ApiRocketMovies.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ApiRocketMovies.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> Create(int id, CreateMovieDto createMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
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
                    UserId = id,
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
                        UserId = id,
                        Name = tagName,
                    };
                    _context.Tags.Add(newTag);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok();
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
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.Tags)
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    return NotFound(new { Message = "Filme não encontrado." });
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar o filme: {ex.Message}");

                return StatusCode(500, new { Message = "Ocorreu um erro ao buscar o filme." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> Index(int userId, string title = null)
        {
            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar os filmes para o usuário {userId} com o título '{title}': {ex.Message}");
                return StatusCode(500, new { Message = "Ocorreu um erro ao buscar os filmes." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {                
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { Message = "Filme não encontrado." });
                }
                
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Filme deletado com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar o filme: {ex.Message}");
                return StatusCode(500, new { Message = "Ocorreu um erro ao deletar o filme." });
            }
        }
    }
}

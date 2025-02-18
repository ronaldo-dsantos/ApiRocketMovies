using ApiRocketMovies.Data;
using Microsoft.AspNetCore.Mvc;
using ApiRocketMovies.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApiRocketMovies.DTOs.Movies;

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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesAll(string title = null)
        {
            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Montar a query e adicionar o filtro de título se for informado
            var query = _context.Movies
                .Include(m => m.Tags)
                .Where(m => m.UserId == userId);
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{title}%"));
            }

            // Executar a query e retornar os filmes
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetMovieDto>> GetMovieById(int id)
        {
            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o usuário está autenticado
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Obter o filme
            var movie = await _context.Movies
                .Include(m => m.Tags)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (movie == null)
            {
                return NotFound(new { Message = "Filme não encontrado ou você não tem permissão para visualizá-lo." });
            }

            var getMovieDto = new GetMovieDto
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

            return Ok(getMovieDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieDto createMovieDto)
        {
            // Validação do modelo
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o usuário está autenticado
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Verificar se as tags estão preenchidas
            if (createMovieDto.Tags.Any(tag => string.IsNullOrWhiteSpace(tag)))
            {
                return BadRequest(new { Message = "As tags não podem ser vazias." });
            }

            // Iniciar uma transação
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Criar um novo filme
                var movie = new Movie
                {
                    Title = createMovieDto.Title,
                    Description = createMovieDto.Description,
                    Rating = createMovieDto.Rating,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                // Adicionar o filme ao contexto
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                // Adicionar as tags ao filme
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMovie(int id, UpdateMovieDto updateMovieDto)
        {
            // Validação do modelo
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar se o filme existe e pertence ao usuário
            var movie = await _context.Movies
                .Include(m => m.Tags)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (movie == null)
            {
                return NotFound(new { Message = "Filme não encontrado ou não pertence ao usuário." });
            }

            // Verificar se as tags estão preenchidas
            if (updateMovieDto.Tags.Any(tag => string.IsNullOrWhiteSpace(tag)))
            {
                return BadRequest(new { Message = "As tags não podem ser vazias." });
            }

            // Iniciar uma transação
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Atualizar os dados do filme
                movie.Title = updateMovieDto.Title;
                movie.Description = updateMovieDto.Description;
                movie.Rating = updateMovieDto.Rating;
                movie.UpdatedAt = DateTime.Now;

                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                // Atualizar as tags: Remover as antigas e adicionar as novas
                _context.Tags.RemoveRange(movie.Tags); // Remove todas as tags associadas ao filme
                await _context.SaveChangesAsync();

                var newTags = updateMovieDto.Tags.Select(tagName => new Tag
                {
                    MovieId = movie.Id,
                    UserId = userId,
                    Name = tagName
                }).ToList();

                await _context.Tags.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Filme atualizado com sucesso." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocorreu um erro ao atualizar o filme." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            // Obter o ID do usuário autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Usuário não autenticado." });
            }

            // Obter o filme e verificar se ele pertence ao usuário autenticado
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

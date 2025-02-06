using ApiRocketMovies.Data;
using Microsoft.AspNetCore.Mvc;
using ApiRocketMovies.Models;
using ApiRocketMovies.DTOs;

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
    }
}

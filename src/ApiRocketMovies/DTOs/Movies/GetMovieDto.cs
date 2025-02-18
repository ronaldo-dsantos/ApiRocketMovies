namespace ApiRocketMovies.DTOs.Movies
{
    public class GetMovieDto
    {
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public MovieDto Movie { get; set; }
    }
}

namespace ApiRocketMovies.Utils
{
    public class AppError
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public AppError(string message, int statusCode = 400)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}

namespace ApiRocketMovies.DTOs
{
    public class UserResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

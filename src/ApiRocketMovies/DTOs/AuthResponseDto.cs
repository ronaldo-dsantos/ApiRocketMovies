﻿namespace ApiRocketMovies.DTOs
{
    public class AuthResponseDto
    {
        public UserResponseDto User { get; set; }
        public string Token { get; set; }
    }

    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

﻿using System.Text.Json.Serialization;

namespace ApiRocketMovies.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}

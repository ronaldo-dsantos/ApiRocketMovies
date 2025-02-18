namespace ApiRocketMovies.Configuration
{
    public static class CorsConfig
    {
        public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Development", policy => policy
                                                .AllowAnyOrigin()
                                                .AllowAnyHeader()
                                                .AllowAnyMethod());

                options.AddPolicy("Production", policy => policy
                                                .WithOrigins("http://localhost:5173")
                                                .AllowAnyHeader()
                                                .AllowAnyMethod());
            });

            return builder;
        }
    }
}

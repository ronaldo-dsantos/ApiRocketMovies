using ApiRocketMovies.DTOs.Users;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiRocketMovies.Configuration
{
    public static class SwaggerConfig
    {
        public static WebApplicationBuilder AddSwaggerConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                c.SchemaFilter<ExampleSchemaFilter>();
            });

            return builder;
        }

        public class ExampleSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                var examples = new Dictionary<Type, OpenApiObject>
                {
                    [typeof(CreateUserDto)] = new OpenApiObject
                    {
                        ["name"] = new OpenApiString("João Silva"),
                        ["email"] = new OpenApiString("joao@email.com"),
                        ["password"] = new OpenApiString("Senha@123")
                    },
                    [typeof(LoginUserDto)] = new OpenApiObject
                    {
                        ["email"] = new OpenApiString("joao@email.com"),
                        ["password"] = new OpenApiString("Senha@123")
                    }
                    ,
                    [typeof(UpdateUserDto)] = new OpenApiObject
                    {
                        ["name"] = new OpenApiString("João Silva"),
                        ["email"] = new OpenApiString("joao@email.com"),
                        ["oldPassword"] = new OpenApiString("Senha@123"),
                        ["newPassword"] = new OpenApiString("NovaSenha@123")
                    }
                };

                if (examples.TryGetValue(context.Type, out var example))
                {
                    schema.Example = example;
                }
            }
        }
    }
}

using ApiRocketMovies.DTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace ApiRocketMovies.Configuration
{
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

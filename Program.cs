
using _4VGymAPI.Models;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Añadir los controladores
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errorList = context.ModelState
                .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                {
                    // Intentamos separar "1001|Mensaje de error"
                    var parts = error.ErrorMessage.Split('|');

                    if (parts.Length == 2 && int.TryParse(parts[0], out int parsedCode))
                    {
                        return new ErrorResponse(parsedCode, parts[1]);
                    }

                    // Si no tiene delimitador o falla el parseo
                    return new ErrorResponse(9999, error.ErrorMessage);
                }))
                .ToList();

            return new BadRequestObjectResult(errorList);
        };
    });


// Añadir el soporte a OpenAPI:  https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Añadir el soporte a nuestro repo
builder.Services.AddSingleton<InMemoryActivityTypeRepository>();
builder.Services.AddSingleton<InMemoryInstructorRepository>();
builder.Services.AddSingleton<InMemoryActivityRepository>();

// Contruir la aplicacion
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // 1. Expone el endpoint del documento OpenAPI nativo (ej: /openapi/v1.json)
    app.MapOpenApi();

    // Si instalaste Swashbuckle para seguir usando Swagger UI:
    app.UseSwaggerUI(options =>
    {
        // Le indicamos dónde está el archivo JSON generado por .NET
        options.SwaggerEndpoint("/openapi/v1.json", "RestAPI for 4VGym v1");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

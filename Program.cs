
using _4VGymAPI.Data;
using _4VGymAPI.Dtos;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Añadir los controladores
builder.Services.AddControllers()
    // Activar la serialización de ciclos de referencia para evitar errores al serializar entidades con relaciones circulares
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
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

// Registro de Entity Framework Core (DbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // DefaultConnection: Es el nombre de la cadena de conexión definida en el archivo appsettings.json
// Registrar el DbContext
builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlServer(connectionString));

// Añadir los repositorios: Scoped, igual que el DbContext (una instancia por petición HTTP)
builder.Services.AddScoped<ActivityTypeRepository>();
builder.Services.AddScoped<InstructorRepository>();
builder.Services.AddScoped<ActivityRepository>();

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
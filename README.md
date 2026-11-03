# 4VGym con BBDD relacional

API REST para gestionar las actividades del gimnasio Cuatrovientos (4VGym). Une los dos proyectos de clase:

- **4VGymAPI**: la API completa, con los datos en memoria (listas y diccionarios).
- **4VGymEF**: los modelos y la BBDD SQL Server, creada y gestionada con Entity Framework Core (Code-First).

Este proyecto expone **la misma API que 4VGymAPI** (mismas rutas, operaciones, parámetros, JSON y códigos de error), pero lee y guarda los datos en SQL Server con Entity Framework. La unión entre los dos mundos son las clases **repositorio**.

## Requisitos

- .NET 10 SDK
- SQL Server (probado con SQL Server Express, instancia `.\SqlExpress`) y SQL Server Management Studio
- Herramienta de Entity Framework (una sola vez):

```powershell
dotnet tool install --global dotnet-ef
```

## Puesta en marcha

1. **Cadena de conexión.** Revisa `DefaultConnection` en `appsettings.json` y pon tu servidor.
2. **Crear la BBDD** con las migraciones (crea la BBDD `4vGym` y sus tablas):

   ```powershell
   dotnet ef database update
   ```

3. **Cargar los datos de prueba**: abre `Sql/datos.sql` en SQL Server Management Studio, selecciona la BBDD `4vGym` y ejecútalo. Hay que hacerlo sobre la BBDD recién creada, para que los ids empiecen en 1.
4. **Arrancar la API**:

   ```powershell
   dotnet run --launch-profile http
   ```

   Swagger UI queda en <http://localhost:5117/swagger/index.html>.

5. **Probar** con el fichero `4VGymAPI.http` desde Visual Studio, o importando el swagger (`/openapi/v1.json`) en Postman.

Para volver al estado inicial: eliminar la BBDD `4vGym` en SSMS, `dotnet ef database update` y ejecutar otra vez `Sql/datos.sql`.

## Base de datos

```
ActivityTypes (Id, Name, NumberMonitors)
      1
      |
      N
Activities (Id, ActivityTypeId, MaxParticipants, DateStart, DateEnd)
      N
      |               ActivityInstructor (ActivitiesId, InstructorsId)
      M
Instructors (Id, Name, Email, PhotoUrl, Phone)
```

- **ActivityTypes → Activities (1:N)**: `Activity.ActivityTypeId` con `[ForeignKey]` y la navegación `ActivityType`; en `ActivityType` la colección inversa `Activities`.
- **Activities ↔ Instructors (M:N)**: una colección en cada clase (`Activity.Instructors` e `Instructor.Activities`). EF genera la tabla intermedia `ActivityInstructor`.
- El teléfono del monitor es opcional en la BBDD (`Phone` admite `NULL`), aunque la API lo exige al crear un monitor.
- La relación 1:N se configura también con Fluent API en `GymDbContext.OnModelCreating` (nombre de la restricción `FK_Activities_ActivityTypes`).

## Endpoints

Los mismos que 4VGymAPI (el mismo Swagger):

| Método | Ruta | Respuesta OK | Error |
|---|---|---|---|
| GET | `/activity-types` | 200 lista de tipos de actividad | 400 `ErrorResponse` |
| POST | `/activity-types` | 201 tipo de actividad creado | 400 `ErrorResponse` |
| GET | `/instructors?first_initial=M` | 200 lista de monitores (filtro opcional por inicial) | 400 `ErrorResponse` |
| GET | `/instructors/{id}` | 200 monitor | 400 `ErrorResponse` |
| POST | `/instructors` | 201 monitor creado | 400 `ErrorResponse` |
| POST | `/activities` | 201 actividad con su tipo y sus monitores | 400 lista de `ErrorResponse` |

## Estructura del proyecto

| Carpeta | Contenido |
|---|---|
| `Models` | Entidades de la BBDD (`Activity`, `ActivityType`, `Instructor`) con sus propiedades de navegación |
| `Dtos` | Modelos de entrada y salida de la API (`NewActivityType`, `NewInstructor`, `ActivityCreateDto`, `ActivityResponse`) y `ErrorResponse` |
| `Validations` | Validadores propios `[FutureDate]` y `[DateGreaterThan]` |
| `Data` | `GymDbContext`, con un `DbSet` por tabla y la configuración Fluent API |
| `Migrations` | Migración `InitialCreate` que crea la BBDD |
| `Repositories` | Acceso a datos con EF y reglas de negocio |
| `Controllers` | Endpoints de la API |
| `Sql` | Script con los datos de prueba |

## Decisiones de diseño

### ¿Separo modelos de BBDD y DTO para la API? Solo donde hace falta

- `ActivityType` e `Instructor` son entidades y se devuelven directamente por la API. Mantienen sus `[JsonPropertyName]` y sus colecciones de navegación llevan `[JsonIgnore]` para evitar bucles infinitos al serializar.
- `Activity` no puede ser a la vez entidad y modelo de entrada: la petición trae una lista de ids de monitores (`instructors_id`), que no puede ser una columna de la tabla. Por eso la entrada es un DTO (`ActivityCreateDto`) y la salida otro (`ActivityResponse`, con el tipo y los monitores completos).
- Los modelos de entrada `NewActivityType` y `NewInstructor` se mantienen como DTO: no tienen id porque lo genera la BBDD.

### ¿Dónde hago las validaciones? En dos sitios

1. **En el DTO de actividades, con DataAnnotations**: campos obligatorios, rango del aforo, lista de monitores no vacía y, con los validadores propios `[FutureDate]` y `[DateGreaterThan]`, que las fechas sean futuras y la de fin posterior a la de inicio. `Program.cs` (`InvalidModelStateResponseFactory`) las convierte en la lista de `ErrorResponse` usando el formato `"código|mensaje"`.
2. **En los repositorios**: todo lo demás, en especial lo que necesita consultar la BBDD (que existan el tipo de actividad o los monitores, nombres repetidos, monitores ocupados a la misma hora...).

Los **controladores no validan**: llaman al repositorio y transforman el resultado en la respuesta HTTP (200, 201, 400 o 500).

### Repositorios como unión entre la API y la BBDD

- Reciben el `GymDbContext` por inyección de dependencias y se registran con `AddScoped`, igual que el propio `DbContext`: una instancia por petición HTTP.
- Los métodos son asíncronos (`async`/`await` con `ToListAsync`, `FindAsync`, `FirstOrDefaultAsync` y `SaveChangesAsync`).
- Las consultas se escriben con LINQ y EF las traduce a SQL.
- Como los métodos `async` no admiten parámetros `out`, devuelven una tupla `(resultado, error)`.

### Bucles al serializar JSON

Las colecciones de navegación de `ActivityType` e `Instructor` llevan `[JsonIgnore]` y las actividades se devuelven con el DTO `ActivityResponse`. Además, `Program.cs` activa `ReferenceHandler.IgnoreCycles` como red de seguridad.

### Inserciones

El enunciado dice que los POST no tienen por qué insertar, pero sí validar. Aquí validan siempre y, si todo es correcto, **también insertan en la BBDD** (añadir al contexto + `SaveChangesAsync`). En las actividades, EF rellena la tabla intermedia `ActivityInstructor` con los monitores asignados.

## Códigos de error

| Código | Dónde | Significado |
|---|---|---|
| 1 | Controladores | Error inesperado en el servidor |
| 2 | Controladores | No hay tipos de actividad / no existe el monitor |
| 3, 4, 5, 6 | `ActivityTypeRepository` | Datos nulos, sin nombre, monitores < 1, nombre repetido |
| 10 a 17 | `InstructorRepository` | Datos nulos, sin nombre, sin email, email incorrecto, sin teléfono, teléfono incorrecto, foto que no es URL, monitor repetido (email + nombre) |
| 25 a 30 | `ActivityCreateDto` (DataAnnotations) | Aforo 1-25, monitores nulos o vacíos, fechas obligatorias, tipo obligatorio |
| 36, 37 | `ActivityCreateDto` (`[DateGreaterThan]`, `[FutureDate]`) | Fin antes del inicio, fechas en el pasado |
| 34, 35, 38, 39, 40 | `ActivityRepository` | Tipo inexistente, monitor inexistente, menos monitores de los que pide el tipo, monitor ocupado a esa hora, monitores repetidos |

## Diferencias con 4VGymAPI

- Los identificadores son `int` (en Swagger, `int32`) porque así están definidas las claves en la BBDD. En 4VGymAPI eran `long`. El JSON que se envía y se recibe es el mismo.
- En Swagger, el esquema de entrada de `POST /activities` se llama `ActivityCreateDto` en lugar de `Activity`, porque `Activity` es ahora la entidad de la BBDD. Sus campos son los mismos.
- Los errores de fechas (36 y 37) se comprueban con DataAnnotations antes de llegar al repositorio, así que ya no salen en la misma respuesta que los errores 34, 35, 38, 39 y 40. Los códigos y mensajes son los mismos.
- Los datos ya no se pierden al parar la API: están en SQL Server.
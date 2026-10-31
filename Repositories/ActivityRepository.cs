using _4VGymAPI.Data;
using _4VGymAPI.Dtos;
using _4VGymAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace _4VGymAPI.Repositories
{
    public class ActivityRepository
    {
        private readonly GymDbContext _context;
        public ActivityRepository(GymDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Valida y guarda una nueva actividad. Las validaciones básicas ya las hacen las DataAnnotations del DTO,
        /// aquí validamos lo referente a las entidades cruzadas y las reglas de negocio.
        /// </summary>
        /// <param name="newActivity">Actividad a guardar (sin ID).</param>
        /// <returns>Tupla con la actividad guardada (o null) y la lista de errores (vacía si todo va bien).</returns>
        public async Task<(ActivityResponse? activity, List<ErrorResponse>)> Add(ActivityCreateDto newActivity)
        {
            List<ErrorResponse> errors = new List<ErrorResponse>();
            List<int> ids = newActivity.InstructorsId;

            // Validación 1: Que exista el tipo de actividad
            ActivityType? activityType = await _context.ActivityTypes.FindAsync(newActivity.ActivityTypeId!.Value);
            if (activityType == null)
            {
                errors.Add(new ErrorResponse(34, $"El tipo de actividad: {newActivity.ActivityTypeId} no existe"));
            }
            // Validación 2: Que existan todos los monitores (los cargamos todos en una sola consulta)
            List<Instructor> instructors = await _context.Instructors
                .Where(instructor => ids.Contains(instructor.Id))
                .ToListAsync();

            var missingInstructors = ids
                .Distinct()
                .Where(instructorId => !instructors.Any(instructor => instructor.Id == instructorId));

            foreach (var instructorId in missingInstructors)
            {
                errors.Add(new ErrorResponse(35, $"El instructor: {instructorId} no existe"));
            }
            // Validación 3: Que no se repitan monitores en la lista
            if (ids.Distinct().Count() != ids.Count)
            {
                errors.Add(new ErrorResponse(40, "La lista de monitores tiene monitores repetidos"));
            }
            // Validación 4: Número mínimo de monitores según el tipo de actividad
            if (activityType != null && ids.Distinct().Count() < activityType.NumberMonitors)
            {
                errors.Add(new ErrorResponse(38,
                    $"La actividad {activityType.Name} necesita al menos {activityType.NumberMonitors} monitores"));
            }
            // Validación 5: Fechas
            DateTime dateStart = newActivity.DateStart!.Value;
            DateTime dateEnd = newActivity.DateEnd!.Value;

            if (dateEnd <= dateStart)
            {
                errors.Add(new ErrorResponse(36, "La fecha de fin tiene que ser posterior a la fecha de inicio"));
            }
            if (dateStart <= DateTime.Now)
            {
                errors.Add(new ErrorResponse(37, "Las fechas tienen que ser en el futuro"));
            }

            // Validación 6: Los monitores no pueden estar en otras actividades al mismo tiempo
            List<int> busyInstructors = await _context.Activities
                .Where(activity => activity.DateStart < dateEnd && dateStart < activity.DateEnd)
                .SelectMany(activity => activity.Instructors)
                .Select(instructor => instructor.Id)
                .Where(instructorId => ids.Contains(instructorId))
                .Distinct()
                .ToListAsync();

            foreach (var instructorId in busyInstructors)
            {
                errors.Add(new ErrorResponse(39, $"El instructor: {instructorId} ya tiene otra actividad en ese horario"));
            }

            if (errors.Count > 0)
            {
                return (null, errors);
            }
            // Asignamos los objetos cargados: EF rellena la clave foránea y la tabla intermedia ActivityInstructor
            Activity newEntity = new Activity
            {
                ActivityType = activityType,
                Instructors = instructors,
                MaxParticipants = newActivity.MaxParticipants,
                DateStart = dateStart,
                DateEnd = dateEnd
            };

            // Añadimos al contexto y guardamos en la BBDD
            _context.Activities.Add(newEntity);
            await _context.SaveChangesAsync();

            // Devolvemos el modelo de salida con toda la información
            ActivityResponse response = new ActivityResponse
            {
                Id = newEntity.Id,
                ActivityType = activityType,
                Instructors = instructors,
                MaxParticipants = newEntity.MaxParticipants,
                DateStart = newEntity.DateStart,
                DateEnd = newEntity.DateEnd
            };

            return (response, errors);
        }
    }
}
using _4VGymAPI.Models;

namespace _4VGymAPI.Repositories
{
    public class InMemoryActivityRepository
    {
        private List<Activity> _activities = new();

        private readonly InMemoryActivityTypeRepository _repositoryActivityType;
        private readonly InMemoryInstructorRepository _repositoryInstructor;

        private long _nextId = 1; // Para el autonumerico

        public InMemoryActivityRepository(InMemoryActivityTypeRepository repositoryActivityType, InMemoryInstructorRepository repositoryInstructor)
        {
            _repositoryActivityType = repositoryActivityType;
            _repositoryInstructor = repositoryInstructor;
        }

        /// <summary>
        /// Valida y guarda una nueva actividad. Las validaciones básicas ya las hacen las DataAnnotations del modelo,
        /// aquí validamos lo referente a las entidades cruzadas y las reglas de negocio.
        /// </summary>
        /// <param name="activity">Actividad a guardar (sin ID).</param>
        /// <param name="errorResponse">Lista con todos los errores encontrados (vacía si todo va bien).</param>
        /// <returns>La actividad guardada con su ID autogenerado, o null si hay errores.</returns>
        public Activity? Add(Activity activity, out List<ErrorResponse> errorResponse)
        {
            errorResponse = new List<ErrorResponse>();

            // Validación 1: Que exista el tipo de actividad
            ActivityType? activityType = _repositoryActivityType.GetById(activity.ActivityTypeId!.Value);
            if (activityType == null)
            {
                errorResponse.Add(new ErrorResponse(34, $"El tipo de actividad: {activity.ActivityTypeId} no existe"));
            }

            // Validación 2: Que existan todos los monitores
            var missingInstructors = activity.InstructorsId
                .Where(instructorId => _repositoryInstructor.GetById(instructorId) == null);

            foreach (var instructorId in missingInstructors)
            {
                errorResponse.Add(new ErrorResponse(35, $"El instructor: {instructorId} no existe"));
            }

            // Validación 3: Que no se repitan monitores en la lista
            if (activity.InstructorsId.Distinct().Count() != activity.InstructorsId.Count)
            {
                errorResponse.Add(new ErrorResponse(40, "La lista de monitores tiene monitores repetidos"));
            }

            // Validación 4: Número mínimo de monitores según el tipo de actividad
            if (activityType != null && activity.InstructorsId.Distinct().Count() < activityType.NumberMonitors)
            {
                errorResponse.Add(new ErrorResponse(38,
                    $"La actividad {activityType.Name} necesita al menos {activityType.NumberMonitors} monitores"));
            }

            // Validación 5: Fechas
            DateTime dateStart = activity.DateStart!.Value;
            DateTime dateEnd = activity.DateEnd!.Value;

            if (dateEnd <= dateStart)
            {
                errorResponse.Add(new ErrorResponse(36, "La fecha de fin tiene que ser posterior a la fecha de inicio"));
            }
            if (dateStart <= DateTime.Now)
            {
                errorResponse.Add(new ErrorResponse(37, "Las fechas tienen que ser en el futuro"));
            }

            // Validación 6: Los monitores no pueden estar en otras actividades al mismo tiempo
            var busyInstructors = _activities
                .Where(other => other.DateStart < dateEnd && dateStart < other.DateEnd)
                .SelectMany(other => other.InstructorsId)
                .Intersect(activity.InstructorsId);

            foreach (var instructorId in busyInstructors)
            {
                errorResponse.Add(new ErrorResponse(39, $"El instructor: {instructorId} ya tiene otra actividad en ese horario"));
            }

            if (errorResponse.Count > 0)
            {
                return null;
            }

            // Asignamos el ID autoincremental en la capa de datos
            activity.Id = _nextId++;

            _activities.Add(activity);

            // Devolvemos el objeto completo tal como se ha guardado
            return activity;
        }

        /// <summary>
        /// Convierte la actividad guardada (con IDs) al modelo de salida (con los objetos completos).
        /// </summary>
        public ActivityResponse ToResponse(Activity activity)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                ActivityType = _repositoryActivityType.GetById(activity.ActivityTypeId!.Value),
                Instructors = activity.InstructorsId
                    .Select(id => _repositoryInstructor.GetById(id))
                    .OfType<Instructor>()
                    .ToList(),
                MaxParticipants = activity.MaxParticipants,
                DateStart = activity.DateStart!.Value,
                DateEnd = activity.DateEnd!.Value
            };
        }
    }
}

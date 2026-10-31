using _4VGymAPI.Models;

namespace _4VGymAPI.Repositories
{
    public class InMemoryActivityTypeRepository
    {
        private List<ActivityType> _activityTypes = new();

        private long _nextId = 1; // Para el autonumerico

        public InMemoryActivityTypeRepository()
        {
            SeedData(); // Precargamos algunos datos de prueba
        }

        private void SeedData()
        {
            _activityTypes = new List<ActivityType>()
            {
                new ActivityType (10, "BodyPump",  2 ),
                new ActivityType ( 20, "Spinning",  1 ),
                new ActivityType { Id = 30, Name = "Pilates", NumberMonitors = 1 }
            };
            _activityTypes.Add(new ActivityType(40, "CrossFit", 3));

            // El autonumérico sigue a partir del mayor ID precargado
            _nextId = _activityTypes.Max(at => at.Id) + 1;
        }

        // Método para obtener todos los tipos de actividades
        public IEnumerable<ActivityType> GetAll()
        {
            return _activityTypes;
        }

        // Método para obtener un tipo de actividad por Nombre
        public ActivityType? GetByName(string name)
        {
            return _activityTypes.SingleOrDefault(at =>
                string.Equals(at.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public ActivityType? GetById(long activityTypeId)
        {
            return _activityTypes.SingleOrDefault(at => at.Id == activityTypeId);
        }

        /// <summary>
        /// Valida y guarda un nuevo tipo de actividad generando su ID de forma automática. (Autonumerico)
        /// </summary>
        /// <param name="newActivityType">Datos a guardar (sin ID).</param>
        /// <param name="errorResponse">Error de validación, o null si todo ha ido bien.</param>
        /// <returns>La entidad guardada con su ID autogenerado, o null si hay error.</returns>
        public ActivityType? Add(NewActivityType newActivityType, out ErrorResponse? errorResponse)
        {
            errorResponse = null; // Inicializamos a null para cuando no hay error

            // Validaciones
            if (newActivityType == null)
            {
                errorResponse = new ErrorResponse(3, "Invalid activity type data");
                return null;
            }
            if (string.IsNullOrWhiteSpace(newActivityType.Name))
            {
                errorResponse = new ErrorResponse(4, "The name of the activity is mandatory");
                return null;
            }
            // Si no viene el número de monitores, se da por supuesto que es 1
            int numberMonitors = newActivityType.NumberMonitors ?? 1;
            if (numberMonitors < 1)
            {
                errorResponse = new ErrorResponse(5, "The number of monitors is less than 1");
                return null;
            }
            // No puede haber tipos de actividad con el mismo nombre
            if (GetByName(newActivityType.Name) != null)
            {
                errorResponse = new ErrorResponse(6, "The name is unique, there is already another ActivityType with the same Name");
                return null;
            }

            // Asignamos el ID autoincremental en la capa de datos
            ActivityType activityType = new ActivityType(_nextId++, newActivityType.Name.Trim(), numberMonitors);

            _activityTypes.Add(activityType);

            // Devolvemos el objeto completo tal como se ha guardado
            return activityType;
        }
    }
}

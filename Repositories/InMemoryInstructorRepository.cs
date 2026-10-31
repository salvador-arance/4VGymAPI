using _4VGymAPI.Models;
using System.Text.RegularExpressions;

namespace _4VGymAPI.Repositories
{
    public class InMemoryInstructorRepository
    {
        // Clave: ID del monitor, Valor: el monitor
        private Dictionary<long, Instructor> _instructors = new();

        private long _nextId = 1; // Para el autonumerico

        public InMemoryInstructorRepository()
        {
            SeedData(); // Precargamos algunos datos de prueba
        }

        private void SeedData()
        {
            _instructors = new Dictionary<long, Instructor>
            {
                [1] = new Instructor(1, "Ana García", "ana@example.com", "https://example.com/ana.jpg", "+34600111222"),
                [2] = new Instructor(2, "Luis Pérez", "luis@example.com", "https://example.com/luis.jpg", "+34600333444"),
                [3] = new Instructor(3, "Marta López", "marta@example.com", "https://example.com/marta.jpg", "+34600555666")
            };
            _instructors.Add(4, new Instructor(4, "Miguel Ruiz", "miguel@example.com", "https://example.com/miguel.jpg", "+34600777888"));

            _nextId = _instructors.Keys.Max() + 1;
        }

        // Método para obtener todos los instructores, filtrados opcionalmente por la inicial del nombre
        public IEnumerable<Instructor> GetByFirstInitial(string? firstInitial = null)
        {
            IEnumerable<Instructor> _allInstructors = _instructors.Values;

            // Si se recibe la inicial, aplicamos el filtro sobre la colección
            if (!string.IsNullOrWhiteSpace(firstInitial))
            {
                IEnumerable<Instructor> filteredInstructors = _allInstructors
                    .Where(instructor =>
                        !string.IsNullOrEmpty(instructor.Name) &&
                        instructor.Name.StartsWith(firstInitial, StringComparison.OrdinalIgnoreCase)
                    );
                return filteredInstructors;
            }

            return _allInstructors;
        }

        // Búsqueda por la clave del diccionario
        public Instructor? GetById(long id)
        {
            _instructors.TryGetValue(id, out Instructor? instructor);
            return instructor;
        }

        // Búsqueda por email y nombre, sin distinguir mayúsculas
        public Instructor? GetByEmailAndName(string email, string name)
        {
            return _instructors.Values.FirstOrDefault(i =>
                string.Equals(i.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(i.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Valida y guarda un nuevo monitor generando su ID de forma automática. (Autonumerico)
        /// </summary>
        /// <param name="newInstructor">Datos del monitor a guardar (sin ID).</param>
        /// <param name="errorResponse">Error de validación, o null si todo ha ido bien.</param>
        /// <returns>El monitor guardado con su ID autogenerado, o null si hay error.</returns>
        public Instructor? Add(NewInstructor newInstructor, out ErrorResponse? errorResponse)
        {
            errorResponse = null; // Inicializamos a null para cuando no hay error

            // Validaciones
            if (newInstructor == null)
            {
                errorResponse = new ErrorResponse(10, "Invalid instructor data");
                return null;
            }
            if (string.IsNullOrWhiteSpace(newInstructor.Name))
            {
                errorResponse = new ErrorResponse(11, "The name of the instructor is mandatory");
                return null;
            }
            if (string.IsNullOrWhiteSpace(newInstructor.Email))
            {
                errorResponse = new ErrorResponse(12, "The email of the instructor is mandatory");
                return null;
            }
            if (!Regex.IsMatch(newInstructor.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                errorResponse = new ErrorResponse(13, "Invalid format on email");
                return null;
            }
            if (string.IsNullOrWhiteSpace(newInstructor.Phone))
            {
                errorResponse = new ErrorResponse(14, "The phone of the instructor is mandatory");
                return null;
            }
            if (!Regex.IsMatch(newInstructor.Phone, @"^\+?\d{9,12}$"))
            {
                errorResponse = new ErrorResponse(15, "Invalid format on phone");
                return null;
            }
            // La foto es una URL
            if (!Uri.TryCreate(newInstructor.PhotoUrl, UriKind.Absolute, out Uri? photoUri) ||
                (photoUri.Scheme != Uri.UriSchemeHttp && photoUri.Scheme != Uri.UriSchemeHttps))
            {
                errorResponse = new ErrorResponse(16, "The photo must be a valid URL");
                return null;
            }
            // No puede haber un monitor con el mismo Email y Nombre
            if (GetByEmailAndName(newInstructor.Email, newInstructor.Name) != null)
            {
                errorResponse = new ErrorResponse(17, "There is already an instructor with the same email and name");
                return null;
            }

            Instructor instructor = new Instructor(
                _nextId++,
                newInstructor.Name.Trim(),
                newInstructor.Email.Trim(),
                newInstructor.PhotoUrl!,
                newInstructor.Phone);

            _instructors.Add(instructor.Id, instructor);

            // Devolvemos el objeto completo tal como se ha guardado
            return instructor;
        }
    }
}

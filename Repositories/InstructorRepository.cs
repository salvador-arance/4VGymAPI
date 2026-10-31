using _4VGymAPI.Data;
using _4VGymAPI.Dtos;
using _4VGymAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace _4VGymAPI.Repositories
{
    public class InstructorRepository 
    {
        private readonly GymDbContext _context;
        public InstructorRepository(GymDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los monitores, filtrados opcionalmente por la inicial del nombre
        public async Task<List<Instructor>> GetByFirstInitial(string? firstInitial = null)
        {
            // Si no hay filtro devolvemos todos
            if (string.IsNullOrWhiteSpace(firstInitial))
            {
                return await _context.Instructors.ToListAsync();
            }

            // Si hay filtro, el Where se traduce a SQL: WHERE Name LIKE 'M%'
            return await _context.Instructors
                .Where(instructor => instructor.Name.StartsWith(firstInitial))
                .ToListAsync();
        }
        // Búsqueda por la clave primaria
        public async Task<Instructor?> GetById(int id)
        {
            return await _context.Instructors.FindAsync(id);
        }

        // Búsqueda por email y nombre, sin distinguir mayúsculas
        public async Task<Instructor?> GetByEmailAndName(string email, string name)
        {
            string emailToFind = email.Trim().ToUpper();
            string nameToFind = name.Trim().ToUpper();
            return await _context.Instructors.SingleOrDefaultAsync(i =>
                i.Email.ToUpper() == emailToFind
                && i.Name.ToUpper() == nameToFind);
        }

        /// <summary>
        /// Valida y guarda un nuevo monitor. El ID lo genera la BBDD (autonumérico).
        /// </summary>
        /// <param name="newInstructor">Datos del monitor a guardar (sin ID).</param>
        /// <returns>Tupla con el monitor guardado (o null) y el error de validación (o null).</returns>
        public async Task<(Instructor? instructor, ErrorResponse? error)> Add(NewInstructor? newInstructor)
        {
            if (newInstructor == null)
            {
                return (null, new ErrorResponse(10, "Invalid instructor data"));
            }

            if (string.IsNullOrWhiteSpace(newInstructor.Name))
            {
                return (null, new ErrorResponse(11, "The name of the instructor is mandatory"));
            }
            
            if (string.IsNullOrWhiteSpace(newInstructor.Email))
            {
                return (null, new ErrorResponse(12, "The email of the instructor is mandatory"));
            }
            
            if (!Regex.IsMatch(newInstructor.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return (null, new ErrorResponse(13, "Invalid format on email"));
            }
            
            if (string.IsNullOrWhiteSpace(newInstructor.Phone))
            {
                return (null, new ErrorResponse(14, "The phone of the instructor is mandatory"));
            }
            
            if (!Regex.IsMatch(newInstructor.Phone, @"^\+?\d{9,12}$"))
            {
                return (null, new ErrorResponse(15, "Invalid format on phone"));
            }
            
            if (string.IsNullOrWhiteSpace(newInstructor.PhotoUrl) ||
                !Regex.IsMatch(newInstructor.PhotoUrl, @"^https?://[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(:\d+)?(/\S*)?$"))
            {
                return (null, new ErrorResponse(16, "The photo must be a valid URL"));
            }
            // No puede haber un monitor con el mismo Email y Nombre
            
            if (await GetByEmailAndName(newInstructor.Email, newInstructor.Name) != null)
            {
                return (null, new ErrorResponse(17, "There is already an instructor with the same email and name"));
            }

            Instructor instructor = new Instructor
            {
                Name = newInstructor.Name.Trim(),
                Email = newInstructor.Email.Trim(),
                PhotoUrl = newInstructor.PhotoUrl!,
                Phone = newInstructor.Phone
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return (instructor, null);
        }
    }
}
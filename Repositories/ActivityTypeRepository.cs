using _4VGymAPI.Data;
using _4VGymAPI.Dtos;
using _4VGymAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;

namespace _4VGymAPI.Repositories
{
    public class ActivityTypeRepository
    {
        private readonly GymDbContext _context;
        public ActivityTypeRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityType>> GetAll()
        {
            return await _context.ActivityTypes.ToListAsync();
        }

        public async Task<ActivityType?> GetById(int id)
        {
            return await _context.ActivityTypes.FindAsync(id);
        }

        public async Task<ActivityType?> GetByName(string name) 
        {
            string nameToFind = name.ToUpper();
            return await _context.ActivityTypes
                .SingleOrDefaultAsync(at => at.Name.ToUpper() == nameToFind);
        }

        /// <summary>
        /// Valida y guarda un nuevo tipo de actividad. El ID lo genera la BBDD (autonumérico).
        /// </summary>
        /// <param name="newActivityType">Datos a guardar (sin ID).</param>
        /// <returns>Tupla con la entidad guardada (o null) y el error de validación (o null).</returns>
        public async Task<(ActivityType? activityType, ErrorResponse? error)> Add(NewActivityType? newActivityType)
        {
            if (newActivityType == null) 
            {
                return (null, new ErrorResponse(3, "Invalid activity type data"));
            }

            if (string.IsNullOrWhiteSpace(newActivityType.Name))
            {
                return (null, new ErrorResponse(4, "The name of the activity is mandatory"));
            }

            int numberMonitors = newActivityType.NumberMonitors ?? 1;
            if (numberMonitors < 1)
            {
                return (null, new ErrorResponse(5, "The number of monitors is less than 1"));
            }
            if (await GetByName(newActivityType.Name) != null)
            {
                return (null, new ErrorResponse(6, "The name is unique, there is already another ActivityType with the same Name"));
            }
            ActivityType activityType = new ActivityType
            {
                Name = newActivityType.Name.Trim(),
                NumberMonitors = numberMonitors
            };

            _context.ActivityTypes.Add(activityType);
            await _context.SaveChangesAsync();

            return (activityType, null);
        }
    }
}
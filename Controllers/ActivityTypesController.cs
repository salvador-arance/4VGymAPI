using _4VGymAPI.Models;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPI.Controllers
{
    [ApiController]
    [Route("activity-types")]
    public class ActivityTypesController : ControllerBase
    {
        private readonly InMemoryActivityTypeRepository _repository;

        // Inyectamos la clase concreta del repositorio
        public ActivityTypesController(InMemoryActivityTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /activity-types
        /// Devuelve el listado de tipos de actividades de 4VGym.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ActivityType>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ActivityType>> FindActivityTypes()
        {
            try
            {
                IEnumerable<ActivityType> activityTypes = _repository.GetAll();

                if (activityTypes == null || activityTypes.Count() == 0)
                {
                    return BadRequest(new ErrorResponse(2, "No activity types found"));
                }

                return Ok(activityTypes);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse(1, "Any problem in the Server"));
            }
        }

        /// <summary>
        /// POST /activity-types
        /// Añade un nuevo tipo de actividad en 4VGym.
        /// </summary>
        /// <param name="newActivityType">Objeto NewActivityType enviado en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(ActivityType), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<ActivityType> CreateActivityType([FromBody] NewActivityType newActivityType)
        {
            try
            {
                // No hacemos validaciones de Controlador, todas son de repositorio
                ActivityType? createdActivityType = _repository.Add(newActivityType, out ErrorResponse? errorResponse);

                // Primero controlo el error
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }

                // Retornar respuesta 201 Created con el objeto creado
                return StatusCode(StatusCodes.Status201Created, createdActivityType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}

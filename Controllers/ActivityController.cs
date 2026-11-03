using _4VGymAPI.Dtos;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPI.Controllers
{
    [ApiController]
    [Route("activities")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepository _repository;

        // Inyectamos la clase concreta del repositorio
        public ActivityController(ActivityRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST /activities
        /// Añade una nueva actividad en 4VGym.
        /// </summary>
        /// <param name="activity">Objeto ActivityCreateDto enviado en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ErrorResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ActivityResponse>> CreateActivity([FromBody] ActivityCreateDto activity)
        {
            try
            {
                var (activityCreated, errors) = await _repository.Add(activity);

                // En caso de errores de validacion del repositorio
                if (activityCreated == null && errors.Count > 0)
                {
                    return BadRequest(errors);
                }

                // En caso de OK devolvemos el modelo de salida con toda la información
                return StatusCode(StatusCodes.Status201Created, activityCreated);
            }
            catch (Exception)
            {
                return tatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}
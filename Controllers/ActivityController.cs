using _4VGymAPI.Models;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPI.Controllers
{
    [ApiController]
    [Route("activities")]
    public class ActivityController : ControllerBase
    {
        private readonly InMemoryActivityRepository _repository;

        // Inyectamos la clase concreta del repositorio
        public ActivityController(InMemoryActivityRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// POST /activities
        /// Añade una nueva actividad en 4VGym.
        /// </summary>
        /// <param name="activity">Objeto Activity enviado en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ErrorResponse>), StatusCodes.Status400BadRequest)]
        public ActionResult<ActivityResponse> CreateActivity([FromBody] Activity activity)
        {
            try
            {
                Activity? activityCreated = _repository.Add(activity, out List<ErrorResponse> errors);

                // En caso de errores de validacion del repositorio
                if (activityCreated == null && errors.Count > 0)
                {
                    return BadRequest(errors);
                }

                // En caso de OK devolvemos el modelo de salida con toda la información
                return StatusCode(StatusCodes.Status201Created, _repository.ToResponse(activityCreated!));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}

using _4VGymAPI.Dtos;
using _4VGymAPI.Models;
using _4VGymAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _4VGymAPI.Controllers
{
    [ApiController]
    [Route("instructors")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorRepository _repository;

        // Inyectamos la clase concreta del repositorio
        public InstructorController(InstructorRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /instructors
        /// GET /instructors?first_initial=M
        /// Devuelve el listado de monitores (filtrado opcionalmente por la inicial del nombre)
        /// </summary>
        /// <param name="firstInitial">Filtro opcional para la inicial del monitor</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Instructor>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Instructor>>> FindInstructors([FromQuery(Name = "first_initial")] string? firstInitial)
        {
            try
            {
                List<Instructor> instructors = await _repository.GetByFirstInitial(firstInitial);
                return Ok(instructors);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse(1, "Any problem in the Server"));
            }
        }

        /// <summary>
        /// GET /instructors/{id}
        /// Devuelve todos los datos de un monitor dado su identificador.
        /// </summary>
        /// <param name="id">Identificador del monitor</param>
        [HttpGet("{id}")] // El id forma parte de la ruta: /instructors/3
        [ProducesResponseType(typeof(Instructor), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Instructor>> FindInstructorById(int id)
        {
            try
            {
                Instructor? instructor = await _repository.GetById(id);

                if (instructor == null)
                {
                    return BadRequest(new ErrorResponse(2, $"No instructor found with id {id}"));
                }

                return Ok(instructor);
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse(1, "Any problem in the Server"));
            }
        }

        /// <summary>
        /// POST /instructors
        /// Añade un nuevo monitor en 4VGym.
        /// </summary>
        /// <param name="newInstructor">Objeto NewInstructor enviado en el cuerpo de la petición (JSON).</param>
        [HttpPost]
        [ProducesResponseType(typeof(Instructor), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Instructor>> CreateInstructor([FromBody] NewInstructor newInstructor)
        {
            try
            {
                // No hacemos validaciones de Controlador, todas son de repositorio
                var (createdInstructor, errorResponse) = await _repository.Add(newInstructor);

                // Primero controlo el error
                if (errorResponse != null)
                {
                    return BadRequest(errorResponse);
                }

                return StatusCode(StatusCodes.Status201Created, createdInstructor);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new ErrorResponse(1, "An internal server error occurred"));
            }
        }
    }
}
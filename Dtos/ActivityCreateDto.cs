using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using _4VGymAPI.Validations;

namespace _4VGymAPI.Dtos
{
    /// <summary>
    /// Modelo de entrada para crear y modificar actividades. Las referencias a otros modelos van por ID.
    /// </summary>
    public class ActivityCreateDto
    {
        // No hay ID porque lo genera la BBDD

        [JsonPropertyName("activity_type_id")]
        [Required(ErrorMessage = "30|El tipo de actividad es obligatorio.")]
        public int? ActivityTypeId { get; set; }

        [JsonPropertyName("instructors_id")]
        [Required(ErrorMessage = "26|La lista de monitores no puede ser nula.")]
        [MinLength(1, ErrorMessage = "27|La actividad debe tener asignado al menos un monitor.")]
        public List<int> InstructorsId { get; set; } = new();

        [JsonPropertyName("max_participants")]
        [Range(1, 25, ErrorMessage = "25|El aforo máximo debe estar entre 1 y 25 participantes.")]
        public int MaxParticipants { get; set; }

        [JsonPropertyName("date_start")]
        [Required(ErrorMessage = "28|La fecha de inicio es obligatoria.")]
        [FutureDate(ErrorMessage = "37|Las fechas tienen que ser en el futuro")]
        public DateTime? DateStart { get; set; }

        [JsonPropertyName("date_end")]
        [Required(ErrorMessage = "29|La fecha de fin es obligatoria.")]
        [DateGreaterThan(nameof(DateStart), ErrorMessage = "36|La fecha de fin tiene que ser posterior a la fecha de inicio")]
        public DateTime? DateEnd { get; set; }
    }
}
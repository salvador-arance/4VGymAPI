using System.ComponentModel.DataAnnotations;

namespace _4VGymAPI.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Si el valor es nulo, dejamos que la responsabilidad de requerirlo sea del atributo [Required]
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is DateTime fechaIngresada)
            {
                if (fechaIngresada > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                string mensajeError = ErrorMessage ?? "La fecha debe ser posterior al momento actual.";
                return new ValidationResult(mensajeError);
            }

            return new ValidationResult("El formato de fecha no es válido.");
        }
    }
}
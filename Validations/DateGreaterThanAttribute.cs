using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace _4VGymAPI.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        // Recibe el nombre de la propiedad con la que se compara
        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            // 1. Buscar la propiedad por su nombre usando reflexión
            PropertyInfo? propertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (propertyInfo is null)
            {
                return new ValidationResult($"La propiedad con el nombre '{_comparisonProperty}' no existe.");
            }

            // 2. Leer el valor de esa propiedad en el objeto que se está validando
            object? comparisonValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            // 3. Comparar las dos fechas
            if (value is DateTime fechaFin && comparisonValue is DateTime fechaInicio)
            {
                if (fechaFin > fechaInicio)
                {
                    return ValidationResult.Success;
                }
                string mensaje = ErrorMessage ?? $"La fecha debe ser posterior a {_comparisonProperty}.";
                return new ValidationResult(mensaje, new[] { validationContext.MemberName! });
            }

            return ValidationResult.Success;
        }
    }
}
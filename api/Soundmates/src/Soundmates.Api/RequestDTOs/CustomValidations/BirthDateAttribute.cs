using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.CustomValidations;

[AttributeUsage(AttributeTargets.Property)]
public class BirthDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly birthdate)
        {
            DateOnly minDate = new(1900, 1, 1);
            DateOnly maxDate = DateOnly.FromDateTime(DateTime.UtcNow);

            if (birthdate >= minDate && birthdate <= maxDate)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Birth date must be between {minDate:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}.");
        }
        return new ValidationResult("Invalid birth date.");
    }
}
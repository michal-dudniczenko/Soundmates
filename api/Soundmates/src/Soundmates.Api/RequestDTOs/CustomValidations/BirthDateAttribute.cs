using System.ComponentModel.DataAnnotations;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Api.RequestDTOs.CustomValidations;

[AttributeUsage(AttributeTargets.Property)]
public class BirthDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly birthdate)
        {
            DateOnly minDate = MinUserBirthDate;
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
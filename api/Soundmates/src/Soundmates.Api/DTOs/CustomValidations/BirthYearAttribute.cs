using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.CustomValidations;

[AttributeUsage(AttributeTargets.Property)]
public class BirthYearAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int year)
        {
            int currentYear = DateTime.UtcNow.Year;
            if (year >= 1900 && year <= currentYear)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Birth year must be between 1900 and {currentYear}.");
        }
        return new ValidationResult("Invalid birth year.");
    }
}
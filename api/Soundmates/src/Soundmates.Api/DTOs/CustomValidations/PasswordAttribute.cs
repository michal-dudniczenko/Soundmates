using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.CustomValidations;

[AttributeUsage(AttributeTargets.Property)]
public class PasswordAttribute : ValidationAttribute
{
    private const int minPasswordLength = 8;
    private const int maxPasswordLength = 32;
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string password)
        {
            if (password.Length < minPasswordLength)
            {
                return new ValidationResult($"Password is too short. Minimal password length is {minPasswordLength}.");
            }

            if (password.Length > maxPasswordLength)
            {
                return new ValidationResult($"Password is too long. Maximum password length is {maxPasswordLength}.");
            }

            bool hasLower = false, hasUpper = false, hasDigit = false, hasSpecial = false;

            foreach (char ch in password)
            {
                if (ch < 33 || ch > 126)
                {
                    return new ValidationResult($"Incorrect password. Character: {ch} is not allowed.");
                }

                if (!hasLower && Char.IsLower(ch))
                {
                    hasLower = true;
                }

                if (!hasUpper && Char.IsUpper(ch))
                {
                    hasUpper = true;
                }

                if (!hasDigit && Char.IsDigit(ch))
                {
                    hasDigit = true;
                }

                if (!hasSpecial &&
                    ((ch >= 33 && ch <= 47) || (ch >= 58 && ch <= 64) || (ch >= 91 && ch <= 96) || (ch >= 123 && ch <= 126)))
                {
                    hasSpecial = true;
                }
            }

            if (!hasLower)
            {
                return new ValidationResult("Password must have at least one lowercase letter.");
            }

            if (!hasUpper)
            {
                return new ValidationResult("Password must have at least one uppercase letter.");
            }

            if (!hasDigit)
            {
                return new ValidationResult("Password must have at least one digit.");
            }

            if (!hasSpecial)
            {
                return new ValidationResult("Password must have at least one special character.");
            }

            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid password");
    }
}
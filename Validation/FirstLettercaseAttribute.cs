using System.ComponentModel.DataAnnotations;

namespace MyDotNet9Api.Validation;

public class FirstLettercaseAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }
        string firstLetter = value.ToString()![0].ToString();
        if (firstLetter != firstLetter.ToUpper())
        {
            return new ValidationResult("The first letter must be a uppercase."); 
        }

        return ValidationResult.Success;
    }
    

}
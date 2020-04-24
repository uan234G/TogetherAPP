using System;
using System.ComponentModel.DataAnnotations;

namespace finalGSA.Models
{
    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value < DateTime.Now)
            {
            return new ValidationResult("Can't be a past date");
            }
            return ValidationResult.Success;
        }
    }
}
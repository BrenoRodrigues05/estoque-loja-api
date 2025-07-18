using System.ComponentModel.DataAnnotations;

namespace APILojaEstoque.Validations
{
    public class PrimeiraLetraMaiúsculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var PrimeiraLetraMaiúscula = value.ToString()[0].ToString();
                if (PrimeiraLetraMaiúscula != PrimeiraLetraMaiúscula.ToUpper())
                {
                    return new ValidationResult("A primeira letra precisa ser maiúscula");
                }
                return ValidationResult.Success;
            
        }
    }
}

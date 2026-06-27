using System;
using System.ComponentModel.DataAnnotations;

public class MayorDeEdadAttribute : ValidationAttribute
{
    // Cambiamos 'object value' a 'object? value' (nullable)
    // Cambiamos el retorno a 'ValidationResult?'
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;

            if (edad >= 18)
                return ValidationResult.Success;

            return new ValidationResult("Debes ser mayor de 18 años para registrarte.");
        }

        // devuelva error si la fecha no es válida
        return new ValidationResult("Fecha inválida.");
    }
}
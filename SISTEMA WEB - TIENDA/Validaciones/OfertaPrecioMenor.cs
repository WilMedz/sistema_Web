using System;
using System.ComponentModel.DataAnnotations;

public class OfertaPrecioMenor : ValidationAttribute
{
    private readonly string _propiedadComparar;

    public OfertaPrecioMenor (string propiedadComparar)
    {
        _propiedadComparar = propiedadComparar;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var propiedadInfo = validationContext.ObjectType.GetProperty(_propiedadComparar);
        if (propiedadInfo == null)
            return new ValidationResult($"No se encontró la propiedad {_propiedadComparar}.");

        var valorComparar = propiedadInfo.GetValue(validationContext.ObjectInstance);

        if (value is decimal oferta && valorComparar is decimal lista && oferta >= lista)
            return new ValidationResult(ErrorMessage ?? "El precio de oferta debe ser menor al precio de lista.");

        return ValidationResult.Success;
    }
}

using System.ComponentModel.DataAnnotations;

namespace SISTEMA_WEB___TIENDA.ViewModels;

public class LoginVM
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio para iniciar sesión.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [MaxLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
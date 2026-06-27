using System;
using System.ComponentModel.DataAnnotations;

namespace SISTEMA_WEB___TIENDA.ViewModels
{
    public class RegistroVM
    {
        [Required(ErrorMessage = "Por favor, ingresa tu nombre completo.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un formato de correo válido.")]
        [MaxLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Debes confirmar tu contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; }

        [Required(ErrorMessage = "Por favor, selecciona tu ciudad de residencia.")]
        public int CiudadId { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
    }
}

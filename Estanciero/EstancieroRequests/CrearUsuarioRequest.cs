    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroRequests
{
    public class CrearUsuarioRequest
    {
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El DNI debe ser un valor válido mayor que 0.")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        // El atributo EmailAddress valida que el formato del email sea correcto
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [StringLength(120, ErrorMessage = "El email no puede tener más de 120 caracteres.")]
        public string Email { get; set; }
    }
}

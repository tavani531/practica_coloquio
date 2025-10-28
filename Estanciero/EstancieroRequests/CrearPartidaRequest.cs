using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroRequests
{
    public class CrearPartidaRequest
    {
        [Required(ErrorMessage = "Debe haber al menos dos jugadores.")]
        //Se asegura que haya entre 2 y 4 jugadores
        [MinLength(2, ErrorMessage = "La partida debe tener al menos 2 jugadores.")]
        [MaxLength(4, ErrorMessage = "La partida no puede tener más de 4 jugadores.")]
        public List<int> JugadoresDNIs { get; set; }

       
        
    }
}

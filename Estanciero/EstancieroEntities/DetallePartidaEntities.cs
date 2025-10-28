using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class DetallePartidaEntities
    {
        public int NumeroPartida { get; set; }
        public int DNIUsuario { get; set; }
        public int PosicionActual { get; set; } 
        public double DineroDisponible { get; set; }
        public List<Movimiento> HistorialMovimientos { get; set; }
        public Enums.EstadoDetalle Estado { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EstancieroEntities.Enums;

namespace EstancieroEntities
{
    public class Movimiento
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TipoMovimiento Tipo { get; set; }
        public string Descripcion { get; set; }
        public double Monto { get; set; }
        public int CasilleroOrigen { get; set; }
        public int CasilleroDestino { get; set; }
        public int? DniJugadorAfectado { get; set; }
    }
}

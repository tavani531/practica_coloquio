using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class Enums
    {
        public enum EstadoPartida
        {
            EnJuego,
            Finalizada,
            Suspendida,
            Pausada
        }
        public enum TipoMovimiento
        {
            MovimientoDado,
            CompraProvincia,
            PagoAlquiler,
            Multa,
            Premio
        }
        public enum EstadoDetalle
        {
            EnJuego,
            Derrotado
        }
        public enum TipoCasillero
        {
            Inicio,
            Provincia,
            Multa,
            Premio
        }
        public enum MotivoVictoria
        {
            DoceProvincias,
            UnicoConSaldoPositivo,
            MayorSaldoAlFinal
        }
    }
}

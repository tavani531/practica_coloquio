using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class PartidasEntities
    {
        public int NumeroPartida { get; set; }
        public DateTime FechaInicio { get; set; }
        //Los que tienen el ? pueden ser nulos en caso de que el casillero no los necesite
        public DateTime? FechaFin { get; set; }
        public TimeSpan? Duracion { get; set; }
        public Enums.EstadoPartida Estado { get; set; } 
        public TurnosPartidasEntities TurnoActual { get; set; }
        public List<TurnosPartidasEntities> ConfiguracionTurnos { get; set; }
        public List<UsuariosEntities> Jugadores { get; set; }
        public List<CasilleroTablero> Tablero { get; set; }
        public List<DetallePartidaEntities> DetalleJugadores { get; set; }
        public int? DNIGanador { get; set; }
        public Enums.MotivoVictoria? MotivoVictoria { get; set; }

    }
}

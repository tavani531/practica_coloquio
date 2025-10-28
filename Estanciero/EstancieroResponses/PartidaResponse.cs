using EstancieroEntities;
namespace EstancieroResponses
{
    public class PartidaResponse
    {
        public int NumeroPartida { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Estado { get; set; }
        // Lista de jugadores en la partida
        public List<JugadorResponse> Jugadores { get; set; } = new List<JugadorResponse>();
        public TurnoResponse TurnoActual { get; set; }
        public List<CasilleroTablero> Tablero { get; set; }

    }
}

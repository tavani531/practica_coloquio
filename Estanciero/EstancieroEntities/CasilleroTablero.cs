using System.Collections.Generic;
using static EstancieroEntities.Enums;

namespace EstancieroEntities
{
    public class CasilleroTablero
    {
        public int NroCasillero { get; set; }
        public string Nombre { get; set; }
        public TipoCasillero Tipo { get; set; }
        //Los que tienen el ? pueden ser nulos en caso de que el casillero no los necesite
        public double? PrecioCompra { get; set; } 
        public double? PrecioAlquiler { get; set; }

        public int? DNIPropietario { get; set; }
        public double? Monto { get; set; }
    }

    //Clase para cargar la configuracion del tablero desde un archivo JSON
    public class TableroConfig
    {
        public List<CasilleroTablero> Tablero { get; set; }
    }
}

using EstancieroEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization; 
using Newtonsoft.Json.Converters;  
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EstancieroData
{
    public static class Archivos
    {
        //método auxiliar que define un conjunto de reglas de configuración para
        //serializar (convertir objetos de C# a JSON) y deserializar (convertir JSON a objetos de C#).
        private static JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),

                Converters = new List<JsonConverter> { new StringEnumConverter() },

                Formatting = Formatting.Indented 
            };
        }

        //RUTAS DE ARCHIVOS
        private static string ObtenerRutaPartidas()
        {
            string directorio = "../EstancieroData/Database";
            Directory.CreateDirectory(directorio);
            return Path.Combine(directorio, "partidas.json");
        }

        private static string ObtenerRutaUsuarios()
        {
            string directorio = "../EstancieroData/Database";
            Directory.CreateDirectory(directorio);
            return Path.Combine(directorio, "jugadores.json");
        }

        private static string ObtenerRutaTablero()
        {
            string directorio = "../EstancieroData/Database";
            Directory.CreateDirectory(directorio);
            return Path.Combine(directorio, "tablero.json");
        }

        private static string ObtenerRutaContador()
        {
            string directorio = "../EstancieroData/Database";
            Directory.CreateDirectory(directorio);
            return Path.Combine(directorio, "contador.txt");
        }

        //Esta función lee el número de partida actual (idActual) desde un archivo de texto,
        //guarda el siguiente número (proximoId) en ese mismo archivo para la próxima vez,
        //y devuelve el número actual que acaba de leer (no el siguiente).
        public static int ObtenerSiguienteNumeroPartida()
        {
            int idActual = 0;
            string rutaContador = ObtenerRutaContador();
            if (File.Exists(rutaContador))
            {
                string contenido = File.ReadAllText(rutaContador);
                int.TryParse(contenido, out idActual);
            }

            int proximoId = idActual + 1;
            File.WriteAllText(rutaContador, proximoId.ToString());

            return idActual;
        }

        //Este método lee un archivo JSON que contiene una lista de usuarios,
        //lo convierte (deserializa) en una lista de objetos C# (List<UsuariosEntities>),
        //y devuelve una lista vacía si el archivo no existe o está vacío.
        public static List<UsuariosEntities> LeerUsuarios()
        {
            string ruta = ObtenerRutaUsuarios();
            if (!File.Exists(ruta))
            {
                return new List<UsuariosEntities>();
            }

            string json = File.ReadAllText(ruta);

            return JsonConvert.DeserializeObject<List<UsuariosEntities>>(json, GetJsonSettings()) ?? new List<UsuariosEntities>();
        }

        //Lee un usuario específico de la lista de usuarios basado en su DNI.
        public static UsuariosEntities? LeerUsuario(int dni)
        {
            var usuarios = LeerUsuarios();
            return usuarios.FirstOrDefault(u => u.DNI == dni);
        }

        //Guarda una lista completa de usuarios en un archivo JSON.
        public static void GuardarUsuarios(List<UsuariosEntities> usuarios)
        {
            string json = JsonConvert.SerializeObject(usuarios, GetJsonSettings());
            File.WriteAllText(ObtenerRutaUsuarios(), json);
        }

        //Guarda o actualiza un usuario específico en la lista de usuarios.
        public static void GuardarUsuario(UsuariosEntities usuario)
        {
            var usuarios = LeerUsuarios();
            var index = usuarios.FindIndex(u => u.DNI == usuario.DNI);

            if (index >= 0)
            {
                usuarios[index] = usuario;
            }
            else
            {
                usuarios.Add(usuario);
            }

            GuardarUsuarios(usuarios);
        }

        //Elimina un usuario específico de la lista de usuarios basado en su DNI.
        public static void EliminarUsuario(int dni)
        {
            var usuarios = LeerUsuarios();
            usuarios.RemoveAll(u => u.DNI == dni);
            GuardarUsuarios(usuarios);
        }

        //Guarda o actualiza una partida específica en la lista de partidas.
        public static void GuardarPartida(PartidasEntities partida)
        {
            var partidas = LeerTodasLasPartidas();
            var index = partidas.FindIndex(p => p.NumeroPartida == partida.NumeroPartida);

            if (index >= 0)
            {
                partidas[index] = partida;
            }
            else
            {
                partidas.Add(partida);
            }

            string json = JsonConvert.SerializeObject(partidas, GetJsonSettings());
            File.WriteAllText(ObtenerRutaPartidas(), json);
        }

        //Lee una partida específica de la lista de partidas basado en su número de partida.
        public static PartidasEntities LeerPartida(int numeroPartida)
        {
            var partidas = LeerTodasLasPartidas();
            return partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
        }

        //Lee todas las partidas desde un archivo JSON y las convierte en una lista de objetos C#.
        public static List<PartidasEntities> LeerTodasLasPartidas()
        {
            string ruta = ObtenerRutaPartidas();
            if (!File.Exists(ruta))
            {
                return new List<PartidasEntities>();
            }

            string json = File.ReadAllText(ruta);

            
            return JsonConvert.DeserializeObject<List<PartidasEntities>>(json, GetJsonSettings()) ?? new List<PartidasEntities>();
        }

        //Carga la configuración del tablero desde un archivo JSON y devuelve una lista de casilleros.
        public static List<CasilleroTablero> CargarTableroTemplate()
        {
            string ruta = ObtenerRutaTablero();

            if (!File.Exists(ruta))
            {
                throw new FileNotFoundException("No se encontró el archivo de configuración del tablero (tablero.json).", ruta);
            }

            string json = File.ReadAllText(ruta);

            
            var config = JsonConvert.DeserializeObject<TableroConfig>(json, GetJsonSettings());

            return config?.Tablero ?? new List<CasilleroTablero>();
        }
    }
}
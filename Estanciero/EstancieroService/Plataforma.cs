using EstancieroData;
using EstancieroEntities;
using EstancieroRequests;
using EstancieroResponses;
using Newtonsoft.Json;
using System.Numerics;
namespace EstancieroService
{
    public class Plataforma
    {
        public List<UsuariosEntities> ObtenerTodosLosUsuarios()
        {
            return Archivos.LeerUsuarios();
        }
        public ApiResponse<UsuariosEntities> CrearUsuario(CrearUsuarioRequest request)
        {
            var existente = Archivos.LeerUsuario(request.DNI);
            if (existente != null)
            {
                return new ApiResponse<UsuariosEntities>
                {
                    Success = false,
                    Message = "El usuario ya existe",
                    Errors = new List<string> { $"Usuario con DNI {request.DNI} ya registrado" }
                };
            }

            var usuario = new UsuariosEntities
            {
                DNI = request.DNI,
                Nombre = request.Nombre,
                Email = request.Email,
                Estadisticas = new EstadisticasJugador()
            };

            Archivos.GuardarUsuario(usuario);

            return new ApiResponse<UsuariosEntities>
            {
                Success = true,
                Message = "Usuario creado correctamente",
                Data = usuario
            };
        }
        public ApiResponse<UsuariosEntities> ObtenerUsuario(int dni)
        {
            var usuario = Archivos.LeerUsuario(dni);

            if (usuario == null)
            {
                return new ApiResponse<UsuariosEntities>
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    Errors = new List<string> { $"No se encontró un usuario con DNI {dni}" }
                };
            }

            return new ApiResponse<UsuariosEntities>
            {
                Success = true,
                Message = "Usuario obtenido exitosamente",
                Data = usuario
            };
        }

        public ApiResponse<string> EliminarUsuario(int dni)
        {
            var usuario = Archivos.LeerUsuario(dni);
            if (usuario == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    Errors = new List<string> { $"El usuario con DNI {dni} no existe." }
                };
            }

            var todasLasPartidas = Archivos.LeerTodasLasPartidas();

            bool estaEnPartidaActiva = todasLasPartidas.Any(p =>
                (p.Estado == Enums.EstadoPartida.EnJuego || p.Estado == Enums.EstadoPartida.Pausada) &&
                p.DetalleJugadores.Any(j => j.DNIUsuario == dni)
            );

            if (estaEnPartidaActiva)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "No se puede eliminar al usuario",
                    Errors = new List<string> { "El usuario se encuentra actualmente en una partida activa (En Juego o Pausada)." }
                };
            }

            Archivos.EliminarUsuario(dni);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Usuario eliminado correctamente"
            };
        }

        public ApiResponse<PartidaResponse> CrearPartida(CrearPartidaRequest request)
        {
            if (request.JugadoresDNIs.Count < 2 || request.JugadoresDNIs.Count > 4)
            {
                return new ApiResponse<PartidaResponse>
                {
                    Success = false,
                    Message = "La partida debe tener entre 2 y 4 jugadores",
                    Errors = new List<string> { "Cantidad de jugadores inválida" }
                };
            }

            var partida = new PartidasEntities
            {
                NumeroPartida = GenerarNumeroPartida(),
                FechaInicio = DateTime.Now,
                Estado = Enums.EstadoPartida.EnJuego,
                ConfiguracionTurnos = new List<TurnosPartidasEntities>(),
                Tablero = CargarTablero(),
                Jugadores = new List<UsuariosEntities>(),
                DetalleJugadores = new List<DetallePartidaEntities>()
            };

            for (int i = 0; i < request.JugadoresDNIs.Count; i++)
            {
                partida.ConfiguracionTurnos.Add(new TurnosPartidasEntities
                {
                    NroTurno = i + 1,
                    DNIJugador = request.JugadoresDNIs[i]
                });
            }

            partida.TurnoActual = partida.ConfiguracionTurnos.First();

            foreach (var dni in request.JugadoresDNIs)
            {
                partida.DetalleJugadores.Add(new DetallePartidaEntities
                {
                    NumeroPartida = partida.NumeroPartida,
                    DNIUsuario = dni,
                    PosicionActual = 0,
                    DineroDisponible = 5000000,
                    Estado = Enums.EstadoDetalle.EnJuego,
                    HistorialMovimientos = new List<Movimiento>()
                });

                var usuario = Archivos.LeerUsuario(dni);
                if (usuario != null)
                {
                    partida.Jugadores.Add(usuario);
                }
            }

            Archivos.GuardarPartida(partida);

            return new ApiResponse<PartidaResponse>
            {
                Success = true,
                Message = "Partida creada exitosamente",
                Data = MapearPartida(partida)
            };
        }
        public ApiResponse<PartidaResponse> ObtenerPartida(int numeroPartida)
        {
            var partida = Archivos.LeerPartida(numeroPartida);

            if (partida == null)
            {
                return new ApiResponse<PartidaResponse>
                {
                    Success = false,
                    Message = "Partida no encontrada",
                    Errors = new List<string> { $"No se encontró una partida con el ID {numeroPartida}" }
                };
            }

            var partidaResponse = MapearPartida(partida);

            return new ApiResponse<PartidaResponse>
            {
                Success = true,
                Message = "Partida obtenida exitosamente",
                Data = partidaResponse
            };
        }

        public ApiResponse<MovimientoResponse> LanzarDado(int numeroPartida)
        {
            var partida = Archivos.LeerPartida(numeroPartida);
            if (partida == null)
            {
                return new ApiResponse<MovimientoResponse>
                {
                    Success = false,
                    Message = "Partida no encontrada",
                    Errors = new List<string> { "No existe la partida" }
                };
            }

            if (partida.TurnoActual == null)
            {
                return new ApiResponse<MovimientoResponse>
                {
                    Success = false,
                    Message = "Error crítico: La partida no tiene un turno inicializado.",
                    Errors = new List<string> { "TurnoActual es null" }
                };
            }

            int dniJugador = partida.TurnoActual.DNIJugador;

            var jugador = partida.DetalleJugadores.FirstOrDefault(j => j.DNIUsuario == dniJugador);
            if (jugador == null)
            {
                return new ApiResponse<MovimientoResponse>
                {
                    Success = false,
                    Message = "El jugador asignado al turno actual no fue encontrado.",
                    Errors = new List<string> { "Error de datos internos de la partida" }
                };
            }

            if (jugador.Estado != Enums.EstadoDetalle.EnJuego)
            {
                return new ApiResponse<MovimientoResponse>
                {
                    Success = false,
                    Message = "El jugador no puede lanzar el dado porque está derrotado",
                    Errors = new List<string> { "Estado del jugador inválido" }
                };
            }

            Random random = new Random();
            int valorDado = random.Next(1, 7);
            int nuevaPosicion = jugador.PosicionActual + valorDado;

            if (nuevaPosicion >= partida.Tablero.Count)
            {
                nuevaPosicion = nuevaPosicion - partida.Tablero.Count;
                jugador.DineroDisponible += 100000;
            }

            jugador.PosicionActual = nuevaPosicion;

            var casillero = partida.Tablero.FirstOrDefault(c => c.NroCasillero == nuevaPosicion);
            if (casillero != null)
            {
                AplicarReglasCasillero(partida, jugador, casillero);
            }

            AvanzarTurno(partida);
            VerificarCondicionesVictoria(partida);
            Archivos.GuardarPartida(partida);

            return new ApiResponse<MovimientoResponse>
            {
                Success = true,
                Message = "Jugador movido exitosamente",
                Data = new MovimientoResponse
                {
                    DniJugador = dniJugador, 
                    ValorDado = valorDado,
                    PosicionNueva = nuevaPosicion,
                    DineroDisponible = jugador.DineroDisponible
                }
            };
        }

        public ApiResponse<string> PausarPartida(int numeroPartida)
        {
            var partida = Archivos.LeerPartida(numeroPartida);
            if (partida == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Partida no encontrada",
                    Errors = new List<string> { "No existe la partida" }
                };
            }

            if (partida.Estado != Enums.EstadoPartida.EnJuego)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Solo se pueden pausar partidas en juego",
                    Errors = new List<string> { "Estado inválido" }
                };
            }

            partida.Estado = Enums.EstadoPartida.Pausada;
            Archivos.GuardarPartida(partida);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Partida pausada exitosamente"
            };
        }

        public ApiResponse<string> ReanudarPartida(int numeroPartida)
        {
            var partida = Archivos.LeerPartida(numeroPartida);
            if (partida == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Partida no encontrada",
                    Errors = new List<string> { "No existe la partida" }
                };
            }

            if (partida.Estado != Enums.EstadoPartida.Pausada)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Solo se pueden reanudar partidas pausadas",
                    Errors = new List<string> { "Estado inválido" }
                };
            }

            partida.Estado = Enums.EstadoPartida.EnJuego;
            Archivos.GuardarPartida(partida);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Partida reanudada exitosamente"
            };
        }

        public ApiResponse<string> SuspenderPartida(int numeroPartida)
        {
            var partida = Archivos.LeerPartida(numeroPartida);
            if (partida == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Partida no encontrada",
                    Errors = new List<string> { "No existe la partida" }
                };
            }

            if (partida.Estado != Enums.EstadoPartida.EnJuego)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Solo se pueden suspender partidas en juego",
                    Errors = new List<string> { "Estado inválido" }
                };
            }

            var ganador = partida.DetalleJugadores.OrderByDescending(j => j.DineroDisponible).FirstOrDefault();

            int? dniGanador = ganador?.DNIUsuario;

            ActualizarEstadisticasFinDePartida(partida, dniGanador);

            partida.Estado = Enums.EstadoPartida.Suspendida;
            partida.DNIGanador = ganador?.DNIUsuario;
            partida.MotivoVictoria = Enums.MotivoVictoria.MayorSaldoAlFinal;
            partida.FechaFin = DateTime.Now;
            partida.Duracion = partida.FechaFin - partida.FechaInicio;


            Archivos.GuardarPartida(partida);

            return new ApiResponse<string>
            {
                Success = true,
                Message = $"Partida suspendida. Ganador: {ganador?.DNIUsuario}"
            };
        }

        private void AplicarReglasCasillero(PartidasEntities partida, DetallePartidaEntities jugador, CasilleroTablero casillero)
        {
            switch (casillero.Tipo)
            {
                case Enums.TipoCasillero.Provincia:
                    if (casillero.DNIPropietario == null)
                    {
                        if (jugador.DineroDisponible >= casillero.PrecioCompra.Value)
                        {
                            jugador.DineroDisponible -= casillero.PrecioCompra.Value;
                            casillero.DNIPropietario = jugador.DNIUsuario;

                            jugador.HistorialMovimientos.Add(new Movimiento
                            {
                                Fecha = DateTime.Now,
                                Tipo = Enums.TipoMovimiento.CompraProvincia,
                                Descripcion = $"Compró la provincia {casillero.Nombre} por ${casillero.PrecioCompra.Value:N0}",
                                Monto = -casillero.PrecioCompra.Value,
                                CasilleroOrigen = jugador.PosicionActual,
                                CasilleroDestino = jugador.PosicionActual,
                                DniJugadorAfectado = jugador.DNIUsuario
                            });
                        }
                    }
                    else if (casillero.DNIPropietario != jugador.DNIUsuario)
                    {
                        //////////////////////////////////////
                        ///Esa línea de código es necesaria para encontrar el objeto completo del jugador que es dueño de la propiedad, 
                        ///usando su DNI como clave de búsqueda. El objeto casillero(la propiedad) en el que cayó el jugador solo 
                        ///almacena el DNI del dueño(casillero.DNIPropietario). Pero para poder pagarle el alquiler, 
                        ///no te alcanza solo con su DNI. Necesitas el objeto propietario completo, porque más abajo en el código 
                        ///necesitas acceder a su dinero y a su historial.
                        var propietario = partida.DetalleJugadores.FirstOrDefault(j => j.DNIUsuario == casillero.DNIPropietario);
                        //////////////////////////////////////
                        if (propietario != null)
                        {
                            double montoAlquiler = casillero.PrecioAlquiler.Value;

                            jugador.DineroDisponible -= montoAlquiler;
                            propietario.DineroDisponible += montoAlquiler;

                            jugador.HistorialMovimientos.Add(new Movimiento
                            {
                                Fecha = DateTime.Now,
                                Tipo = Enums.TipoMovimiento.PagoAlquiler,
                                Descripcion = $"Pagó alquiler de ${montoAlquiler:N0} a {casillero.DNIPropietario} por {casillero.Nombre}",
                                Monto = -montoAlquiler,
                                CasilleroOrigen = jugador.PosicionActual,
                                CasilleroDestino = jugador.PosicionActual,
                                DniJugadorAfectado = propietario.DNIUsuario
                            });

                            propietario.HistorialMovimientos.Add(new Movimiento
                            {
                                Fecha = DateTime.Now,
                                Tipo = Enums.TipoMovimiento.PagoAlquiler,
                                Descripcion = $"Cobró alquiler de ${montoAlquiler:N0} de {jugador.DNIUsuario} por {casillero.Nombre}",
                                Monto = montoAlquiler,
                                CasilleroOrigen = propietario.PosicionActual,
                                CasilleroDestino = propietario.PosicionActual,
                                DniJugadorAfectado = jugador.DNIUsuario
                            });
                        }
                    }
                    break;

                case Enums.TipoCasillero.Multa:
                    jugador.DineroDisponible -= casillero.Monto ?? 0;
                    break;

                case Enums.TipoCasillero.Premio:
                    jugador.DineroDisponible += casillero.Monto ?? 0;
                    break;

                case Enums.TipoCasillero.Inicio:
                    break;
            }

            if (jugador.DineroDisponible <= 0)
            {
                jugador.Estado = Enums.EstadoDetalle.Derrotado;
            }
        }

        /// //////////////////////////////////////////
        /// Este método se encarga de pasar el turno al siguiente jugador activo. 
        /// Primero, verifica si queda más de un jugador "EnJuego" (si no, se detiene). 
        /// Luego, partiendo del jugador actual, avanza en la lista de turnos 
        /// (dando la vuelta al llegar al final) y utiliza un bucle do-while para saltar 
        /// a cualquier jugador que ya no esté activo, repitiendo el proceso hasta encontrar 
        /// al próximo jugador válido que siga participando, y finalmente lo asigna como el 
        /// TurnoActual de la partida.
        private void AvanzarTurno(PartidasEntities partida)
        {
            var jugadoresActivosDNIs = partida.DetalleJugadores
                                            .Where(j => j.Estado == Enums.EstadoDetalle.EnJuego)
                                            .Select(j => j.DNIUsuario)
                                            .ToHashSet();

            if (jugadoresActivosDNIs.Count <= 1)
            {
                return;
            }

            int indiceActual = partida.ConfiguracionTurnos
                                    .FindIndex(t => t.DNIJugador == partida.TurnoActual.DNIJugador);

            TurnosPartidasEntities siguienteTurno;
            int maxIntentos = partida.ConfiguracionTurnos.Count;

            do
            {
                int indiceSiguiente = (indiceActual + 1) % partida.ConfiguracionTurnos.Count;
                siguienteTurno = partida.ConfiguracionTurnos[indiceSiguiente];

                indiceActual = indiceSiguiente;
                maxIntentos--;

            } while (!jugadoresActivosDNIs.Contains(siguienteTurno.DNIJugador) && maxIntentos > 0);

            partida.TurnoActual = siguienteTurno;
        }
        /////////////////////
        private int GenerarNumeroPartida()
        {
            return Archivos.ObtenerSiguienteNumeroPartida();
        }

        //Esta función carga una plantilla de tablero y la clona (convirtiéndola a JSON y luego de vuelta a objeto)
        //para crear un nuevo tablero independiente para la partida.
        private List<CasilleroTablero> CargarTablero()
        {
            var plantillaTablero = Archivos.CargarTableroTemplate();
            ///////////////////////////////
            var jsonPlantilla = JsonConvert.SerializeObject(plantillaTablero);
            var tableroParaPartida = JsonConvert.DeserializeObject<List<CasilleroTablero>>(jsonPlantilla);
            /////////////////////////
            return tableroParaPartida;
        }
        private PartidaResponse MapearPartida(PartidasEntities partida)
        {
            string nombreGanador = null;
            if (partida.DNIGanador.HasValue)
            {
                var usuarioGanador = Archivos.LeerUsuario(partida.DNIGanador.Value);
                if (usuarioGanador != null)
                {
                    nombreGanador = usuarioGanador.Nombre;
                }
                else
                {
                    nombreGanador = $"DNI: {partida.DNIGanador.Value}";
                }
            }

            return new PartidaResponse
            {
                NumeroPartida = partida.NumeroPartida,
                FechaInicio = partida.FechaInicio,
                Estado = partida.Estado.ToString(),
                Jugadores = partida.DetalleJugadores.Select(j => {

                    var usuario = Archivos.LeerUsuario(j.DNIUsuario);

                    var nombreJugador = (usuario != null) ? usuario.Nombre : $"DNI: {j.DNIUsuario}";

                    return new JugadorResponse
                    {
                        Dni = j.DNIUsuario,
                        Nombre = nombreJugador, 
                        Posicion = j.PosicionActual,
                        Dinero = j.DineroDisponible,
                        Estado = j.Estado.ToString()
                    };
                }).ToList(),

                TurnoActual = partida.TurnoActual != null ? new TurnoResponse
                {
                    NroTurno = partida.TurnoActual.NroTurno,
                    DNIJugador = partida.TurnoActual.DNIJugador
                } : null
            };

        }
        private void VerificarCondicionesVictoria(PartidasEntities partida)
        {
            var jugadorConDoceProvincias = partida.DetalleJugadores.FirstOrDefault(j =>
                partida.Tablero.Count(c => c.DNIPropietario == j.DNIUsuario) >= 12);

            if (jugadorConDoceProvincias != null)
            {
                FinalizarPartida(partida, jugadorConDoceProvincias.DNIUsuario, Enums.MotivoVictoria.DoceProvincias);
                return;
            }

            var jugadoresConSaldoPositivo = partida.DetalleJugadores.Where(j => j.DineroDisponible > 0).ToList();
            if (jugadoresConSaldoPositivo.Count == 1)
            {
                FinalizarPartida(partida, jugadoresConSaldoPositivo.First().DNIUsuario, Enums.MotivoVictoria.UnicoConSaldoPositivo);
                return;
            }
        }
        private void ActualizarEstadisticasFinDePartida(PartidasEntities partida, int? dniGanador)
        {
            foreach (var detalleJugador in partida.DetalleJugadores)
            {
                var usuario = Archivos.LeerUsuario(detalleJugador.DNIUsuario);

                if (usuario != null)
                {
                    if (usuario.Estadisticas == null)
                    {
                        usuario.Estadisticas = new EstadisticasJugador();
                    }

                    usuario.Estadisticas.PartidasJugadas++;

                    if (dniGanador.HasValue && usuario.DNI == dniGanador.Value)
                    {
                        usuario.Estadisticas.PartidasGanadas++;
                    }
                    else
                    {
                        usuario.Estadisticas.PartidasPerdidas++;
                    }

                    Archivos.GuardarUsuario(usuario);
                }
            }
        }
        private void FinalizarPartida(PartidasEntities partida, int dniGanador, Enums.MotivoVictoria motivo)
        {
            ActualizarEstadisticasFinDePartida(partida, dniGanador);

            partida.Estado = Enums.EstadoPartida.Finalizada;
            partida.DNIGanador = dniGanador;
            partida.MotivoVictoria = motivo;
            partida.FechaFin = DateTime.Now;
            partida.Duracion = partida.FechaFin - partida.FechaInicio;

            Archivos.GuardarPartida(partida);
        }
    }
}

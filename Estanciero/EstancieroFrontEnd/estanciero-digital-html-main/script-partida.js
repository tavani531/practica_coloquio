const API_URL = 'http://localhost:5034'; 

let numeroPartidaActual = null;
let partidaActual = null; 

function mostrarError(mensaje) {
    alert(mensaje); 
}

document.addEventListener("DOMContentLoaded", function(event) {
    console.log("Página de partida cargada correctamente");
    inicializarPartida();
});


async function inicializarPartida() {
    const urlParams = new URLSearchParams(window.location.search);
    const idPartida = urlParams.get('id'); 

    if (!idPartida) {
        mostrarError('No se especificó el número de partida en la URL. Volviendo al inicio.');
        window.location.href = 'index.html';
        return;
    }

    numeroPartidaActual = parseInt(idPartida);

    try {
        await cargarEstadoDeLaPartida();

        if (partidaActual) {
            crearTableroVisual(); 
            configurarEventosPartida(); 
            actualizarUICompleta();
        } else {
             throw new Error("No se pudo obtener la información inicial de la partida.");
        }

    } catch (error) {
        console.error('Error al inicializar partida:', error);
        mostrarError(`Error al cargar la partida: ${error.message}. Volviendo al inicio.`);
    }
}

async function cargarEstadoDeLaPartida() {
    if (!numeroPartidaActual) return;

    try {
        const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartidaActual}`);

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ message: `Error HTTP ${response.status}` }));
            throw new Error(errorData.message || `Error HTTP ${response.status}`);
        }

        const apiResponse = await response.json();
        if (!apiResponse.success) {
            throw new Error(apiResponse.message || 'La API indicó un error al obtener la partida.');
        }

        partidaActual = apiResponse.data;
        console.log("Estado de la partida cargado:", partidaActual);

    } catch (error) {
        console.error('Error al cargar estado de la partida:', error);
        mostrarError(`Error al obtener datos de la partida: ${error.message}`);
        partidaActual = null; 
    }
}

function crearTableroVisual() {
    const casillerosGrid = document.getElementById('casilleros-grid');
    if (!casillerosGrid) return; 

    casillerosGrid.innerHTML = ''; 

    for (let i = 1; i <= 30; i++) {
        const casillero = document.createElement('div');
        casillero.className = 'casillero';
        casillero.id = `casillero-${i}`;

        const numero = document.createElement('span');
        numero.className = 'numero-casillero';
        numero.textContent = i;

        const jugadoresEnCasillero = document.createElement('div');
        jugadoresEnCasillero.className = 'jugadores-en-casillero';
        jugadoresEnCasillero.id = `jugadores-${i}`; 

        casillero.appendChild(numero);
        casillero.appendChild(jugadoresEnCasillero);
        casillerosGrid.appendChild(casillero);
    }
     const casilleroCeroVisual = document.getElementById('jugadores-0');
     if (casilleroCeroVisual) casilleroCeroVisual.innerHTML = ''; 
}

function configurarEventosPartida() {
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    if (btnLanzarDado) btnLanzarDado.addEventListener('click', lanzarDado);

    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    const btnSuspender = document.getElementById('btnSuspender');

    if (btnPausar) btnPausar.addEventListener('click', pausarPartida);
    if (btnReanudar) btnReanudar.addEventListener('click', reanudarPartida);
    if (btnSuspender) btnSuspender.addEventListener('click', suspenderPartida);

    const btnRegresar = document.getElementById('btnRegresar'); 
    if(btnRegresar) btnRegresar.addEventListener('click', regresarAlInicio);
}

async function lanzarDado() {
    if (!partidaActual || !numeroPartidaActual) {
        mostrarError('No hay datos de partida cargados.');
        return;
    }
    if (partidaActual.estado !== 'EnJuego') {
        mostrarError('La partida no está en juego.');
        return;
    }

    const btnLanzar = document.getElementById('btnLanzarDado');
    try {
        if(btnLanzar) btnLanzar.disabled = true; 
        const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartidaActual}/lanzardado`, { 
            method: 'POST'
        });

        const apiResponse = await response.json();

        if (!response.ok || !apiResponse.success) {
            throw new Error(apiResponse.message || `Error HTTP ${response.status}`);
        }

        const resultadoDado = apiResponse.data;

        const ultimoDadoSpan = document.getElementById('ultimoDado');
        if (ultimoDadoSpan) ultimoDadoSpan.textContent = resultadoDado.valorDado;

        alert(`Jugador ${resultadoDado.dniJugador} sacó un ${resultadoDado.valorDado}. Se mueve a la posición ${resultadoDado.posicionNueva}. Saldo: $${resultadoDado.dineroDisponible.toLocaleString()}`);

        await cargarEstadoDeLaPartida();
        if (partidaActual) { 
             actualizarUICompleta(); 
        }

    } catch (error) {
        console.error('Error al lanzar dado:', error);
        mostrarError(`Error al lanzar dado: ${error.message}`);
    } finally {
        if (btnLanzar && partidaActual && partidaActual.estado === 'EnJuego') {
            btnLanzar.disabled = false;
        }
    }
}


function actualizarUICompleta() {
    if (!partidaActual) return;

    document.title = `Partida ${partidaActual.numeroPartida} - El Estanciero Digital`;
    actualizarEstadoGeneralPartida(partidaActual.estado);

    actualizarInfoJugadores();

    actualizarPosicionesVisuales();
}

function actualizarEstadoGeneralPartida(estado) {
    const btnLanzarDado = document.getElementById('btnLanzarDado');
    const btnPausar = document.getElementById('btnPausar');
    const btnReanudar = document.getElementById('btnReanudar');
    const btnSuspender = document.getElementById('btnSuspender');
    const btnRegresar = document.getElementById('btnRegresar');

    if (btnLanzarDado) { btnLanzarDado.disabled = true; btnLanzarDado.style.display = 'block'; }
    if (btnPausar) { btnPausar.disabled = true; btnPausar.style.display = 'block'; }
    if (btnReanudar) { btnReanudar.disabled = true; btnReanudar.style.display = 'none'; }
    if (btnSuspender) btnSuspender.disabled = true;
    if (btnRegresar) btnRegresar.style.display = 'none'; 

    if (estado === 'EnJuego') {
        if (btnLanzarDado) btnLanzarDado.disabled = false;
        if (btnPausar) btnPausar.disabled = false;
        if (btnSuspender) btnSuspender.disabled = false;
    } else if (estado === 'Pausada') {
        if (btnReanudar) { btnReanudar.disabled = false; btnReanudar.style.display = 'block'; } 
        if (btnPausar) btnPausar.style.display = 'none'; 
        if (btnLanzarDado) btnLanzarDado.style.display = 'none'; 
        if (btnSuspender) btnSuspender.disabled = false;
        if (btnRegresar) btnRegresar.style.display = 'block'; 
    } else {
        if (btnLanzarDado) btnLanzarDado.style.display = 'none';
        if (btnPausar) btnPausar.style.display = 'none';
        if (btnReanudar) btnReanudar.style.display = 'none';
        if (btnSuspender) btnSuspender.disabled = true;
        if (btnRegresar) btnRegresar.style.display = 'block';
    }
}

function actualizarInfoJugadores() {
    if (!partidaActual || !Array.isArray(partidaActual.jugadores)) {
        console.warn("No hay datos de jugadores para actualizar.");
        return;
    }

    partidaActual.jugadores.forEach((jugador, index) => {
        const numeroJugador = index + 1;

        const nombreElement = document.getElementById(`nombreJugador${numeroJugador}`);
        const saldoElement = document.getElementById(`saldoJugador${numeroJugador}`);
        const estadoElement = document.getElementById(`estadoJugador${numeroJugador}`);
        const posicionElement = document.getElementById(`posicionJugador${numeroJugador}`);
        const infoDivElement = document.getElementById(`jugador${numeroJugador}-info`);
        if (nombreElement) {
            nombreElement.textContent = jugador.nombre;
        }
        if (saldoElement) {
            saldoElement.textContent = jugador.dinero.toLocaleString(); 
        }
        if (estadoElement) {
            estadoElement.textContent = jugador.estado;
        }
        if (posicionElement) {
            posicionElement.textContent = jugador.posicion;
        }

        if (infoDivElement) {
            infoDivElement.style.display = 'block';
            
             infoDivElement.classList.remove('turno-actual', 'derrotado'); 

             if (partidaActual.turnoActual && partidaActual.turnoActual.dniJugador === jugador.dni) {
                 infoDivElement.classList.add('turno-actual'); 
             }
             if (jugador.estado === 'Derrotado') {
                 infoDivElement.classList.add('derrotado'); 
             }
        }
    });

    const maxJugadoresPosiblesEnHTML = 4; 
    for (let i = partidaActual.jugadores.length + 1; i <= maxJugadoresPosiblesEnHTML; i++) {
        const divOcultar = document.getElementById(`jugador${i}-info`);
        if (divOcultar) {
            divOcultar.style.display = 'none';
        }
    }
}

function actualizarPosicionesVisuales() {
    for (let i = 0; i <= 30; i++) {
        const jugadoresEnCasillero = document.getElementById(`jugadores-${i}`);
        if (jugadoresEnCasillero) {
            jugadoresEnCasillero.innerHTML = '';
        }
    }

    if (!partidaActual || !Array.isArray(partidaActual.jugadores)) return;

    partidaActual.jugadores.forEach((jugador, index) => {
        const jugadoresEnCasillero = document.getElementById(`jugadores-${jugador.posicion}`);

        if (jugadoresEnCasillero) {
            const esTurnoActual = partidaActual.turnoActual && partidaActual.turnoActual.dniJugador === jugador.dni;
            const colores = ['#e74c3c', '#27ae60', '#f39c12', '#9b59b6'];
            const colorFondo = colores[index % colores.length] || '#95a5a6';

            const indicador = document.createElement('div');
            indicador.className = 'indicador-jugador';
            indicador.title = `Jugador ${jugador.dni}`;
            indicador.style.backgroundColor = colorFondo;
            if (esTurnoActual) {
                 indicador.style.border = '4px solid black';
                 indicador.style.transform = 'scale(1.1)'; 
                 indicador.style.zIndex = '10'; 
            }
            jugadoresEnCasillero.appendChild(indicador);
        }
    });
}


async function pausarPartida() {
    if (!numeroPartidaActual) return;
    try {
        const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartidaActual}/pausar`, { method: 'POST' });
        const apiResponse = await response.json();
        if (!response.ok || !apiResponse.success) throw new Error(apiResponse.message || `Error HTTP ${response.status}`);
        alert('Partida pausada');
        await cargarEstadoDeLaPartida();
        if(partidaActual) actualizarUICompleta();
    } catch (error) {
        console.error('Error al pausar partida:', error);
        mostrarError(`Error al pausar: ${error.message}`);
    }
}

async function reanudarPartida() {
     if (!numeroPartidaActual) return;
    try {
        const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartidaActual}/reanudar`, { method: 'POST' });
        const apiResponse = await response.json();
        if (!response.ok || !apiResponse.success) throw new Error(apiResponse.message || `Error HTTP ${response.status}`);
        alert('Partida reanudada');
        await cargarEstadoDeLaPartida();
        if(partidaActual) actualizarUICompleta();
    } catch (error) {
        console.error('Error al reanudar partida:', error);
        mostrarError(`Error al reanudar: ${error.message}`);
    }
}

async function suspenderPartida() {
    if (!numeroPartidaActual) return;
    if (confirm('¿Está seguro de que desea suspender la partida? Esto finalizará el juego.')) {
        try {
            const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartidaActual}/suspender`, { method: 'POST' });
            const apiResponse = await response.json();
            if (!response.ok || !apiResponse.success) throw new Error(apiResponse.message || `Error HTTP ${response.status}`);

            alert(apiResponse.message);
            
            await cargarEstadoDeLaPartida();
            if(partidaActual) {
                actualizarUICompleta();
                if (partidaActual.dniGanador) {
                    alert(`¡Partida finalizada! El ganador es: ${partidaActual.dniGanador}`);
                }
            }
        } catch (error) {
            console.error('Error al suspender partida:', error);
            mostrarError(`Error al suspender: ${error.message}`);
        }
    }
}

function regresarAlInicio() {
    window.location.href = 'index.html';
}
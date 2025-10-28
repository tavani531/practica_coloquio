
const API_URL = 'http://localhost:5034';

let jugadoresRegistrados = [];

function mostrarError(mensaje) {
    alert(mensaje);
}
async function cargarJugadoresRegistrados() {
    try {
        const response = await fetch(`${API_URL}/api/usuario`);

        if (response.ok) {
            const apiResponse = await response.json();
            jugadoresRegistrados = (apiResponse.success && Array.isArray(apiResponse.data)) ? apiResponse.data : [];
            return jugadoresRegistrados;
        } else {
            console.error('Error al cargar jugadores desde la API:', response.status);
            jugadoresRegistrados = [];
            return [];
        }
    } catch (error) {
        console.error('Error de red al cargar jugadores:', error);
        jugadoresRegistrados = [];
        return [];
    }
}

document.addEventListener("DOMContentLoaded", function(event) {
    console.log("Página de inicio cargada correctamente");
    inicializarInicio();
});

function inicializarInicio() {
    const formNuevaPartida = document.getElementById('formNuevaPartida');
    const formContinuarPartida = document.getElementById('formContinuarPartida');
    const btnGestionarJugadores = document.getElementById('btnGestionarJugadores');

    if (formNuevaPartida) {
        formNuevaPartida.addEventListener('submit', function(event) {
            event.preventDefault();
            crearNuevaPartida();
        });
    }
    if (formContinuarPartida) {
        formContinuarPartida.addEventListener('submit', function(event) {
            event.preventDefault();
            continuarPartida();
        });
    }
    if (btnGestionarJugadores) {
        btnGestionarJugadores.addEventListener('click', function() {
            window.location.href = 'gestionJugadores.html';
        });
    }

    cargarJugadoresRegistrados().then(() => {
        actualizarSelectoresJugadores();
    });
}

async function crearNuevaPartida() {
    const dniJugador1 = document.getElementById('dniJugador1').value;
    const dniJugador2 = document.getElementById('dniJugador2').value;
    const dniJugador3 = document.getElementById('dniJugador3').value;
    const dniJugador4 = document.getElementById('dniJugador4').value;

    if (!dniJugador1 || !dniJugador2) {
        alert('Debe seleccionar un DNI para los Jugadores 1 y 2.');
        return;
    }
    
    const jugadoresDNIs = [
        parseInt(dniJugador1),
        parseInt(dniJugador2)
    ];

    if (dniJugador3) {
        jugadoresDNIs.push(parseInt(dniJugador3));
    }
    if (dniJugador4) {
        jugadoresDNIs.push(parseInt(dniJugador4));
    }

    const jugadoresUnicos = new Set(jugadoresDNIs);
    if (jugadoresUnicos.size !== jugadoresDNIs.length) {
        alert('No puede seleccionar el mismo jugador (mismo DNI) más de una vez.');
        return;
    }

    try {
        const response = await fetch(`${API_URL}/api/partida/partidas`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                jugadoresDNIs: jugadoresDNIs 
            })
        });

        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                const numeroPartida = apiResponse.data.numeroPartida;
                window.location.href = `partida.html?id=${numeroPartida}`; 
            } else {
                alert('Error al crear la partida: ' + apiResponse.message);
            }
        } else {
            const error = await response.json().catch(() => ({})); 
            alert('Error al crear la partida: ' + (error.message || `Status ${response.status}`));
        }
    } catch (error) {
        console.error('Error de red al crear partida:', error);
        alert('Error al crear la partida. Verifique que la API esté funcionando.');
    }
}

async function continuarPartida() {
    const numeroPartida = document.getElementById('numeroPartida').value;
    if (!numeroPartida) {
        alert('Por favor, ingrese un número de partida.');
        return;
    }

    try {
        const response = await fetch(`${API_URL}/api/partida/partidas/${numeroPartida}`);

        if (response.ok) {
            const apiResponse = await response.json();
            if (apiResponse.success) {
                window.location.href = `partida.html?id=${numeroPartida}`; 
            } else {
                alert('Error al cargar la partida: ' + apiResponse.message);
            }
        } else if (response.status === 404) {
            alert('La partida no existe o no se pudo encontrar. Verifique el número.');
        } else {
             const error = await response.json().catch(() => ({}));
            alert('Error al cargar la partida: ' + (error.message || `Status ${response.status}`));
        }
    } catch (error) {
        console.error('Error de red al continuar partida:', error);
        alert('Error al cargar la partida. Verifique que la API esté funcionando.');
    }
}

function actualizarSelectoresJugadores() {
    const selectJugador1 = document.getElementById('dniJugador1');
    const selectJugador2 = document.getElementById('dniJugador2');
    const selectJugador3 = document.getElementById('dniJugador3');
    const selectJugador4 = document.getElementById('dniJugador4');

    if (!selectJugador1 || !selectJugador2 || !selectJugador3 || !selectJugador4) return;

    selectJugador1.innerHTML = '';
    selectJugador2.innerHTML = '';
    selectJugador3.innerHTML = '';
    selectJugador4.innerHTML = '';

    const opcionPorDefecto = document.createElement('option');
    opcionPorDefecto.value = '';
    opcionPorDefecto.textContent = 'Seleccione DNI';
    opcionPorDefecto.disabled = true;
    opcionPorDefecto.selected = true;

    selectJugador1.appendChild(opcionPorDefecto.cloneNode(true));
    selectJugador2.appendChild(opcionPorDefecto);

    const opcionOpcional = document.createElement('option');
    opcionOpcional.value = '';
    opcionOpcional.textContent = 'Seleccione DNI (Opcional)';
    opcionOpcional.selected = true;

    selectJugador3.appendChild(opcionOpcional.cloneNode(true));
    selectJugador4.appendChild(opcionOpcional.cloneNode(true));

    if (!Array.isArray(jugadoresRegistrados) || jugadoresRegistrados.length === 0) {
        console.warn("No hay jugadores registrados para mostrar en los selects.");
        return;
    }

    jugadoresRegistrados.forEach(jugador => {
        const option = document.createElement('option');
        option.value = jugador.dni;
        option.textContent = `${jugador.nombre} (${jugador.dni})`;
        selectJugador1.appendChild(option.cloneNode(true));
        selectJugador2.appendChild(option);
        selectJugador3.appendChild(option.cloneNode(true));
        selectJugador4.appendChild(option.cloneNode(true));
    });
}
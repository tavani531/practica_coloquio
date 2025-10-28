
const API_URL = 'http://localhost:5034';

document.addEventListener('DOMContentLoaded', () => {
    cargarJugadores();
    const formCrearJugador = document.getElementById('formCrearJugador');
    if (formCrearJugador) {
        formCrearJugador.addEventListener('submit', manejarCrearJugador);
    }
    const tablaBody = document.getElementById('tablaJugadoresBody');
    if (tablaBody) {
        tablaBody.addEventListener('click', manejarClicTabla);
    }
});

async function cargarJugadores() {
    const tablaBody = document.getElementById('tablaJugadoresBody');
    tablaBody.innerHTML = '<tr><td colspan="2">Cargando jugadores...</td></tr>';
    try {
        const response = await fetch(`${API_URL}/api/usuario`);
        if (!response.ok) throw new Error(`Error ${response.status}: No se pudo cargar la lista.`);
        const apiResponse = await response.json();
        if (apiResponse.success && Array.isArray(apiResponse.data)) {
            mostrarJugadoresEnTabla(apiResponse.data);
        } else {
            console.warn("API devolvió éxito falso o data no es array:", apiResponse);
            tablaBody.innerHTML = '<tr><td colspan="2">No se encontraron jugadores o hubo un error.</td></tr>';
        }
    } catch (error) {
        console.error('Error al cargar jugadores:', error);
        tablaBody.innerHTML = `<tr><td colspan="2">Error: ${error.message}</td></tr>`;
    }
}

function mostrarJugadoresEnTabla(jugadores) {
    const tablaBody = document.getElementById('tablaJugadoresBody');
    if (jugadores.length === 0) {
        tablaBody.innerHTML = '<tr><td colspan="2">No hay jugadores registrados.</td></tr>';
        return;
    }
    tablaBody.innerHTML = '';
    jugadores.forEach(jugador => {
        const fila = document.createElement('tr');
        fila.innerHTML = `
            <td>${jugador.nombre} (${jugador.dni})</td>
            <td><button class="boton-eliminar" data-dni="${jugador.dni}">Eliminar</button></td>
        `;
        tablaBody.appendChild(fila);
    });
}

async function manejarCrearJugador(event) {
    event.preventDefault();
    const inputDNI = document.getElementById('dniNuevo');
    const inputNombre = document.getElementById('nombreNuevo');
    const inputEmail = document.getElementById('emailNuevo');
    const dni = inputDNI.value.trim();
    const nombre = inputNombre.value.trim();
    const email = inputEmail.value.trim();

    if (!dni || !nombre || !email) {
        alert('Por favor, complete DNI, Nombre y Email.');
        return;
    }

    try {
        const response = await fetch(`${API_URL}/api/usuario`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                dni: parseInt(dni),
                nombre: nombre,
                email: email
            })
        });
        const apiResponse = await response.json();
        if (response.ok && apiResponse.success) {
            alert('¡Jugador creado!');
            inputDNI.value = '';
            inputNombre.value = '';
            inputEmail.value = '';
            cargarJugadores();
        } else {
            throw new Error(apiResponse.message || 'No se pudo crear el jugador.');
        }
    } catch (error) {
        console.error('Error al crear jugador:', error);
        alert(`Error: ${error.message}`);
    }
}

function manejarClicTabla(event) {
    if (event.target.classList.contains('boton-eliminar')) {
        const dni = event.target.dataset.dni;
        if (dni && confirm(`¿Eliminar al jugador con DNI ${dni}?`)) {
            eliminarJugador(dni);
        }
    }
}

async function eliminarJugador(dni) {
    try {
        const response = await fetch(`${API_URL}/api/usuario/${dni}`, {
            method: 'DELETE'
        });
        const apiResponse = await response.json().catch(() => ({}));
        if (response.ok && apiResponse.success) {
            alert('Jugador eliminado.');
            cargarJugadores();
        } else {
            throw new Error(apiResponse.message || `Error ${response.status}`);
        }
    } catch (error) {
        console.error('Error al eliminar jugador:', error);
        alert(`Error: ${error.message}`);
    }
}
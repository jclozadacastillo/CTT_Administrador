const baseUrl = `${_route}Estudiantes/`;
frmDatos.addEventListener("submit", e => {
    e.preventDefault();
    authorization();
});

async function authorization() {
    try {
        bloquearBotones();
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos")
        const url = `${baseUrl}authorization`;
        const data = new FormData(frmDatos);
        addAntiForgeryToken(data);
        await axios.post(url, data);
        top.location.reload();
    } catch (e) {
        handleError(e);
    } finally {
        desbloquearBotones();
    }
}
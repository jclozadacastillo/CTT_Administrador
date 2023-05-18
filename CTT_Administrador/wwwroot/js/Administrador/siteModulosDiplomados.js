const baseUrl = `${_route}ModulosDiplomados/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idCurso = 0;
let activo = 1;
const mayusculas = true;
(async function () {
    if (mayusculas) frmDatos.classList.add("to-uppercase");
    loader.hidden = false;
    $(idCursoDiplomado).select2();
    activarValidadores(frmDatos);
    await comboDiplomados();
    loader.hidden = true;
    content.hidden = false;
})();
async function listar() {
    try {
        idCursoDiplomado.value == "" ? btnAgregar.hidden = true : btnAgregar.removeAttribute("hidden");
        datosDiplomado.innerHTML = "";
        if (idCursoDiplomado.value != "") {
            let url = `${baseUrl}datosDiplomado`;
            const data = new FormData();
            data.append("idCursoDiplomado", idCursoDiplomado.value);
            const res = (await axios.post(url, data)).data;
            let html = `<b class='fs-xxs text-primary'>DATOS DIPLOMADO</b>
                          <table class='table-datos-diplomado'>
                            <tr>
                                <td class='title'>DIPLOMADO</td>
                                <td>${res.curso}</td>
                            </tr>
                            <tr>
                                <td class='title'>TOTAL HORAS</td>
                                <td class='text-end'>${res.horasCurso}</td>
                            </tr>
                            <tr>
                                <td class='title'>PRECIO</td>
                                <td class='text-end'>$${res.precioCurso}</td>
                            </tr>
                            <tr>
                                <td class='title'>PUNTAJE MÁXIMO</td>
                                <td class='text-end'>${res.puntajeMaximo}</td>
                            </tr>
                            <tr>
                                <td class='title'>PUNTAJE MÍNIMO</td>
                                <td class='text-end'>${res.puntajeMinimo}</td>
                            </tr>
                        </table>`;
            datosDiplomado.innerHTML = html;
        }
        const url = `${baseUrl}listar`;
        const data = new FormData();
        data.append("idCursoDiplomado",idCursoDiplomado.value)
        const res = (await axios.post(url,data)).data;
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    data: "idCurso",
                    class: "text-center td-switch",
                    render: (data, type, row) => {
                        const eliminar = row.eliminable == true ?
                            `<li>
                                <a class="dropdown-item"
                                onclick="eliminar('${data}')">
                                <i class='bi-trash-fill me-1 text-gray'></i>
                                <small>ELIMINAR</small>
                                </a>
                             </li>
                            `
                            :
                            ``;
                        return `<div class="btn-group dropleft" role="group">
                                      <a id="btnGroup${data}" type="button" class="dropdown-toggle no-arrow btn-group-sm" data-bs-toggle="dropdown" aria-expanded="false">
                                              <i class='bi-three-dots-vertical'></i>
                                       </a>
                                          <ul class="dropdown-menu" aria-labelledby="btnGroup${data}">
                                              <li>
                                              <a class="dropdown-item"
                                              onclick="editar('${data}')">
                                              <i class='bi-pencil-fill me-1 text-gray'></i>
                                              <small>EDITAR</small>
                                              </a>
                                              </li>
                                              ${eliminar}
                                          </ul>
                                </div>`;
                    }
                },
                {
                    title: "<i class='bi-eye-fill'></i>",
                    data: "activo",
                    className: "text-center switch-td",
                    render: (data, type, row) => {
                        return `
                                <label class="switch">
                                       <input type="checkbox"
                                            id="check${row.idCurso}" ${(row.activo == true) ? "checked" : ""}
                                            onchange="activar('${row.idCurso}',this)"
                                            />
                                        <span class="slider"></span>
                                </label>
                                `;
                    }
                },
                {
                    title: "Módulo",
                    data: "curso",
                    class: "w-100",
                    render: (data) => {
                        return `<span class='ellipsis w-370' title="${data}">${data}</span>`;
                    }
                },

            ],
            columnDefs: [
                { targets: [0, 1], orderable: false }
            ],
            aaSorting: []
        })
    } catch (e) {
        handleError(e);
    }
}


async function comboDiplomados() {
    try {
        let html = "<option value=''>SELECCIONE</option>";
        const url = `${baseUrl}comboDiplomados`;
        const res = (await axios.get(url)).data;
        res.forEach(item => {
            html += `<option value='${item.idCurso}'>${item.curso}</option>`;
        });
        idCursoDiplomado.innerHTML = html;
        listar();
    } catch (e) {
        handleError(e);
    }
}

function comboCursos(_cursos) {
    try {
        let html = "<option value=''>Seleccione</option>";
        _cursos.forEach(item => {
            html += `<option value='${item.idCurso}'>${item.curso}</option>`;
        });
        idCursoPrecedencia.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function nuevo() {
    idCurso = 0;
    activo = true;
    limpiarForm(frmDatos);
    modal.show();
    modalDatosLabel.innerHTML = "Nuevo registro";
    handleAsistencia();
    handlePrecedencia();
}

async function comboCategorias() {
    try {
        const url = `${baseUrl}comboCategorias`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idCategoria}'>${item.categoria}</option>`;
        });
        idCategoria.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboTiposCursos() {
    try {
        const url = `${baseUrl}comboTiposCursos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function guardar() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        bloquearBotones();
        const url = `${baseUrl}guardar`;
        if (mayusculas) formToUpperCase(frmDatos);
        const data = new FormData(frmDatos);
        data.append("idCurso", idCurso);
        data.append("idCursoDiplomado", idCursoDiplomado.value);
        data.append("activo", activo);
        await axios.post(url, data);
        toastSuccess("<b>Guardado</b> con éxito");
        modal.hide();
        listar();
    } catch (e) {
        handleError(e);
    } finally {
        desbloquearBotones();
    }
}

async function editar(_idCurso) {
    try {
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idCurso", _idCurso);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        console.log(res);
        idCurso = res.idCurso;
        activo = res.activo == 1 || res.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res);
        handleAsistencia(true);
        handlePrecedencia(true);
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idCurso, _switch) {
    try {
        const url = `${baseUrl}activar`;
        const data = new FormData();
        data.append("idCurso", _idCurso);
        await axios.post(url, data);
        toastSuccess(`<b>${_switch.checked ? "Activado" : "Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked = !_switch.checked;
    }
}

function handlePrecedencia(_edita) {
    idCursoPrecedencia.setAttribute("data-validate", "no-validate")
    divPrecedencia.hidden = !tienePrecedencia.checked;
    if (!_edita) limpiarForm(divPrecedencia);
    if (tienePrecedencia.checked) idCursoPrecedencia.removeAttribute("data-validate");;
    activarValidadores(frmDatos);
}

function handleAsistencia(_edita) {
    asistenciaMinima.setAttribute("data-validate", "no-validate")
    divAsistencia.hidden = !calificaAsistencia.checked;
    if (!_edita) limpiarForm(divAsistencia);
    if (calificaAsistencia.checked) asistenciaMinima.removeAttribute("data-validate");;
    activarValidadores(frmDatos);
}

numeroNotas.addEventListener("focusout", () => {
    if (parseInt(numeroNotas.value) > 5) numeroNotas.value = "5";
})
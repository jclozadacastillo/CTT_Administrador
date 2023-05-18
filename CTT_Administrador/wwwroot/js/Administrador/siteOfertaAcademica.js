const baseUrl = `${_route}OfertaAcademica/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idGrupoCurso = 0;
let activo = 1;
const mayusculas = true;
(async function () {
    if (mayusculas) frmDatos.classList.add("to-uppercase");
    loader.hidden = false;
    $(idPeriodo).select2();
    $(idCurso).select2({ dropdownParent: modalDatos });
    $(idModalidad).select2({ dropdownParent: modalDatos });
    activarValidadores(frmDatos);
    await comboPeriodos();
    loader.hidden = true;
    content.hidden = false;
})();
async function listar() {
    try {
        idPeriodo.value == "" ? btnAgregar.hidden = true : btnAgregar.removeAttribute("hidden");
        const url = `${baseUrl}listar`;
        const data = new FormData();
        data.append("idPeriodo", idPeriodo.value)
        const res = (await axios.post(url,data)).data;
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    data: "idGrupoCurso",
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
                    title: "Tipo",
                    data:"tipoCurso"
                },
                {
                    title: "Nombre",
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


async function comboPeriodos() {
    try {
        let html = "<option value=''>SELECCIONE</option>";
        const url = `${baseUrl}comboPeriodos`;
        const res = (await axios.get(url)).data;
        res.forEach(item => {
            html += `<option value='${item.idPeriodo}'>${item.detalle}</option>`;
        });
        idPeriodo.innerHTML = html;
        listar();
    } catch (e) {
        handleError(e);
    }
}

async function comboCursos() {
    try {
        let html = "<option value=''>SELECCIONE</option>";
        const url = `${baseUrl}comboCursos`;
        const data = new FormData();
        data.append("idPeriodo", idPeriodo.value);  
        const res = (await axios.post(url, data)).data;
        res.forEach(item => {
            html += `<option value='${item.idCurso}'>(${item.tipoCurso}) ${item.curso}</option>`;
        });
        idCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboModalidades() {
    try {
        let html = "<option value=''>SELECCIONE</option>";
        const url = `${baseUrl}comboModalidades`;
        const res = (await axios.get(url)).data;
        res.forEach(item => {
            html += `<option value='${item.idModalidad}'>${item.modalidad}</option>`;
        });
        idModalidad.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}



async function nuevo() {
    idGrupoCurso = 0;
    await comboCursos();
    await comboModalidades();
    activo = true;
    limpiarForm(frmDatos);
    modal.show();
    modalDatosLabel.innerHTML = "Nuevo registro";
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
        data.append("idGrupoCurso", idGrupoCurso);
        data.append("idPeriodo", idPeriodo.value);
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

async function editar(_idGrupoCurso) {
    try {
        comboModalidades();
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idGrupoCurso", _idGrupoCurso);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        idGrupoCurso = res.grupoCurso.idGrupoCurso;
        activo = res.grupoCurso.activo == 1 || res.grupoCurso.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res.grupoCurso);
        idCurso.innerHTML = `<option value='${res.grupoCurso.idCurso}'>${res.curso.curso}</option>`;
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idGrupoCurso, _switch) {
    try {
        const url = `${baseUrl}activar`;
        const data = new FormData();
        data.append("idGrupoCurso", _idGrupoCurso);
        await axios.post(url, data);
        toastSuccess(`<b>${_switch.checked ? "Activado" : "Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked = !_switch.checked;
    }
}


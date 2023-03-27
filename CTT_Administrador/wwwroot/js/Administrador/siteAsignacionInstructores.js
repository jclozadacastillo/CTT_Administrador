const baseUrl = `${_route}AsignacionInstructores/`;
const modal = new bootstrap.Modal(modalDatos, {
    keboard: false,
    backdrop: "static"
})
let idAsignacion = 0;
let activo = 1;
const mayusculas = true;
window.addEventListener("load", async function () {
    if (mayusculas) frmDatos.classList.add("to-uppercase");
    activarValidadores(frmDatos);
    loader.hidden = false;
    $(idPeriodo).select2({ dropdownParent: modalDatos });
    $(idGrupoCurso).select2({ dropdownParent: modalDatos });
    $(idInstructor).select2({ dropdownParent: modalDatos });
    $(idCurso).select2({ dropdownParent: modalDatos });
    await listar();
    await comboPeriodos();
    await comboInstructores();
    loader.hidden = true;
    content.hidden = false;
});

function nuevo() {
    idPeriodo.removeAttribute("disabled");
    idTipoCurso.removeAttribute("disabled");
    idGrupoCurso.removeAttribute("disabled");
    paralelo.removeAttribute("disabled");
    idInstructor.removeAttribute("disabled");
    idCurso.removeAttribute("disabled");
    idAsignacion = 0;
    activo = true;
    limpiarForm(frmDatos);
    modal.show();
    modalDatosLabel.innerHTML = "Nuevo registro";
}

async function listar() {
    try {
        const url = `${baseUrl}listar`;
        const res = (await axios.get(url)).data;
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    data: "idAsignacion",
                    class: "text-center td-switch",
                    render: (data, type, row) => {
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
                                              <li>
                                                <a class="dropdown-item"
                                                onclick="eliminar('${data}')">
                                                <i class='bi-trash-fill me-1 text-gray'></i>
                                                <small>ELIMINAR</small>
                                                </a>
                                             </li>
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
                                            id="check${row.idAsignacion}" ${(row.activo == true) ? "checked" : ""}
                                            onchange="activar('${row.idAsignacion}',this)"
                                            />
                                        <span class="slider"></span>
                                </label>
                                `;
                    }
                },
                {
                    title: "Documento",
                    data: "documentoIdentidad",
                    class: "w-cedula"
                },
                {
                    title: "Instructor",
                    data: "idInstructor",
                    class: "text-start w-25",
                    render: (data, type, row) => {
                        return `${row.abreviaturaTitulo.replaceAll(".", "")}. ${row.primerApellido} ${row.segundoApellido} ${row.primerNombre} ${row.segundoNombre}`;
                    }
                },
                { title: "Curso", data: "curso", class: "w-25" },
                { title: "Paralelo", data: "paralelo", class: "w-id" },
                { title: "Periodo", data: "detalle", class: "w-25" }
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
        const url = `${baseUrl}comboPeriodos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idPeriodo}'>${item.detalle}</option>`
        });
        idPeriodo.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboInstructores() {
    try {
        const url = `${baseUrl}comboInstructores`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idInstructor}'>${item.documentoIdentidad} - ${item.abreviaturaTitulo?.replaceAll(".")}. ${item.primerNombre} ${item.segundoNombre} ${item.primerApellido} ${item.segundoApellido}</option>`
        });
        idInstructor.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboTiposCursos() {
    try {
        if (idPeriodo.value == "") {
            idTipoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
            idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo de curso</option>`;
            return;
        }

        const url = `${baseUrl}comboTiposCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`
        });
        idTipoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCursos() {
    try {
        if (idTipoCurso.value == "") {
            idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo de curso</option>`;
            return;
        }

        const url = `${baseUrl}comboCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`
        });
        idGrupoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCursosAsociados() {
    try {
        if (idGrupoCurso.value == "") {
            idCurso.innerHTML = `<option value="">Seleccione un curso</option>`;
            return;
        }

        const url = `${baseUrl}comboCursosAsociados`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idCurso}'>${item.curso}</option>`
        });
        idCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function guardar() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        bloquearBotones();
        const url = `${baseUrl}guardar`;
        if (idAsignacion > 0) {
            idPeriodo.removeAttribute("disabled");
            idTipoCurso.removeAttribute("disabled");
            idGrupoCurso.removeAttribute("disabled");
            paralelo.removeAttribute("disabled");
            idInstructor.removeAttribute("disabled");
            idCurso.removeAttribute("disabled");
        }
        if (mayusculas) formToUpperCase(frmDatos);
        const data = new FormData(frmDatos);
        data.append("idAsignacion", idAsignacion);
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

async function editar(_idAsignacion) {
    try {
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idAsignacion", _idAsignacion);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        idAsignacion = res.idAsignacion;
        activo = res.activo == 1 || res.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res);
        setTimeout(() => {
            $(idTipoCurso).val(res.idTipoCurso).trigger("change");
        }, 109);
        setTimeout(() => {
            $(idGrupoCurso).val(res.idGrupoCurso).trigger("change");
        }, 190);
        setTimeout(() => {
            $(idCurso).val(res.idCurso).trigger("change");
            idPeriodo.disabled = true;
            idTipoCurso.disabled = true;
            idGrupoCurso.disabled = true;
            paralelo.disabled = true;
            idInstructor.disabled = true;
            idCurso.disabled = true;
        }, 216);
        modal.show();
    } catch (e) {
        handleError(e);
    }
}
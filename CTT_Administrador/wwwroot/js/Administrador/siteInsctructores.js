const baseUrl = `${_route}Instructores/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idInstructor = 0;
let activo = 1;
const mayusculas = true;
window.addEventListener("load", async function () {
    if (mayusculas) frmDatos.classList.add("to-uppercase");
    activarValidadores(frmDatos);
    loader.hidden = false;
    await comboTiposDocumentos();
    await listar();
    loader.hidden = true;
    content.hidden = false;
});

async function comboTiposDocumentos() {
    try {
        const url = `${baseUrl}comboTiposDocumentos`;
        const res = (await axios.get(url)).data;
        let html = ``;
        res.forEach(item => {
            html += `<option value='${item.idTipoDocumento}' data-cedula='${item.esCedula}'>${item.tipo}</option>`;
        });
        tipoDocumento.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
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
                    data: "idInstructor",
                    class: "text-center w-id",
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
                                            id="check${row.idInstructor}" ${(row.activo == true) ? "checked" : ""}
                                            onchange="activar('${row.idInstructor}',this)"
                                            />
                                        <span class="slider"></span>
                                </label>
                                `;
                    }
                },
                { title: "Documento", data: "documentoIdentidad", class: "w-cedula" },
                { title: "Primer Apellido", data: "primerApellido", class: "w-25" },
                { title: "Segundo Apellido", data: "segundoApellido", class: "w-25" },
                { title: "Primer Nombre", data: "primerNombre", class: "w-25" },
                { title: "Segundo Nombre", data: "segundoNombre", class: "w-25" },

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

function nuevo() {
    idInstructor = 0;
    activo = true;
    limpiarForm(frmDatos);
    modal.show();
    modalDatosLabel.innerHTML = "Nuevo registro";
}

async function guardar() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        bloquearBotones();
        const url = `${baseUrl}guardar`;
        if (mayusculas) formToUpperCase(frmDatos);
        const data = new FormData(frmDatos);
        data.append("idInstructor", idInstructor);
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

async function editar(_idInstructor) {
    try {
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idInstructor", _idInstructor);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        idInstructor = res.idInstructor;
        activo = res.activo == 1 || res.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res);
        handleTipoDocumento();
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idInstructor, _switch) {
    try {
        const url = `${baseUrl}activar`;
        const data = new FormData();
        data.append("idInstructor", _idInstructor);
        await axios.post(url, data);
        toastSuccess(`<b>${_switch.checked ? "Activado" : "Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked = !_switch.checked;
    }
}

function handleTipoDocumento() {
    documentoIdentidad.removeAttribute("data-validate");
    limpiarValidadores(documentoIdentidad.closest("div"));
    if (tipoDocumento.options[tipoDocumento.selectedIndex].dataset.cedula == "1") {
        documentoIdentidad.setAttribute("data-validate", "cedula");
        validarCedula(documentoIdentidad);
    }
    activarValidadores(frmDatos);
}
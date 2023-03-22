const baseUrl = `${_route}TiposCursos/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idTipoCurso = 0;
let activo = 1;
window.addEventListener("load", async function () {
    loader.hidden = false;
    await listar();
    loader.hidden = true;
    content.hidden = false;
});
async function listar() {
    try {
        const url = `${baseUrl}listar`;
        const res = (await axios.get(url)).data;
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    data: "idTipoCurso",
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
                                            id="check${row.idTipoCurso}" ${(row.activo == true) ? "checked" : ""}
                                            onchange="activar('${row.idTipoCurso}',this)"
                                            />
                                        <span class="slider"></span>
                                </label>
                                `;
                    }
                },
                {
                    title: "Tipos de Cursos",
                    data: "tipoCurso",
                    class: "w-100"
                }
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
    idTipoCurso = 0;
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
        const data = new FormData(frmDatos);
        data.append("idTipoCurso", idTipoCurso);
        data.append("activo", activo);
        if (esCurso.checked) data.append("esCurso", 1);
        if (esDiplomado.checked) data.append("esDiplomado", 1);
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

async function editar(_idTipoCurso) {
    try {
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idTipoCurso", _idTipoCurso);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        idTipoCurso = res.idTipoCurso;
        activo = res.activo == 1 || res.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res);
        if (res.esCurso) esCurso.checked = true;
        if (res.esDiplomado) esDiplomado.checked = true;
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idTipoCurso, _switch) {
    try {
        const url = `${baseUrl}activar`;
        const data = new FormData();
        data.append("idTipoCurso", _idTipoCurso);
        await axios.post(url, data);
        toastSuccess(`<b>${_switch.checked ? "Activado" : "Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked = !_switch.checked;
    }
}
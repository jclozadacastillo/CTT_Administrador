const baseUrl = `${_route}Cursos/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idCurso = 0;
let activo = 1;
window.addEventListener("load", async function(){
    modal.show();
    loader.hidden=false;
    $(idCategoria).select2({ dropdownParent: $('#modalDatos') });
    $(idTipoCurso).select2({ dropdownParent: $('#modalDatos') });
    activarValidadores(frmDatos);
    await comboCategorias();
    await comboTiposCursos();
    await listar();
    loader.hidden=true;
    content.hidden=false;
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
                {title: "Curso",data: "curso",class: "w-50"},
                {title: "Categoría",data: "categoria",class: "w-25"},
                {title: "Tipo",data: "tipoCurso",class: "w-25"},

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

async function nuevo() {
    idCurso = 0;
    activo = true;
    limpiarForm(frmDatos);
    modal.show();
    modalDatosLabel.innerHTML = "Nuevo registro";
}

async function comboCategorias(){
    try {
        const url=`${baseUrl}comboCategorias`;
        const res=(await axios.get(url)).data;
        let html="<option value=''>Seleccione</option>";
        res.forEach(item => {
            html+=`<option value='${item.idCategoria}'>${item.categoria}</option>`;
        });
        idCategoria.innerHTML=html;
    } catch (e) {
        handleError(e);
    }
}

async function comboTiposCursos(){
    try {
        const url=`${baseUrl}comboTiposCursos`;
        const res=(await axios.get(url)).data;
        let html="<option value=''>Seleccione</option>";
        res.forEach(item => {
            html+=`<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML=html;
    } catch (e) {
        handleError(e);
    }
}


async function guardar() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        bloquearBotones();
        const url = `${baseUrl}guardar`;
        const data = new FormData(frmDatos);
        data.append("idCurso", idCurso);
        data.append("activo", activo);
        await axios.post(url, data);
        toastSuccess("<b>Guardado</b> con éxito");
        modal.hide();
        listar();
    } catch (e) {
        handleError(e);
    }finally{
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
        idCurso = res.idCurso;
        activo = res.activo == 1 || res.activo == true ? 1 : 0;
        cargarFormularioInForm(frmDatos, res);
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idCurso,_switch){
    try {
        const url=`${baseUrl}activar`;
        const data=new FormData();
        data.append("idCurso",_idCurso);
        await axios.post(url,data);
        toastSuccess(`<b>${_switch.checked?"Activado":"Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked=!_switch.checked;
    }
}
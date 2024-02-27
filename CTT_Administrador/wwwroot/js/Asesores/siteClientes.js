const baseUrl = `${_route}Clientes/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idCliente = 0;
let activo = 1;
const mayusculas = true;
(async function () {
    if (mayusculas) frmDatos.classList.add("to-uppercase");
    activarValidadores(frmDatos);
    loader.hidden = false;
    await comboTiposDocumentos();
    await listar();
    loader.hidden = true;
    content.hidden = false;
})();

async function comboTiposDocumentos() {
    try {
        const url = `${baseUrl}comboTiposDocumentos`;
        const res = (await axios.get(url)).data;
        let html = ``;
        res.forEach(item => {
            html += `<option value='${item.idTipoDocumento}' data-cedula='${item.esCedula}'>${item.tipo}</option>`;
        });
        idTipoDocumento.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function listar() {
    try {
        const url = `${baseUrl}listar`;
        await $(tableDatos).DataTable({
            bDestroy: true,
            serverSide: true,
            processing:false,
            ajax: async function (_data, resolve) {
                try {
                    const res = (await axios.post(url, _data,{
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })).data;
                    resolve(res);
                } catch (e) {
                    handleError(e);
                    resolve([]);
                }
            },
            columns: [
                {
                    title:'<i class="bi-gear"></i>',
                    data: "idCliente",
                    class: "text-center w-id",
                    render: (data, type, row) => {
                        return `<button class='btn-option-table text-primary' onclick='editar(${data})'><i class='bi-pencil-square'></i></button>`
                    }
                },
                { title: "Documento", data: "documento", class: "w-cedula" },
                { title: "Cliente", data: "nombre",class:"text-nowrap" },
                {
                    title: "Teléfono",
                    data: "telefono",
                    class:"text-nowrap",
                    render: (data) => {
                        return data||"<small class='text-muted'>SIN REGISTRO</small>"
                    }
                },
                {
                    title: "Email",
                    data: "email",
                    class: "w-50 text-nowrap",
                    render: (data) => {
                        return data || "<small class='text-muted'>SIN REGISTRO</small>"
                    }
                }
            ],
            columnDefs: [
                { targets: [0, 1], orderable: false }
            ],
            order: [[2, "ASC"]],
        })
    } catch (e) {
        handleError(e);
    }
}
function reloadDataTable() {
    setTimeout(function(){$(tableDatos).DataTable().ajax.reload();}, 100);
}
function nuevo() {
    idCliente = 0;
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
        data.append("idCliente", idCliente);
        await axios.post(url, data);
        toastSuccess("<b>Guardado</b> con éxito");
        modal.hide();
        reloadDataTable();
    } catch (e) {
        handleError(e);
    } finally {
        desbloquearBotones();
    }
}

async function editar(_idCliente) {
    try {
        const url = `${baseUrl}unDato`;
        const data = new FormData();
        data.append("idCliente", _idCliente);
        const res = (await axios.post(url, data)).data;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = "Editar registro";
        idCliente = res.idCliente;
        cargarFormularioInForm(frmDatos, res);
        handleidTipoDocumento();
        modal.show();
    } catch (e) {
        handleError(e);
    }
}
function handleidTipoDocumento() {
    documento.removeAttribute("data-validate");
    limpiarValidadores(documento.closest("div"));
    if (idTipoDocumento.options[idTipoDocumento.selectedIndex]?.dataset.cedula == "1") {
        documento.setAttribute("data-validate", "cedula");
        validarCedula(documento);
    }
    activarValidadores(frmDatos);
}
const baseUrl = `${_route}Asesores/MatriculasInHouse/`;
const idTipoCursoSession = sessionStorage.getItem("idTipoCurso");
const idPeriodoSession = sessionStorage.getItem("idPeriodo");
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
});
const modalDetalleMatricula = new bootstrap.Modal(modalDetalle, {
    keyboard: false,
    backdrop: "static"
});
let valorPago = 0.0;
let modulosLista = [];
let modulosSeleccionados = "";
let idGrupoInHouse = 0;
let listaParticipantes = [];
const btnTemplate = `<button type="button" class="btn btn-sm btn-primary btn-new mt-lg-4 mt-2" title="Nuevo registro de matricula" onclick="nuevo()">
                <i class="bi-plus"></i> Nueva Matricula
            </button>`;

window.addEventListener("load", async function () {
    loaderShow();
    $(idPeriodo).select2();
    $(idTipoCurso).select2();
    $(idGrupoCurso).select2({
        dropdownParent: modalDatos
    })
    await listarPeriodos();
    await listarTiposDescuentos();
    await listarTiposCursos();
    loaderHide();
});

async function listarPeriodos() {
    try {
        const url = `${baseUrl}listarPeriodos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>SELECCIONE</option>";
        res.forEach(item => {
            html += `<option value='${item.idPeriodo}' ${item.idPeriodo == idPeriodoSession ? 'selected' : ''}>${item.detalle}</option>`;
        });
        idPeriodo.innerHTML = html;
        handlePreload();
    } catch (e) {
        handleError(e);
    }
}

async function listarTiposDescuentos() {
    try {
        const url = `${baseUrl}listarTiposDescuentos`;
        const res = (await axios.get(url)).data;
        let html = "";
        res.forEach(item => {
            html += `<option value='${item.idTipoDescuento}'>${item.nombreDescuento} - ${item.porcentaje}%</option>`;
        });
        idTipoDescuento.innerHTML = html;
        handlePreload();
    } catch (e) {
        handleError(e);
    }
}
async function listarTiposCursos() {
    try {
        const url = `${baseUrl}listarTiposCursos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>SELECCIONE</option>";

        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}' ${item.idTipoCurso == idTipoCursoSession ? 'selected' : ''}>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML = html;
        handlePreload();
    } catch (e) {
        handleError(e);
    }
}
function handlePreload() {
    if (!!idPeriodoSession || !!idTipoCursoSession) listar();
}
async function listar() {
    try {
        sessionStorage.setItem("idPeriodo", idPeriodo.value);
        sessionStorage.setItem("idTipoCurso", idTipoCurso.value);
        containerBtn.innerHTML = !!idPeriodo.value && !!idTipoCurso.value ? btnTemplate : "";
        const url = `${baseUrl}listar`;
        const params = JSON.stringify(await formToJsonTypes(frmFiltros));
        await $(tableDatos).DataTable({
            bDestroy: true,
            serverSide: true,
            processing: false,
            ajax: async function (_data, resolve) {
                try {
                    _data.parametros = params;
                    const res = (await axios.post(url, _data, {
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
                    title: "<i class='bi-gear'></i>",
                    data: "idGrupoInHouse",
                    class: "text-center w-btn",
                    render: (data, type, row) => {
                        return `<button class='btn-option-table text-info' title='detalle de matricula' onclick='verDetalle(${data})'><i class='bi-file-earmark-text-fill'></i></button>`
                    }
                },
                {
                    title: "Fecha",
                    data: "idGrupoInHouse",
                    class: 'w-fecha',
                    render: (data, type, row) => row.fechaRegistro
                },
                { title: "Documento", data: "documento", class: "w-cedula" },
                { title: "Estudiante", data: "nombre", class: "text-nowrap" },
                { title: capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso"), data: "curso", class: "w-50" },
                { title: "Estudiantes", data: "estudiantes", class: "w-fecha" },
                //{
                //    title: "Deuda",
                //    class: "text-end",
                //    data: "deuda",
                //    render: data => {
                //        return data > 0 ? `<span class='text-danger'>${data.toFixed(2)}</span>` : `<span class='text-success'>0.00</span>`;
                //    }
                //}
            ],
            columnDefs: [
                { targets: [0], orderable: false }
            ],
            order: [[1, "DESC"]],
        });
    } catch (e) {
        handleError(e);
    }
}

function reloadDataTable() {
    setTimeout(function () { $(tableDatos).DataTable().ajax.reload(); }, 100);
}

async function nuevo() {
    listaParticipantes = [];
    llenarTablaParticipantes();
    modalDatosLabel.innerText = `NUEVA MATRICULA IN-HOUSE DE ${idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "CURSO"}`;
    labelGrupoCurso.innerText = capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso");
    await listarCursos();
    if (!!$(idCliente).data("select2")) {
        idCliente.innerHTML = "";
        $(idCliente).select2("destroy");
    }
    $(idCliente).select2({
        dropdownParent: modalDatos,
        ajax: {
            delay: 370,
            url: `${baseUrl}listarClientes`,
            data: function (params) {
                var query = {
                    search: params.term
                }
                return query;
            },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        cache: true,
        placeholder: 'SELECCIONE',
        minimumInputLength: 1
    },
    );
    activarValidadores(frmDatos);
    limpiarForm(frmDatos);
}

function handleCursos() {
    if (!!idCliente.value) {
        gruposcursos.removeAttribute("hidden");
    } else {
        gruposcursos.hidden = true;
        modulos.hidden = true;
    }
}

async function listarCursos() {
    try {
        const url = `${baseUrl}listarCursos`;
        const data = new FormData(frmFiltros);
        const res = (await axios.post(url, data)).data;
        let html = `<option value=''>SELECCIONE</option>`;
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`;
        });
        idGrupoCurso.innerHTML = html;
        if (res.length > 0) {
            modal.show()
        } else {
            toastWarning("No existe ninguna oferta académica que pueda generar matriculas para ese periodo y ese tipo de curso");
        }
    } catch (e) {
        handleError(e);
    }
}
async function cargarModulos() {
    try {
        if (!idGrupoCurso.value) {
            modulos.hidden = true;
            participantes.hidden = true;
            participantesTabla.hidden = true;
        } else {
            modulos.removeAttribute("hidden");
            participantes.removeAttribute("hidden");
            participantesTabla.removeAttribute("hidden");
            grupoCursoLabel.innerHTML = idGrupoCurso.options[idGrupoCurso.selectedIndex].text?.toLowerCase();
        }
        modulosLista = [];
        idGrupoInHouse = 0;
        const url = `${baseUrl}cargarModulos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "";
        modulosSeleccionados = "";
        modulosLista = res.listaModulos;
        idGrupoInHouse = res.idGrupoInHouse;
        res.listaModulos.forEach(item => {
            html += `
                    <label class="form-check ms-2 my-1">
                    <input class="form-check-input" name='idCurso' type="checkbox" data-id-curso="${item.idCurso}" checked onchange='calcularTotal()'>
                    <span class="form-check-label">
                      ${item.curso}
                    </span>
                  </label>
                    `;
        });
        idCurso.innerHTML = html;
        calcularTotal();
    } catch (e) {
        handleError(e);
    }
}

function calcularTotal() {
    try {
        valorPago = 0;
        modulosSeleccionados = "";
        idCurso.querySelectorAll("input:checked").forEach(item => {
            const moduloObjeto = modulosLista.find(x => x.idCurso == item.dataset.idCurso);
            valorPago += parseFloat(moduloObjeto.valor);
            modulosSeleccionados += modulosSeleccionados == "" ? `${moduloObjeto.idCurso}` : `,${moduloObjeto.idCurso}`;
        });
    } catch (e) {
        console.warn(e);
    }
}

archivoParticipantes.addEventListener("change", async function () {
    try {
        listaParticipantes = [];
        if (!this.value) return;
        const file = this.files[0];
        if (file.type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") throw new Error("El archivo debe ser el de la plantilla");
        const url = `${baseUrl}leerExcel`;
        const data = new FormData(frmDatos);
        listaParticipantes = (await axios.post(url, data)).data;
        llenarTablaParticipantes();
    } catch (e) {
        handleError(e);
        this.value = "";
    }
});

function llenarTablaParticipantes() {
    try {
        $(tableParticipantes).DataTable({
            bDestroy: true,
            data: listaParticipantes,
            columns: [
                { title: "Documento", data: "documentoIdentidad" },
                { title: "Primer Apellido", data: "primerApellido" },
                { title: "Segundo Apellido", data: "segundoApellido" },
                { title: "Primer Nombre", data: "primerNombre" },
                { title: "Segundo Nombre", data: "segundoNombre" },
                { title: "Paralelo", data: "paralelo" }
            ]
        })
    } catch (e) {
        handleError(e);
    }
}

async function generarMatricula() {
    try {
        if (! await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        if (idCurso.querySelectorAll("input:checked").length == 0) throw new Error("Debe seleccionar al menos un modulo");
        if (listaParticipantes.length == 0) throw new Error("Al menos debe existir un participante");
        if (!await toastPreguntar(`
        <div class='text-center'><i class='fs-lg bi-exclamation-circle-fill text-info fs-2 text-center'></i></div>
        <div class='alert alert-secondary fs-sm'>
            ¿Está seguro que desea continuar con <b>${listaParticipantes.length}</b> matriculas?
        </div>
        <div class='fs-sm text-danger'>
        <i class='bi-exclamation-triangle-fill me-2'></i> Una vez generadas las matriculas no se podrán modificar.
        </div>
        `)) return;
        const url = `${baseUrl}guardarMatriculas`;
        const data = new FormData(frmDatos);
        data.append("modulos", modulosSeleccionados);
        data.append("participantes", JSON.stringify(listaParticipantes));
        loaderShow();
        const res = (await axios.post(url, data)).data
        loaderHide();
        await toastPromise(`
        <div class='alert alert-success fs-sm text-start'>
        Matriculas procesadas exitosamente, las matriculas se encuentran legalizadas.
        </div>`);
        reloadDataTable();
        modal.hide();
    } catch (e) {
        console.error(e);
        handleError(e);
    } finally {
        loaderHide();
    }
}

async function verDetalle(_idGrupoInHouse) {
    try {
        const url = `${baseUrl}detalleMatriculas/${_idGrupoInHouse}`;
        const res = (await axios.get(url)).data;
        console.log(res);
        //const matricula = res.matricula;
        //const modulos = res.modulos;
        //const pagos = res.pagos;
        //modalDetalleLabel.innerText = `MATRICULA #${matricula.idGrupoInHouse.toString().padStart(6, "0")}`;
        //deudaDetalleMatricula.innerHTML = `<b class='text-${matricula.deuda > 0 ? 'danger' : 'success'}'>$${matricula.deuda.toFixed(2)}</b>`;
        //documentoIdentidadDetalleMatricula.innerHTML = matricula.documentoIdentidad;
        //estudianteDetalleMatricula.innerHTML = matricula.estudiante;
        //cursoDetalleMatricula.innerHTML = matricula.curso;
        //tipoCursoDetalleMatricula.innerHTML = matricula.tipoCurso;
        //if (matricula.esDiplomado == 1) {
        //    let html = "";
        //    detalleMatriculaModulos.removeAttribute("hidden");
        //    html = `<div class='col-sm-12'><label class='fs-sm mt-3 fw-bold'>MÓDULOS</label></div>
        //            <div class='table-responsive'>
        //            <table class='fs-sm table table-striped w-100'>
        //            <thead class='bg-primary text-white'>
        //                <tr>
        //                    <th>FECHA MATRICULA</th>
        //                    <th>MÓDULO</th>
        //                </tr>
        //            </thead>
        //            `;
        //    modulos.forEach(item => {
        //        html += `<tr>
        //                    <td>${item.fechaRegistro.replaceAll("T", " ").substring(0, item.fechaRegistro.length - 3)}</td>
        //                    <td>${item.curso}</td>
        //                </tr>`
        //    });
        //    detalleMatriculaModulos.innerHTML = html + "</div></table>";
        //} else {
        //    detalleMatriculaModulos.innerHTML = "";
        //    detalleMatriculaModulos.hidden = true;
        //}
        //html = "";
        //pagos.forEach(item => {
        //    html += `<tr>
        //                <td class='w-btn'>
        //                    <a class='btn-option-table text-primary' target='_blank' href='${_route}${item.imagenComprobante}?v=${(new Date()).getTime()}'><i class='bi-receipt'></i></a>
        //                </td>
        //                <td>${item.fechaPago.substring(0, 10)}</td>
        //                <td>${item.numero} - ${item.banco}</td>
        //                <td>${item.numeroComprobante}</td>
        //                <td>${item.formaPago}</td>
        //                <td class='text-end'>${item.valor.toFixed(2)}</td>
        //            </tr>`
        //});
        //detallePagosMatricula.innerHTML = html;
        modalDetalleMatricula.show();
    } catch (e) {
        handleError(e);
    }
}
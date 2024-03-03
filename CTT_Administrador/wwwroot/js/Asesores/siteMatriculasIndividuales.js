const baseUrl = `${_route}Asesores/MatriculasIndividuales/`;
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
let idMatricula = 0;
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
    await listarTiposCursos();
    await comboFormasPagos();
    await comboCuentas();
    await comboTiposDocumentos();
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
                    data: "idMatricula",
                    class: "text-center w-btn",
                    render: (data, type, row) => {
                        return `<button class='btn-option-table text-info' title='detalle de matricula' onclick='verDetalle(${data})'><i class='bi-file-earmark-text-fill'></i></button>`
                    }
                },
                {
                    title: "Fecha",
                    data: "idMatricula",
                    class: 'w-fecha',
                    render: (data, type, row) => row.fechaRegistro
                },
                { title: "Documento", data: "documentoIdentidad", class: "w-cedula" },
                { title: "Estudiante", data: "estudiante", class: "text-nowrap" },
                { title: capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso"), data: "curso", class: "w-50" },
                { title: "Paralelo", data: "paralelo", class: "w-fecha" },
                {
                    title: "Deuda",
                    class: "text-end",
                    data: "deuda",
                    render: data => {
                        return data > 0 ? `<span class='text-danger'>${data.toFixed(2)}</span>` : `<span class='text-success'>0.00</span>`;
                    }
                }
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
    modalDatosLabel.innerText = `NUEVA MATRICULA DE ${idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "CURSO"}`;
    labelGrupoCurso.innerText = capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso");
    await listarCursos();
    if (!!$(idEstudiante).data("select2")) {
        idEstudiante.innerHTML = "";
        $(idEstudiante).select2("destroy");
    }
    $(idEstudiante).select2({
        dropdownParent: modalDatos,
        ajax: {
            delay: 370,
            url: `${baseUrl}listarEstudiantes`,
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
    if (!!idEstudiante.value) {
        gruposcursos.removeAttribute("hidden");
    } else {
        gruposcursos.hidden = true;
        modulos.hidden = true;
        pagos.hidden = true;
        facturacion.hidden = true;
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
            pagos.hidden = true;
            facturacion.hidden = true;
        } else {
            modulos.removeAttribute("hidden");
            pagos.removeAttribute("hidden");
            facturacion.removeAttribute("hidden");
            grupoCursoLabel.innerHTML = idGrupoCurso.options[idGrupoCurso.selectedIndex].text?.toLowerCase();
        }
        modulosLista = [];
        idMatricula = 0;
        const url = `${baseUrl}cargarModulos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "";
        modulosSeleccionados = "";
        modulosLista = res.listaModulos;
        idMatricula = res.idMatricula;
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
        costo.innerHTML = `$${valorPago.toFixed(2)}`;
        valor.value = valorPago.toFixed(2).replaceAll(".", ",");
    } catch (e) {
        console.warn(e);
    }
}

async function comboFormasPagos() {
    try {
        const url = `${baseUrl}comboFormasPagos`;
        const res = (await axios.get(url)).data;
        let html = `<option value=''>Seleccione</option>`;
        res.forEach(item => {
            html += `<option data-tc="${item.codigo_financiero}" value='${item.idFormaPago}'>${item.formaPago}</option>`;
        });
        idFormaPago.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCuentas() {
    try {
        const url = `${baseUrl}comboCuentas`;
        const res = (await axios.get(url)).data;
        let html = `<option value=''>Seleccione</option>`;
        res.forEach(item => {
            html += `<option data-cedula="${item.esCedula}" value='${item.idCuenta}'>${item.alias}</option>`;
        });
        idCuenta.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboTiposDocumentos() {
    try {
        const url = `${baseUrl}comboTiposDocumentos`;
        const res = (await axios.get(url)).data;
        let html = ``;
        res.forEach(item => {
            html += `<option value='${item.idTipoDocumento}'>${item.tipo}</option>`;
        });
        idTipoDocumento.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

idFormaPago.addEventListener("change", function () {
    if (this.options[this.selectedIndex].dataset.tc?.toLowerCase() == "tc") {
        datosTarjeta.removeAttribute("hidden");
        datosTarjeta.querySelectorAll("input,select").forEach(item => {
            item.removeAttribute("data-validate");
        });
        limpiarForm(datosTarjeta);
        activarValidadores(datosTarjeta);
    } else {
        datosTarjeta.hidden = true;
        datosTarjeta.querySelectorAll("input,select").forEach(item => {
            item.setAttribute("data-validate", "no-validate");
        });
        limpiarForm(datosTarjeta);
        activarValidadores(datosTarjeta);
    }
});

archivoComprobante.addEventListener("change", function () {
    try {
        const formatosValidos = [
            "application/pdf",
            "image/png",
            "image/jpg",
            "image/jpeg"
        ];
        if (!this.value) return;
        if (formatosValidos.indexOf(this.files[0].type) < 0) throw new Error("El archivo seleccionado no tiene el formato permitido.");
        if ((this.files[0].size / 1024 / 1024) > 3) throw new Error("El archivo no debe pesar mas de 3Mb.");
    } catch (e) {
        handleError(e);
        this.value = "";
    }
});

documento.addEventListener("focusout", function () {
    if (!!this.value) cargarDatosCliente();
});

async function cargarDatosCliente() {
    try {
        const url = `${baseUrl}datosCliente`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        if (!!res) cargarFormularioInForm(facturacion, res);
    } catch (e) {
        console.warn(e);
    }
}
function handleTipoDocumento() {
    if (idTipoDocumento.options[idTipoDocumento.selectedIndex].dataset.cedula == 1) {
        documento.setAttribute("data-validate", "cedula");
        activarValidadores(documento.closest("div"));
        validarCedula(documento);
    } else {
        documento.removeAttribute("data-validate");
        activarValidadores(documento.closest("div"));
        validarVacio(documento);
    }
}

function handleBancos() {
    const config = {
        "tc": { label: "autorización", verNumeroComprobante: false, verBanco: false },
        "tr": { label: "comprobante", verNumeroComprobante: true, verBanco: true },
        "dp": { label: "depósito o transacción", verNumeroComprobante: true, verBanco: true }
    }
    const op = idFormaPago.options[idFormaPago.selectedIndex].dataset.tc?.toLowerCase();
    labelComprobante.innerHTML = config[op].label;
    config[op].verNumeroComprobante ? numeroComprobante.closest("div").removeAttribute("hidden") : numeroComprobante.closest("div").hidden = true;
    if (config[op].verBanco) {
        idCuenta.closest("div").removeAttribute("hidden");
        idCuenta.removeAttribute("data-validate");
    } else {
        idCuenta.closest("div").hidden = true;
        limpiarValidadores(idCuenta.closest("div"));
        idCuenta.setAttribute("data-validate", "no-validate");
    }
    activarValidadores(idCuenta.closest("div"));
}

async function generarMatricula() {
    try {
        if (idFormaPago.options[idFormaPago.selectedIndex].dataset.tc?.toLowerCase() == "tc") {
            numeroComprobante.value = tajetaAutorizacion.value;
        }
        if (! await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        if (idCurso.querySelectorAll("input:checked").length == 0) throw new Error("Debe seleccionar al menos un modulo");
        if (parseFloat(valor.value.replaceAll(",", ".")) == 0) throw new Error("No se puede generar una matricula sin un pago inicial.");
        if (parseFloat(parseFloat(valor.value.replaceAll(",", ".")).toFixed(2)) > parseFloat(valorPago.toFixed(2))) throw new Error("Su pago no puede ser mayor al valor a cancelar.");
        if (!await toastPreguntar(`
        <div class='text-center'><i class='fs-lg bi-exclamation-circle-fill text-info fs-2 text-center'></i></div>
        <div class='alert alert-secondary fs-sm'>
            ¿Está seguro que desea continuar?
        </div>
        <div class='fs-sm text-danger'>
        <i class='bi-exclamation-triangle-fill me-2'></i> No podrá cambiar los datos de facturación una vez finalizado el proceso
        </div>
        `)) return;
        const url = `${baseUrl}generarMatricula`;
        formToUpperCase(frmDatos);
        const data = new FormData(frmDatos);
        data.append("modulos", modulosSeleccionados);
        data.append("idMatricula", idMatricula);
        data.append("deuda", parseFloat(valorPago) - parseFloat(valor.value.replaceAll(",", ".")));
        loaderShow();
        const res = (await axios.post(url, data)).data
        loaderHide();
        await toastPromise(`
        <div class='alert alert-success fs-sm text-start'>
        Matricula procesada exitosamente, la matricula se legalizará automáticamente cuando el estudiante no tenga deudas pendientes.
        </div>`);
        reloadDataTable();
        modal.hide();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}

async function verDetalle(_idMatricula) {
    try {
        const url = `${baseUrl}detalleMatricula/${_idMatricula}`;
        const res = (await axios.get(url)).data;
        const matricula = res.matricula;
        const modulos = res.modulos;
        const pagos = res.pagos;
        modalDetalleLabel.innerText = `MATRICULA #${matricula.idMatricula.toString().padStart(6, "0")}`;
        deudaDetalleMatricula.innerHTML = `<b class='text-${matricula.deuda > 0 ? 'danger' : 'success'}'>$${matricula.deuda.toFixed(2)}</b>`;
        estudianteDetalleMatricula.innerHTML = matricula.estudiante;
        cursoDetalleMatricula.innerHTML = matricula.curso;
        tipoCursoDetalleMatricula.innerHTML = matricula.tipoCurso;
        if (matricula.esDiplomado == 1) {
            let html = "";
            detalleMatriculaModulos.removeAttribute("hidden");
            html = `<div class='col-sm-12'><label class='fs-sm mt-3 fw-bold'>MÓDULOS</label></div>
                    <div class='table-responsive'>
                    <table class='fs-sm table table-striped w-100'>
                    <thead class='bg-primary text-white'>
                        <tr>
                            <th>FECHA MATRICULA</th>
                            <th>MÓDULO</th>
                        </tr>
                    </thead>
                    `;
            modulos.forEach(item => {
                html += `<tr>
                            <td>${item.fechaRegistro.replaceAll("T", " ").substring(0, item.fechaRegistro.length-3)}</td>
                            <td>${item.curso}</td>
                        </tr>`
            });
            detalleMatriculaModulos.innerHTML = html+"</div></table>";
        } else {
            detalleMatriculaModulos.innerHTML = "";
            detalleMatriculaModulos.hidden = true;
        }       
        html = "";
        pagos.forEach(item => {
            html += `<tr>
                        <td class='w-btn'>
                            <a class='btn-option-table text-danger' target='_blank' href='${_route}${item.imagenComprobante}?v=${(new Date()).getTime()}'><i class='bi-file-pdf-fill'></i></a>
                        </td>
                        <td>${item.fechaPago.substring(0,10)}</td>
                        <td>${item.numero} - ${item.banco}</td>
                        <td>${item.numeroComprobante}</td>
                        <td>${item.formaPago}</td>
                        <td class='text-end'>${item.valor.toFixed(2)}</td>
                    </tr>`
        });
        detallePagosMatricula.innerHTML = html;
        modalDetalleMatricula.show();
    } catch (e) {
        handleError(e);
    }

}
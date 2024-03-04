const baseUrl = `${_route}Asesores/Recaudacion/`;
const modalDetalleMatricula = new bootstrap.Modal(modalDetalle, {
    keyboard: false,
    backdrop: "static"
});
let deuda = 0.0;
let idMatricula = 0;
window.addEventListener("load", async function () {
    await comboFormasPagos();
    await comboCuentas();
    await listar();
});

async function listar() {
    try {
        const url = `${baseUrl}listar`;
        await $(tableDatos).DataTable({
            bDestroy: true,
            serverSide: true,
            processing: false,
            ajax: async function (_data, resolve) {
                try {
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
                        return `<button class='btn-option-table text-info' title='detalle de matricula' onclick='verDetalle(${data},true)'><i class='bi-file-earmark-text-fill'></i></button>`
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
                { title: "Curso/Diplomado", data: "curso", class: "w-50" },
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

async function verDetalle(_idMatricula, verModal) {
    try {
        limpiarForm(frmDatos);
        datosTarjeta.hidden = true;
        activarValidadores(frmDatos);
        if ($.fn.DataTable.isDataTable('#tablePagos')) {
            $('#tablePagos').DataTable().destroy();
        }
        const url = `${baseUrl}detalleMatricula/${_idMatricula}`;
        const res = (await axios.get(url)).data;
        const matricula = res.matricula;
        const modulos = res.modulos;
        const pagos = res.pagos;
        idMatricula = matricula.idMatricula;
        modalDetalleLabel.innerText = `MATRICULA #${matricula.idMatricula.toString().padStart(6, "0")}`;
        deudaDetalleMatricula.innerHTML = `<b class='text-${matricula.deuda > 0 ? 'danger' : 'success'}'>$${matricula.deuda.toFixed(2)}</b>`;
        deuda = parseFloat(matricula.deuda.toFixed(2));
        documentoIdentidadDetalleMatricula.innerHTML = matricula.documentoIdentidad;
        estudianteDetalleMatricula.innerHTML = matricula.estudiante;
        cursoDetalleMatricula.innerHTML = matricula.curso;
        tipoCursoDetalleMatricula.innerHTML = matricula.tipoCurso;
        deuda > 0 ? divPagos.removeAttribute("hidden") : divPagos.hidden = true;
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
                            <td>${item.fechaRegistro.replaceAll("T", " ").substring(0, item.fechaRegistro.length - 3)}</td>
                            <td>${item.curso}</td>
                        </tr>`
            });
            detalleMatriculaModulos.innerHTML = html + "</div></table>";
        } else {
            detalleMatriculaModulos.innerHTML = "";
            detalleMatriculaModulos.hidden = true;
        }
        html = "";
        pagos.forEach(item => {
            html += `<tr>
                        <td class='w-btn text-center'>
                            <a class='btn-option-table text-primary' target='_blank' href='${_route}${item.imagenComprobante}?v=${(new Date()).getTime()}'><i class='bi-receipt'></i></a>
                        </td>
                        <td>${item.fechaPago.substring(0, 10)}</td>
                        <td>${item.numero} - ${item.banco}</td>
                        <td>${item.numeroComprobante}</td>
                        <td>${item.formaPago}</td>
                        <td class='text-end'>${item.valor.toFixed(2)}</td>
                    </tr>`
        });
        detallePagosMatricula.innerHTML = html;
        $(tablePagos).DataTable({
            bDestroy: true,
            columnDefs: [
                { targets: [0], orderable: false }
            ],
            order: [[1, "DESC"]],
        });
        handleValor();
        if (verModal) modalDetalleMatricula.show();
    } catch (e) {
        handleError(e);
    }
}

function handleValor() {
    valor.addEventListener("focusout", () => {
        const valorIngresado = parseFloat(parseFloat(valor.value.replaceAll(",", ".")).toFixed(2));
        if (valorIngresado > deuda) {
            toastWarning(`El valor de pago no puede ser mayor a ${deuda}`);
            valor.value = deuda.toString().replaceAll(".", ",");
        }
    });
}

async function agregarPago() {
    try {
        loaderShow();
        const url = `${baseUrl}agregarPago`;
        const data = new FormData(frmDatos);
        data.append("idMatricula", idMatricula);
        await axios.post(url, data);
        verDetalle(idMatricula, false);
        toastSuccess("Pago agregado exitosamente");
        reloadDataTable();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
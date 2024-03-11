const baseUrl = `${_route}Contadores/ValidarPagos/`;
const modalDetalleMatricula = new bootstrap.Modal(modalDetalle, {
    keyboard: false,
    backdrop: "static"
});
const modalPago = new bootstrap.Modal(modalPagoDetalle, {
    keyboard: false,
    backdrop: "static"
});
let deuda = 0.0;
let idMatricula = 0;
let pagos = [];
window.addEventListener("load", async function () {
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
                        return data > 0 ? `<span class='badge bg-danger'>${data.toFixed(2)}</span>` : `<span class='badge bg-success'>0.00</span>`;
                    }
                },
                {
                    title: "Esperando validación",
                    class: "text-end",
                    data: "pendienteValidacion",
                    render: data => {
                        return data > 0 ? `<span class='badge bg-secondary'>${data.toFixed(2)}</span>` : `<span class='badge bg-success'>0.00</span>`
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

async function verDetalle(_idMatricula, verModal) {
    try {
        if ($.fn.DataTable.isDataTable('#tablePagos')) {
            $('#tablePagos').DataTable().destroy();
        }
        const url = `${baseUrl}detalleMatricula/${_idMatricula}`;
        const res = (await axios.get(url)).data;
        const matricula = res.matricula;
        const modulos = res.modulos;
        pagos = res.pagos;
        idMatricula = matricula.idMatricula;
        modalDetalleLabel.innerText = `MATRICULA #${matricula.idMatricula.toString().padStart(6, "0")}`;
        const pendienteValidacion = matricula.pendienteValidacion || 0;
        deudaDetalleMatricula.innerHTML = `<b class='text-${pendienteValidacion > 0 ? 'dark' : 'success'}'>$${pendienteValidacion.toFixed(2)}</b>`;
        deuda = parseFloat(matricula.deuda.toFixed(2));
        documentoIdentidadDetalleMatricula.innerHTML = matricula.documentoIdentidad;
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
            const banco = !!item.banco ? `${item.numero} - ${item.banco}` : item.tarjetaMarca;
            const colorEstado = {
                "0": "text-info",
                "1": "text-success",
                "-1": "text-danger"
            };
            let opcion = item.idEstado == 1 ? `<i class='bi-check-circle-fill text-success'></i>` : `<i class='bi-x-circle-fill text-danger'></i>`;
            if (item.idEstado == 0) opcion = `<a class='btn-option-table text-primary' onclick='verPago(${item.idPagoMatricula})' href='javascript:;'><i class='bi-receipt'></i></a>`;
            html += `<tr>
                        <td class='w-btn text-center'>
                            ${opcion}
                        </td>
                        <td>${item.fechaPago.substring(0, 10)}</td>
                        <td>${banco}</td>
                        <td>${item.numeroComprobante || item.tarjetaAutorizacion}</td>
                        <td>${item.formaPago}</td>
                        <td class='text-end'>${item.valor.toFixed(2)}</td>
                        <td class='fw-bold ${colorEstado[item.idEstado]}'>${item.estado}</td>
                        <td class='${!item.observaciones ? "text-muted" : ""}'>${item.observaciones || "Sin observaciones"}</td>
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
        if (verModal) modalDetalleMatricula.show();
    } catch (e) {
        handleError(e);
    }
}

function verPago(idPago) {
    const pago = pagos.find(x => x.idPagoMatricula == idPago);
    const banco = !!pago.banco ? `${pago.numero} - ${pago.banco}` : pago.tarjetaMarca;
    modalPagoLabel.innerHTML = `Validación pago #${pago.numeroComprobante || pago.tarjetaAutorizacion}`;
    divDatosPago.innerHTML = `
    <div class='table-responsive card pb-3 mb-0'>
    <h5 class='card-title py-1 bg-primary text-white px-3'>Datos de pago</h5>
    <table class='text-uppercase fs-sm'>
        <tr>
        <td width='127px' class='fw-bold ps-3 text-nowrap'>Fecha de pago:</td>
        <td class='ps-2'>${pago.fechaPago.substring(0, 10)}</td>
        <td class='text-end pe-3'>
        <button class='btn btn-sm btn-success' onclick='validar(${pago.idPagoMatricula},1)'><i class='bi-check pe-1'></i>VALIDADO</button>
        <button class='btn btn-sm btn-danger' onclick='validar(${pago.idPagoMatricula},-1)'><i class='bi-x pe-1'></i>NO VALIDADO</button>
        </td>
        </tr>
        <tr>
        <td width='127px' class='fw-bold ps-3 text-nowrap'>Forma de pago:</td>
        <td colspan='2' class='ps-2'>${pago.formaPago}</td>
        </tr>
        <tr>
        <td width='127px' class='fw-bold ps-3 text-nowrap'>${!!pago.tarjetaAutorizacion ? "Marca de tarjeta" : "Cuenta"}:</td>
        <td colspan='2' class='ps-2'>${banco}</td>
        </tr>
        <tr>
        <td width='127px' class='fw-bold ps-3 text-nowrap'>Número de documento:</td>
        <td colspan='2' class='ps-2'>${pago.numeroComprobante || pago.tarjetaAutorizacion}</td>
        </tr>
        <tr>
        <td width='127px' class='fw-bold ps-3 text-nowrap'>Valor:</td>
        <td colspan='2' class='ps-2 fs-5'>$${pago.valor.toFixed(2)}</td>
        </tr>
    </table>
    <div class='px-2'>
        <textarea class="form-control form-control-sm" id="observaciones" name="observaciones" maxlength="200"
        placeholder="Escriba una observación de ser necesario"
         ></textarea>
    </div>
    </div>
    `
    divIframeDocumento.innerHTML = `<iframe src='${_route}${pago.imagenComprobante}?v=${(new Date()).getTime()}'
    style='width:100%;
    min-height:54.05vh'
    ></iframe>`
    modalPago.show();
}

async function validar(idPagoMatricula, idEstado) {
    try {
        if (!await toastPreguntar(`
            <div class='alert alert-${idEstado == 1 ? "success" : "danger"} text-center'>
                <h5 class='bi-exclamation-triangle-fill fs-2 text-center'></h5>
                <p class='fs-sm'>
                    ¿Está seguro que desea <b>${idEstado == 1 ? "validar" : "invalidar"}</b> el pago?
                </p>
            </div>
            <div class='alert alert-warning text-center' style='min-height:auto'>
                <div class='fs-sm py-0'>
                    <i class='bi-exclamation-circle-fill me-1'></i>Está acción no se puede deshacer
                </div>
            </div>
        `, idEstado == 1 ? "Si,El pago es válido" : "Si, El pago <b>no</b> es válido")) return;
        loaderShow();
        const url = `${baseUrl}validarPago`;
        const data = new FormData();
        data.append("idPagoMatricula", idPagoMatricula);
        data.append("idEstado", idEstado);
        data.append("idMatricula", idMatricula);
        data.append("observaciones", observaciones.value);
        await axios.post(url, data);
        validacionesPendientes();
        reloadDataTable();
        verDetalle(idMatricula, false);
        modalPago.hide();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
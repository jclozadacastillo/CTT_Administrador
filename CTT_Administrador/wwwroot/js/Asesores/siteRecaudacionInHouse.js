const baseUrl = `${_route}Asesores/RecaudacionInHouse/`;
const modalDetalleMatricula = new bootstrap.Modal(modalDetalle, {
    keyboard: false,
    backdrop: "static"
});
let deuda = 0.0;
let idGrupoInHouse = 0;
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
                    data: "idGrupoInHouse",
                    class: "text-center w-btn",
                    render: (data, type, row) => {
                        return `<button class='btn-option-table text-info' title='detalle de matricula' onclick='verDetalle(${data},true)'><i class='bi-file-earmark-text-fill'></i></button>`
                    }
                },
                {
                    title: "Fecha",
                    data: "idGrupoInHouse",
                    class: 'w-fecha',
                    render: (data, type, row) => row.fechaRegistro
                },
                { title: "Documento", data: "documento", class: "w-cedula" },
                { title: "Cliente", data: "cliente", class: "text-nowrap" },
                { title: "Curso/Diplomado", data: "curso", class: "w-50" },
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

async function verDetalle(_idGrupoInHouse, verModal) {
    try {
        limpiarForm(frmDatos);
        datosTarjeta.hidden = true;
        activarValidadores(frmDatos);
        const url = `${baseUrl}detalleMatriculas/${_idGrupoInHouse}`;
        const res = (await axios.get(url)).data;
        const info = res.info;
        const listaParticipantes = res.alumnos;
        const modulos = res.modulos;
        const pagos = res.pagos;
        modalDetalleLabel.innerText = `MATRICULA IN-HOUSE #${info.idGrupoInHouse.toString().padStart(6, "0")}`;
        const valorSinDescuento = info.valorSinDescuento;
        const valorConDescuento = parseInt(((info.valorSinDescuento) - ((info.valorSinDescuento * (info.porcentaje)) / 100)).toFixed(2));
        deuda = valorConDescuento - (info.valorPagado>0? info.valorPagado:0);
        deudaDetalleMatricula.innerHTML = `<b class='text-${deuda > 0 ? 'danger' : 'success'}'>$${deuda.toFixed(2)}</b>`;
        documentoIdentidadDetalleMatricula.innerHTML = info.documento;
        estudianteDetalleMatricula.innerHTML = info.nombre;
        cursoDetalleMatricula.innerHTML = info.curso;
        tipoCursoDetalleMatricula.innerHTML = info.tipoCurso;
        idGrupoInHouse = _idGrupoInHouse;
        if (info.esDiplomado == 1) {
            divModulos.innerHTML = "";
            let html = "<label class='fw-bold'>MODULOS</label>";
            modulos.forEach(item => {
                html += `<div><label>${item.curso}</label></div>`
            });
            divModulos.innerHTML = html;
        } else {
            divModulos.innerHTML = "";
        }
        $(tableParticipantesDetalle).DataTable({
            bDestroy: true,
            data: listaParticipantes,
            columns: [
                {
                    title: "Matricula#",
                    data: "idMatricula",
                    render: (data) => data.toString().padStart(6, "0")
                },
                { title: "Documento", data: "documentoIdentidad" },
                {
                    title: "Estudiante",
                    data: "estudiante",
                    render: data => data.trimStart()
                }
            ],
            order: [[2, 'asc']]
        });
        $(tablePagosDetalle).DataTable({
            bDestroy: true,
            data: pagos,
            columns: [
                {
                    title: "",
                    data: "imagenComprobante",
                    render: data => `<a class='btn-option-table text-primary' target='_blank' href='${_route}${data}?v=${(new Date()).getTime()}'><i class='bi-receipt'></i></a>`
                },
                {
                    title: "Fecha",
                    data: "fechaPago",
                    render: data => data.substring(0, 10)
                },
                {
                    title: "Cuenta/Tarjeta",
                    data: "numero",
                    render: (data, type, row) => {
                        return !!data ? `${data} - ${row.banco}` : row.tarjetaMarca
                    }
                },
                {
                    title: "#Documento",
                    data: "numeroComprobante",
                    render: (data, type, row) => {
                        return data || row.tarjetaAutorizacion
                    }
                },
                { title: "Tipo", data: "formaPago" },
                { title: "Valor", data: "valor" }

            ],
            order: [[2, 'asc']]
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
        data.append("idGrupoInHouse", idGrupoInHouse);
        await axios.post(url, data);
        verDetalle(idGrupoInHouse, false);
        toastSuccess("Pago agregado exitosamente");
        reloadDataTable();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
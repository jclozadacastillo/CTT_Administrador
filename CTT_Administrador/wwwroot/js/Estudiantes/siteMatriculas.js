const baseUrl = `${_route}Matriculas/`;
let valorPago = 0.0;
let modulosLista = [];
let modulosSeleccionados = "";
let idMatricula = 0;
(async function () {
    try {
        loaderShow();
        $(idTipoCurso).select2();
        $(idCategoria).select2();
        $(idGrupoCurso).select2();
        await comboTiposCursos();
        await comboFormasPagos();
        await comboCuentas();
        await comboTiposDocumentos();
    } finally {
        loaderHide();
    }
})();

async function comboTiposCursos() {
    try {
        const url = `${baseUrl}comboTiposCursos`;
        const res = (await axios.get(url)).data;
        let html = `<option value=''>Seleccione</option>`;
        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCategorias() {
    try {
        cursos.hidden = true;
        modulos.hidden = true;
        pagos.hidden = true;
        facturacion.hidden = true;
        if (!idTipoCurso.value) {
            categorias.hidden = true;
        } else {
            categorias.removeAttribute("hidden");
            tipoCursoLabel.innerHTML = idTipoCurso.options[idTipoCurso.selectedIndex].text?.toLowerCase();
            tipoCursoCategoria.innerHTML = idTipoCurso.options[idTipoCurso.selectedIndex].text?.toLowerCase();
        }
        const url = `${baseUrl}comboCategorias`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idCategoria}'>${item.categoria}</option>`;
        });
        idCategoria.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

idCategoria.addEventListener("change", () => comboGruposCursos());

async function comboGruposCursos() {
    try {
        modulos.hidden = true;
        pagos.hidden = true;
        facturacion.hidden = true;
        if (!idCategoria.value) {
            cursos.hidden = true;
        } else {
            cursos.removeAttribute("hidden");
            categoriaLabel.innerHTML = idCategoria.options[idCategoria.selectedIndex].text?.toLowerCase();
        }
        const url = `${baseUrl}comboGruposCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`;
        });
        idGrupoCurso.innerHTML = html;
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
        costo.innerHTML = `<h2>$${valorPago.toFixed(2)}</h2>`;
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
        datosTarjeta.querySelectorAll("input").forEach(item => {
            item.removeAttribute("data-validate");
        });
        limpiarForm(datosTarjeta);
        activarValidadores(datosTarjeta);
    } else {
        datosTarjeta.hidden = true;
        datosTarjeta.querySelectorAll("input").forEach(item => {
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
        if (!((parseFloat(parseFloat(valor.value.replaceAll(",", ".")).toFixed(2)) == parseFloat(valorPago.toFixed(2))))) throw new Error("Su pago no puede ser diferente al valor a cancelar.")
        if (!await toastPreguntar(`
        <i class='fs-lg bi-exclamation-triangle-fill text-warning'></i>
        <div class='alert-secondary'>
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
        loaderShow();
        const res = (await axios.post(url, data)).data
        loaderHide();
        await toastPromise(`<div class='alert-success fs-md text-start'>
        Gracias por preferirnos, tu matricula se ha procesado exitosamente,
        su pago será validado antes de legalizar su matricula.
        </div>`);
        top.location.href = `${_route}Estudiantes`;
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
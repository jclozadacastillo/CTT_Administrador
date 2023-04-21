const baseUrl = `${_route}AdministrarMatriculas/`;
let activo = 1;
let parametros = {};
let editable = false;
window.addEventListener("load", async function () {
    loader.hidden = false;
    $(idPeriodo).select2();
    $(idGrupoCurso).select2();
    $(idCurso).select2();
    await comboPeriodos();
    loader.hidden = true;
    content.hidden = false;
});

async function comboPeriodos() {
    try {
        const url = `${baseUrl}comboPeriodos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idPeriodo}'>${item.detalle}</option>`
        });
        idPeriodo.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCursos() {
    try {
        /*divBotones.hidden = true;*/
        idCurso.innerHTML = `<option value="">Seleccione un curso</option>`;
        paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
        idGrupoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
        listar();
        if (idPeriodo.value == "") return;
        const url = `${baseUrl}comboCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`
        });
        idGrupoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCursosAsociados() {
    try {
        idCurso.innerHTML = `<option value="">Seleccione un curso</option>`;
        paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
        listar();
        /*divBotones.hidden = true;*/
        if (idGrupoCurso.value == "")return;
        const url = `${baseUrl}comboCursosAsociados`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idCurso}'>${item.curso}</option>`
        });
        idCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboParalelos() {
    try {
        /*divBotones.hidden = true;*/
        paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
        listar();
        if (idCurso.value == "") return;
        const url = `${baseUrl}comboParalelosListado`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.paralelo}'>${item.paralelo}</option>`
        });
        paralelo.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function listar() {
    try {
        instructorLabel.hidden = true;
        instructorLabel.innerHTML = "";
        /*divBotones.hidden = true;*/
        editable = false;
        const url = `${baseUrl}listarMatriculados`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        parametros = res.parametros;
        const instructor = res.instructor;
        if (!!instructor) {
            instructorLabel.innerHTML = `<b>Instructor:</b> ${instructor.abreviaturaTitulo.replaceAll(".", "")}.
            ${instructor.primerNombre} ${instructor.segundoNombre || ""}
            ${instructor.primerApellido} ${instructor.segundoApellido || ""}`;
            instructorLabel.classList.add("alert-success");
            instructorLabel.classList.remove("alert-warning");
            instructorLabel.removeAttribute("hidden");
        } else if(res.listaCalificaciones.length>0 && paralelo.value!="TODOS") {
            instructorLabel.innerHTML = "<small>NO SE HA ASIGNADO NINGÚN INSTRUCTOR PARA ESTE MÓDULO Y PARALELO</small>";
            instructorLabel.classList.remove("alert-success");
            instructorLabel.classList.add("alert-warning");
            instructorLabel.removeAttribute("hidden");
        }
        /*if (res.listaCalificaciones.length > 0) divBotones.removeAttribute("hidden");*/
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res.listaCalificaciones,
            columns: [
                {
                    data: "idMatricula",
                    class: "text-center td-switch",
                    render: (data, type, row) => {
                        return res.listaCalificaciones.indexOf(row)+1;
                    }
                },
                {
                    title: "Documento",
                    data: "documentoIdentidad",
                    class: ""
                },
                {
                    title: "Estudiante",
                    data: "estudiante",
                    class: "w-100"
                },
                { title: "Paralelo", data: "paralelo", class: "text-center" },
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

async function suspender(_idGrupoCurso, _idCurso, _idMatricula, _checkbox) {
    let estadoAnterior = { class: "badge bg-warning", estado: "SUSPENDIDO" };
    const spanEstado = document.querySelector(`#estado-${_idGrupoCurso}-${_idCurso}-${_idMatricula}`);
    try {
        const dataEstadoAnterior = spanEstado.dataset.anterior;
        if (dataEstadoAnterior == "a") estadoAnterior = { class: "badge bg-success", estado: "APROBADO" };
        if (dataEstadoAnterior == "j") estadoAnterior = { class: "badge bg-info", estado: "JUSTIFICADO" };
        if (dataEstadoAnterior == "r") estadoAnterior = { class: "badge bg-danger", estado: "REPROBADO" };
        if (dataEstadoAnterior == "s") estadoAnterior = { class: "badge bg-warning", estado: "SUSPENDIDO" };
        if (_checkbox.checked) {
            spanEstado.className = "";
            spanEstado.classList.add("badge", "bg-warning");
            spanEstado.innerHTML = "SUSPENDIDO";
        } else {
            spanEstado.className = estadoAnterior.class;
            spanEstado.innerHTML = estadoAnterior.estado;
        }
        const url = `${baseUrl}suspender`;
        const data = new FormData();
        data.append("idCurso", _idCurso);
        data.append("idGrupoCurso", _idGrupoCurso);
        data.append("idMatricula", _idMatricula);
        await axios.post(url, data);
        toastSuccess(`<b>${_checkbox.checked ? "Suspendido" : "Suspención removida"}</b> con éxito`);
        if (!_checkbox.checked) listar();
    } catch (e) {
        handleError(e);
        _checkbox.checked = !_checkbox.checked;
        spanEstado.className = estadoAnterior.class;
        spanEstado.innerHTML = estadoAnterior.estado;
    }
}

async function eliminar(_idGrupoCurso, _idCurso, _idMatricula, _cedula, _estudiante) {
    try {
        if (!await toastPreguntar(`
            <div class='text-danger text-center'><i class='bi-exclamation-triangle-fill'></i></div>
            <div class='fs-sm'>¿Está seguro que desea eliminar la matricula de este módulo para
            <br>
            <span class='fw-bold fs-xxs'>${_cedula} ${_estudiante}</span>
            ?</div>
            <div class='fw-bold text-danger text-justify mt-2 fs-xxs'>
            <i class='bi-exclamation-triangle-fill'></i> Recuerde que esta acción no se puede deshacer y se perderan todas las
            calificaciones correspondientes al modulo seleccionado. <i class='bi-exclamation-triangle-fill'></i></div>
        `)) return;
        const url = `${baseUrl}eliminar`;
        const data = new FormData();
        data.append("idCurso", _idCurso);
        data.append("idGrupoCurso", _idGrupoCurso);
        data.append("idMatricula", _idMatricula);
        await axios.post(url, data);
        toastSuccess("Módulo eliminado exitosamente");
        listar();
    } catch (e) {
        handleError(e);
    }
}

async function generarExcel() {
    try {
        return response = new Promise(async (resolve) => {
            const url = `${baseUrl}generarExcel`;
            bloquearBotones();
            const data = new FormData(frmDatos);
            let res = await axios({
                method: "POST",
                url,
                data,
                responseType: "arraybuffer"
            }).catch(e => {
                handleError(e.message);
            });
            res = res.data;
            const blob = new Blob([res], { type: 'application/vnd.ms-excel' });
            const urlObject = window.URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = urlObject;
            link.download = `REPORTE_MATRICULAS_${new Date().toISOString()}.xlsx`;
            link.click();
            window.URL.revokeObjectURL(urlObject);
            link.remove();
            resolve(true);
        });
    } catch (e) {
        handleError(e);
        resolve(false);
    } finally {
        desbloquearBotones();
    }
}

async function generarPdf() {
    try {
        bloquearBotones();
        const url = `${baseUrl}generarPdfReporte`;
        const data = new FormData(frmDatos);
        const res = await axios({
            method: "POST",
            url,
            data,
            responseType: "arraybuffer"
        });
        await downloadArchivo(res.data);
    } catch (e) {
        handleError(e);
        desbloquearBotones();
    } finally {
        desbloquearBotones();
    }
}

async function downloadArchivo(res) {
    return new Promise(async (resolve) => {
        try {
            const blob = new Blob([res], { type: 'application/pdf' });
            const urlObject = window.URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = urlObject;
            link.download = `${"Matriculas_Calificaciones"}_${new Date().toISOString()}.pdf`;
            link.click();
            window.URL.revokeObjectURL(urlObject);
            link.remove();
            resolve(true);
        } catch (e) {
            console.error(`${e.message}`);
            resolve(false);
        }
    });
}
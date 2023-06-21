const baseUrl = `${_route}ListadoPorCurso/`;
let activo = 1;
let parametros = {};
let editable = false;
(async function () {
    loader.hidden = false;
    $(idPeriodo).select2();
    $(idGrupoCurso).select2();
    await comboPeriodos();
    loader.hidden = true;
    content.hidden = false;
})();

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
        paralelo.innerHTML = `<option value="">Seleccione un curso/diplomado</option>`;
        idGrupoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
        listar();
        divBotones.hidden = true;
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

async function comboTiposCursos() {
    try {
        idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo</option>`;
        paralelo.innerHTML = `<option value="">Seleccione un curso/diplomado</option>`;
        listar();
        divBotones.hidden = true;
        if (idPeriodo.value == "") return;
        const url = `${baseUrl}comboTiposCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`
        });
        idTipoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboParalelos() {
    try {
        paralelo.innerHTML = `<option value="">Seleccione un curso/diplomado</option>`;
        listar();
        divBotones.hidden = true;
        if (idGrupoCurso.value == "") return;
        const url = `${baseUrl}comboParalelos`;
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
        divBotones.hidden = true;
        editable = false;
        const url = `${baseUrl}listar`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        parametros = res.parametros;
        const instructor = res.instructor;
        if (!!instructor) {
            instructorLabel.innerHTML = `<b>Instructor:</b> ${instructor.abreviaturaTitulo.replaceAll(".", "")}.
            ${instructor.primerNombre} ${instructor.segundoNombre || ""}
            ${instructor.primerApellido} ${instructor.segundoApellido || ""}`;
            instructorLabel.removeAttribute("hidden");
        }
        if ($.fn.DataTable.isDataTable(tableDatos)) {
            $(tableDatos).DataTable().clear().destroy();
            tableDatos.innerHTML = "";
        }
        if (res.listaCalificaciones.length > 0) divBotones.removeAttribute("hidden");
        let columns = [
            {
                title: "Documento",
                data: "documentoIdentidad",
                class: ""
            },
            {
                title: "Estudiante",
                data: "idMatricula",
                class: `${res.listaModulos.length > 1 ? "w-50" : "w-100"}`,
                render: (data, type, row) => {
                    return `${row.primerApellido} ${row.segundoApellido || ""} ${row.primerNombre} ${row.segundoNombre || ""}`
                }
            },
            { title: "Paralelo", data: "paralelo", class: "text-center" }
        ];
        if (res.listaEstudiantes.length > 0) {
            res.listaModulos.forEach(item => {
                columns.push({
                    title: `${res.listaModulos.length>1?item.curso:"Promedio"}`,
                    data: 'idMatricula',
                    class: `text-end ${res.listaModulos.length>1?"":"text-nowrap"}`,
                    render: data => {
                        const promedio = parseFloat(res.listaCalificaciones.find(x => x.idCurso == item.idCurso && x.idMatricula == data)?.promedioFinal).toFixed(2);
                        res.listaEstudiantes.find(x => x.idMatricula == data)[`${res.listaModulos.length > 1 ? item.curso : "Promedio"}`] = promedio;
                        return promedio;
                    }
                });
            });
        }
        if (res.listaEstudiantes.length > 1) {
            columns.push({
                title: "Promedio",
                data: "idMatricula",
                class: "text-end",
                render: data => {
                    const notas = res.listaCalificaciones.filter(x => x.idMatricula == data).map(x => x.promedioFinal);
                    const promedio = notas.length > 0 ? parseFloat(notas.reduce((a, b) => a + b, 0) / notas.length).toFixed(2) : parseFloat(0).toFixed(2);
                    res.listaEstudiantes.find(x => x.idMatricula == data)["promedio"] = promedio;
                    return promedio;
                }
            });
        }

        columns.push({
            title: "Estado",
            data: "idMatricula",
            class: "text-end",
            render: data => {
                const notas = res.listaCalificaciones.filter(x => x.idMatricula == data).map(x => x.promedioFinal);
                const promedio = notas.length > 0 ? parseFloat(notas.reduce((a, b) => a + b, 0) / notas.length).toFixed(2) : parseFloat(0).toFixed(2);
                res.listaEstudiantes.find(x => x.idMatricula == data)["estado"] = promedio >= 7 ? "APROBADO" : "REPROBADO";
                return promedio >= 7 ? "<span class='badge rounded-phill bg-success text-white'>APROBADO</span>" : "<span class='badge rounded-phill bg-danger text-white'>REPROBADO</span>"
            }
        });
            $(tableDatos).DataTable({
                bDestroy: true,
                data: res.listaEstudiantes,
                columns,
                columnDefs: [
                    { targets: [0], orderable: false }
                ],
                aaSorting: []
            });

    } catch (e) {
        handleError(e);
    }
}


async function generarExcel() {
    try {
        return response = new Promise(async (resolve) => {
            let lista = $(tableDatos).DataTable().rows().data().toArray();
            const url = `${baseUrl}generarExcel`;
            bloquearBotones();
            const data = new FormData();
            data.append("_lista", JSON.stringify(lista));
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
            link.download = `CONSOLIDADO_NOTAS_${new Date().toISOString()}.xlsx`;
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

async function generarExcelCertificados() {
    try {
        return response = new Promise(async (resolve) => {
            let lista = $(tableDatos).DataTable().rows().data().toArray();
            lista = lista.filter(x=>x.estado==="APROBADO").map(x => {
                return {
                    "Cédula": x.documentoIdentidad,
                    primerNombre: x.primerNombre,
                    segundoNombre: x.segundoNombre,
                    primerApellido: x.primerApellido,
                    segundoApellido: x.segundoApellido,
                    Ciudad:x.centro,
                    Carrera: x.carrera
                };
            }
            );
            const url = `${baseUrl}generarExcelCertificados`;
            bloquearBotones();
            const data = new FormData(frmDatos);
            data.append("_lista", JSON.stringify(lista));
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
            link.download = `CONSOLIDADO_CERTIFICADOS_${new Date().toISOString()}.xlsx`;
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
    let lista = $(tableDatos).DataTable().rows().data().toArray();
    lista=lista.map(x =>{ 
        return {
            centro: x.centro,
            documentoIdentidad: x.documentoIdentidad,
            primerApellido: x.primerApellido,
            segundoApellido: x.segundoApellido,
            nombres: `${x.primerNombre} ${x.segundoNombre || ""}`,
            carrera: x.carrera,
            estado:x.estado
        };
    }
    );
    try {
        bloquearBotones();
        const url = `${baseUrl}generarPdfReporte`;
        const data = new FormData(frmDatos);
        data.append("_lista", JSON.stringify(lista));
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
            link.download = `${"CONSOLIDADO_NOTAS"}_${new Date().toISOString()}.pdf`;
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
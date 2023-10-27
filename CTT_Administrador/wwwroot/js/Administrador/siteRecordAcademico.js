const baseUrl = `${_route}RecordAcademico/`;

(async function () {
    loader.hidden = false;
    $(idEstudiante).select2();
    await comboEstudiantes();
    loader.hidden = true;
    content.hidden = false;
})();

async function comboEstudiantes() {
    try {
        const url = `${baseUrl}comboEstudiantes`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idEstudiante}'>${item.documentoIdentidad} - ${item.estudiante}</option>`
        });
        idEstudiante.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}
async function listar() {
    try {
        const tableDiv = document.querySelector(".table-responsive");
        if (!!tableDiv) !idEstudiante.value ? tableDiv.hidden = true : tableDiv.removeAttribute("hidden");
        const url = `${baseUrl}listar`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    title: "Fecha Registro",
                    data: "fechaMatricula",
                    render:data=>data?.split("T")[0]
                },
                { title: "Tipo", data: "tipoCurso" },
                { title: "Curso/Modulo", data: "curso" },
                { title: "Paralelo", data: "paralelo" },
                { title: "Nota Final", data: "promedioFinal" },
                { title: "Faltas", data: "faltas" },
                { title: "Estado", data: "estado" },
                { title: "Justificación", data: "justificado" },
                { title: "Observaciones", data: "justificacionObservacion" },

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
            lista = lista.filter(x => x.estado === "APROBADO").map(x => {
                return {
                    "Cédula": x.documentoIdentidad,
                    primerNombre: x.primerNombre,
                    segundoNombre: x.segundoNombre,
                    primerApellido: x.primerApellido,
                    segundoApellido: x.segundoApellido,
                    Ciudad: x.centro,
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
    let listaMatriculas = lista.map(x => {
        return x.idMatricula;
    });

    lista = lista.map(x => {
        return {
            centro: x.centro,
            documentoIdentidad: x.documentoIdentidad,
            primerApellido: x.primerApellido,
            segundoApellido: x.segundoApellido,
            nombres: `${x.primerNombre} ${x.segundoNombre || ""}`,
            carrera: x.carrera,
            promedioFinal: x.promedio,
            estado: x.estado
        };
    }
    );

    let matriculas = "0";
    listaMatriculas.forEach(item => {
        matriculas += `,${item}`;
    })

    try {
        bloquearBotones();
        const url = `${baseUrl}generarPdfReporte`;
        const data = new FormData(frmDatos);
        data.append("_lista", JSON.stringify(lista));
        data.append("_listaMatriculas", matriculas);
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
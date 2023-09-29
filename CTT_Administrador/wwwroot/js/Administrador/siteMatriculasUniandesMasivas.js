const baseUrl = `${_route}MatriculasUniandesMasivas/`;
const modalEspere = new bootstrap.Modal(espere, {
    keyboard: false,
    backdrop: 'static'
})
let listaEstudiantes = [];
(async function () {
    $(idGrupoCurso).select2();
    activarValidadores(frmDatos)
    llenarTabla();
    loader.hidden = false;
    await comboPeriodos();
    loader.hidden = true;
    content.hidden = false;
})();
let formatosPermitidos = [
    "application/vnd.ms-excel",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
];

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

async function comboTiposCursos() {
    try {
        idTipoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
        idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo de curso</option>`;
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
archivo.addEventListener("change", function () {
    listaEstudiantes = [];
    llenarTabla();
    if (this.value == "") return;
    if (!formatosPermitidos.includes(this.files[0].type)) {
        toastWarning("Formato de archivo no admitido");
        this.value = "";
        this.classList.add("is-invalid");
        return;
    }
    cargarDatos();
});
async function comboCursos() {
    try {
        idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo de curso</option>`;
        if (idTipoCurso.value == "") return;
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

async function cargarDatos(_press) {
    try {
        if (archivo.value == "" && _press == true) throw new Error("Debe seleccionar un archivo");
        listaEstudiantes = [];
        btnGenerar.hidden = true;
        const url = `${baseUrl}cargarDatos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        if (res.listaEstudiantes.length == 0) throw new Error("No se ha encontrado ningún estudiante en el documento");
        btnGenerar.hidden = false;
        listaEstudiantes = res.listaEstudiantes;
        if (res.novedades != "") {
            toastHtml(`

            <div class='fw-bold fs-xxs text-justify-all m-0 p-0'><i class='bi-exclamation-triangle text-warning me-2'></i>Se ha procesado el archivo con algunas novedades.</div>
            <ul class='w-100 text-justify-all ul-novedades'>
            ${res.novedades}
            </ul>
            `);
        }
    } catch (e) {
        handleError(e);
        btnGenerar.hidden = true;
    } finally {
        llenarTabla();
    }
}

function llenarTabla() {
    tableDatos.innerHTML = "";
    $(tableDatos).DataTable({
        bDestroy: true,
        data: listaEstudiantes,
        columns: [
            { title: "Cédula", class: "", data: "documentoIdentidad" },
            { title: "Primer Nombre", class: "", data: "primerNombre" },
            { title: "Segundo Nombre", class: "", data: "segundoNombre" },
            { title: "Primer Apellido", class: "", data: "primerApellido" },
            { title: "Segundo Apellido", class: "", data: "segundoApellido" },
            { title: "Centro", class: "", data: "centro_detalle" },

        ]
    })
}

async function generarMatriculas() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        if (!await toastPreguntar(`
            <div class='text-justify-all fs-xxs my-2'>
                Está seguro que desea generar <b>${listaEstudiantes.length}</b> ${listaEstudiantes.length == 1 ? "matricula" : "matriculas"} para:
            </div>
            <div class='text-justify-all fs-xxs'>
            <b>Periodo Académico: </b></br>${idPeriodo.options[idPeriodo.selectedIndex].text}
            </div>
            <div class='text-justify-all fs-xxs'>
            <b>Tipo de curso: </b></br>${idTipoCurso.options[idTipoCurso.selectedIndex].text}
            </div>
            <div class='text-justify-all fs-xxs'>
            <b>Curso: </b></br>${idGrupoCurso.options[idGrupoCurso.selectedIndex].text}
            </div>
            <div class='text-justify-all fs-xxs'>
            <b>Paralelo: </b></br>${paralelo.value}
            </div>
            <div class='mt-2 text-center text-danger fw-bold' style='font-size:11px'>
                <i class='bi-exclamation-triangle-fill me-1'></i> Esta acción generará todas las matriculas y no se puede deshacer
            </div>
        `)) return;
        bloquearBotones();
        modalEspere.show();
        const url = `${baseUrl}generarMatriculas`;
        const data = new FormData(frmDatos);
        console.log(listaEstudiantes);
        listaEstudiantes = [...listaEstudiantes].map(x => {
            x.nombre = `${x.primerApellido} ${x.segundoApellido} ${x.primerNombre} ${x.segundoNombre}`;
            x.documentoIdentidad = x.documentoIdentidad.trim();
            x.documento = x.documentoIdentidad.trim();
            x.idcentro = x.idcentro || "";
            x.codigo_carrera = x.codigo_carrera || "";
            return x;
        });
        console.log(listaEstudiantes);
        data.append("_alumnos", JSON.stringify(listaEstudiantes));
        data.append("_alumnosCedulas", await generarSubconsulta());
        const res = await axios({
            method: "POST",
            url,
            data,
            responseType: "arraybuffer"
        });
        await downloadArchivo(res.data);
        toastSuccess("Matriculas generadas exitosamente");
        await new Promise(resolve => setTimeout(() => resolve(true), 1900));
        //top.location.reload();
    } catch (e) {
        handleError(e);
        setTimeout(() => modalEspere.hide(), 1900);
    } finally {
        desbloquearBotones();
        modalEspere.hide();
    }
}

function generarSubconsulta() {
    let subconsulta = `'D3F@UL7'`;
    return new Promise(resolve => {
        try {
            if (listaEstudiantes.length == 0) throw new Error("Sin datos");
            listaEstudiantes.forEach((item, index) => {
                subconsulta += `,'${item.documentoIdentidad}'`;
                if (index == listaEstudiantes.length - 1) resolve(subconsulta.trim().replaceAll(" ", ""));
            });
        } catch (e) {
            resolve(subconsulta);
        }
    })
}

async function downloadArchivo(res) {
    try {
        return new Promise(async (resolve) => {
            const blob = new Blob([res], { type: 'application/pdf' });
            const urlObject = window.URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = urlObject;
            link.download = `${"Reporte descarga"}_${new Date().toISOString()}.pdf`;
            link.click();
            window.URL.revokeObjectURL(urlObject);
            link.remove();
            resolve(true);
        });
    } catch (e) {
        console.error(`${e.message}`);
        resolve(false);
    }
}
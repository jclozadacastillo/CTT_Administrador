const baseUrl = `${_route}Calificaciones/`;
let matriculas = [];
(async function () {
    loaderShow();
    $(idMatricula).select2();
    await listarCursos();
    loaderHide();
})();

async function listarCursos() {
    try {
        matriculas = [];
        const url = `${baseUrl}listarCursos`;
        matriculas = (await axios.get(url)).data;
        let html = "<option value=''>Seleccione</option>";
        matriculas.forEach(item => {
            html += `<option value='${item.idMatricula}'>${item.year} - (${item.tipoCurso}) ${item.curso}</option>`;
        });
        idMatricula.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function mostrarCalificaciones() {
    try {
        loaderShow();
        calificaciones.hidden = true;
        datosCurso.innerHTML = "";
        if (idMatricula.value == "") return;
        calificaciones.removeAttribute("hidden");
        const curso = matriculas.find(x => x.idMatricula == idMatricula.value);
        let html= `
        <div class='col-sm-12'>
            <h5 class='text-primary fw-bold'>MODALIDAD</h5>
            <h5>${curso.modalidad}</h5>
        </div>
        <hr>
        `;
        const url = `${baseUrl}listarCalificaciones`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        res.forEach(item => {
            html += curso.esDiplomado != 1 ? "" : 
                `<div class='col-sm-12'>
                    <h5 class='text-primary fw-bold'>MÓDULO</h5>
                    <h5>${item.modulo}</h5>
                </div>`
                ;
            html += `<div class='col-sm-12'>
                        <h5 class='text-primary fw-bold'>INSTRUCTOR</h5>
                        <h5>${item.instructor}</h5>
                    </div>`;
            for (var i = 0; i < item.numeroNotas; i++) {
                html += `<div class='col-sm-12'>
                        <h5 class='text-primary fw-bold'>NOTA ${i+1}</h5>
                        <h5>${item[`nota${i+1}`].toFixed(2)}</h5>
                    </div>`;
            }
            html += `<div class='col-sm-12'>
                        <h5 class='text-primary fw-bold'>FALTAS</h5>
                        <h5>${item.justificaFaltas===0?item.faltas:0}</h5>
                    </div>
                    <div class='col-sm-auto'>
                        <h5 class='text-primary fw-bold'>PROMEDIO FINAL</h5>
                        <h5>${item.promedioFinal.toFixed(2)}</h5>
                    </div>
                    <div class='col-sm-auto mt-2 text-center'>
                        <span class='badge fs-4 fw-bold ${item.aprobado === 1 ? "bg-success" : "bg-danger"}'>
                        ${item.aprobado === 1 ? 'APROBADO<i class="bi-check-circle-fill ms-2"></i>' :
                        'REPROBADO<i class="bi-x-circle-fill ms-2"></i>'}</span>
                    </div>
                    ${res.length > 1 ?'<hr>':''}
                    `
        });
        datosCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
        datosCurso.innerHTML = "";
    } finally {
        loaderHide();
    }
}
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
        tableCalificaciones.innerHTML = "";
        if (idMatricula.value == "") return;
        calificaciones.removeAttribute("hidden");
        const curso = matriculas.find(x => x.idMatricula == idMatricula.value);
        datosCurso.innerHTML = `
        <div class='col-sm-12'>
            <h5 class='text-primary fw-bold'>MODALIDAD</h5>
            <h5>${curso.modalidad}</h5>
        </div>
         <div class='col-sm-12'>
            <h5 class='text-primary fw-bold'>INSTRUCTOR</h5>
            <h5>${curso.instructor}</h5>
        </div>
        
        `;
        const url = `${baseUrl}listarCalificaciones`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let thead = "<thead><tr>";
        let tbody="<tbody><tr>"
        thead += curso.esDiplomado == 1 ? "<th>Módulo</th>" : "";
        let max = Math.max(res.map(x => { return x.numeroNotas }));
        for (var i = 0; i < max; i++) {
            thead+=`<th>NOTA ${i+1}</th>`
        }
        thead += `<th class='text-end'>Promedio</th>
                  <th class='text-end'>Faltas</th>
                  <th>Faltas justificadas</th>
                  <th>Estado</th>
                  `;
        thead += "</tr></thead>"
        
        tbody+="</tbody>"
        tableCalificaciones.innerHTML = thead+tbody;
    } catch (e) {
        handleError(e);
        datosCurso.innerHTML = "";
        tableCalificaciones.innerHTML = "";
    } finally {
        loaderHide();
    }
}
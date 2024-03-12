const baseUrl = `${_route}HistorialAcademico/`;
const modal = new bootstrap.Modal(modalDatos, {
    backdrop: "static"
});
(async function () {
    await listar();
    loader.hidden = true;
    content.hidden = false;
})();

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
                    title: "<i class='bi-gear-fill'></i>",
                    data: "idEstudiante",
                    class: "text-center w-id",
                    render: (data, type, row) => {
                        return `<button type='button'
                        title='ver historial académico'
                        class='btn btn-sm btn-primary' onclick='verHistorial(${data})'><i class='bi-arrow-up-right-circle'></i></button>`
                    }
                },
                { title: "Documento", data: "documentoIdentidad", class: "w-cedula" },
                { title: "Estudiante", data: "estudiante", class: "text-nowrap" },
                { title: "Provincia", data: "provincia", class: "text-nowrap" },
                { title: "Ciudad", data: "ciudad", class: "text-nowrap" },
                {
                    title: "Celular",
                    data: "celular",
                    class: "text-nowrap",
                    render: (data) => {
                        return data || "<small class='text-muted'>SIN REGISTRO</small>"
                    }
                },
                {
                    title: "Email",
                    data: "email",
                    class: "w-50 text-nowrap",
                    render: (data) => {
                        return data || "<small class='text-muted'>SIN REGISTRO</small>"
                    }
                }
            ],
            columnDefs: [
                { targets: [0, 1], orderable: false }
            ],
            order: [[2, "ASC"]],
        })
    } catch (e) {
        handleError(e);
    }
}
function reloadDataTable() {
    setTimeout(function () { $(tableDatos).DataTable().ajax.reload(); }, 100);
}

async function verHistorial(_idEstudiante) {
    try {
        loaderShow();
        const url = `${baseUrl}historialAcademico/${_idEstudiante}`;
        const res = (await axios.get(url)).data;
        const matriculas = res.matriculas;
        const estudiante = res.estudiante;
        const calificaciones = res.calificaciones;
        modalDatosLabel.innerHTML = `HISTORIAL ACADÉMICO #${_idEstudiante.toString().padStart(5, "0")}`;
        let html = `<div class='card mb-2'>
                    <div class='card-body'>
                    <div class='table-responsive'>
                        <table class='table-datos-estudiante w-100'>
                            <tr>
                                <td class='fw-bold text-nowrap'>DOCUMENTO:</td>
                                <td width='90%' class='ps-1'>${estudiante.documentoIdentidad}</td>
                                <td rowspan='2' class='text-end'><img src='${_route}lib/utilities/toast/img/logo-azul370.png' width='109'/></td>
                            </tr>
                            <tr>
                                <td class='fw-bold text-nowrap'>ESTUDIANTE:</td>
                                <td class='ps-1 text-nowrap'>${`${estudiante.primerApellido} ${estudiante.segundoApellido || ""} ${estudiante.primerNombre} ${estudiante.segundoNombre || ""}`.replaceAll("  ", " ")}</td>
                            </tr>
                        </table>
                        </div>
                    </div>
                    </div>`;

        divDatosEstudiante.innerHTML = html;
        html = `<div class="accordion" id="acordion${_idEstudiante}">`;
        matriculas.forEach(item => {
            const modulos = calificaciones.filter(x => x.idMatricula == item.idMatricula);
            if (modulos.length > 0) {
                html += `<div class="accordion-item bg-gradient bg-light">
                    <h2 class="accordion-header accordion-custom bg-gradient bg-primary">
                      <button class="accordion-button collapsed bg-gradient bg-primary" type="button" data-bs-toggle="collapse" data-bs-target="#gc${item.idGrupoCurso}" aria-expanded="false" aria-controls="gc${item.idGrupoCurso}">
                        ${`${item.curso} <b class='ms-2'>#${`${item.idMatricula}`.padStart(9, "0")}</b>`}
                      </button>
                    </h2>
                    <div id="gc${item.idGrupoCurso}" class="accordion-collapse collapse">
                      <div class="accordion-body row my-0 py-1">
                      <div class='col-sm-12 py-2'>
                         <div class='row px-0 mx-0'>
                            <div class='col-sm-6'>
                               <h6 class='ms-2 fw-bold text-white mt-2 badge bg-secondary bg-gradient'>${item.esDiplomado == 1 ? "MÓDULOS RECIBIDOS" : "CALIFICACIONES"}</h6>
                            </div>
                            <div class='col-sm-6 text-lg-end text-md-end text-sm-center'>
                                <button class='btn btn-danger btn-sm'>CERTIFICADO DE MATRÍCULA<i class='bi-download ms-2'></i></button>
                            </div>
                        </div>
                      `;
                modulos.forEach(modulo => {
                    html += `<div class='card my-1 border-secondary border'>
                            <div class='card-body py-2'>
                                <table class='table-datos-estudiante table-bordered-custom w-100 mt-0 mb-2'>
                                    ${item.esDiplomado == 1 ? `<tr style='border-bottom:1px solid #798289'><td colspan='2' class='fw-bold fst-italic pb-1 text-muted no-border'>${modulo.curso}</td></tr>` : ""}
                                    <tr>
                                        <td class='fw-bold text-nowrap'>FECHA:</td>
                                        <td width='90%' class='ps-1'>${modulo.fechaRegistro.substring(0, 10)}</td>
                                    </tr>
                                    <tr>
                                        <td class='fw-bold text-nowrap'>CALIFICA ASISTENCIA:</td>
                                        <td width='90%' class='ps-1'>${item.calificaAsistencia == 1 ? "<span class='badge bg-success bg-gradient'>SI</span>" : "<span class='badge bg-secondary bg-gradient'>NO</span>"}</td>
                                    </tr>
                                    ${item.calificaAsistencia == 1 ? `
                                    <tr>
                                        <td class='fw-bold text-nowrap'>FALTAS:</td>
                                        <td width='90%' class='ps-1'>${modulo.faltas.toFixed(2)}</td>
                                    </tr>
                                    <tr>
                                        <td class='fw-bold text-nowrap'>JUSTIFICA FALTAS:</td>
                                        <td width='90%' class='ps-1'>${modulo.justificaFaltas == 1 ? "<span class='badge bg-success bg-gradient'>SI</span>" : "<span class='badge bg-secondary bg-gradient'>NO</span>"}</td>
                                    </tr>
                                    `: ``}
                                    <tr>
                                        <td class='fw-bold text-nowrap'>CALIFICACIÓN:</td>
                                        <td width='90%' class='ps-1'>${modulo.promedioFinal.toFixed(2)}</td>
                                    </tr>
                                    <tr>
                                        <td class='fw-bold text-nowrap'>ESTADO:</td>
                                        <td width='90%' class='ps-1'>${modulo.aprobado == 1 ? "<span class='badge bg-success bg-gradient'>APROBADO</span>" : "<span class='badge bg-danger bg-gradient'>REPROBADO</span>"}</td>
                                    </tr>
                                </table>
                            </div>
                        </div>`;
                });

                html += `</div>
                  </div>
                 </div>
                </div>`;
            }
        });
        html += `</div>`

        divDatosMatriculas.innerHTML = html;
        modal.show();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
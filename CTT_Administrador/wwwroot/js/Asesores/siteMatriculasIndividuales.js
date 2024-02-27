const baseUrl = `${_route}MatriculasIndividuales/`;
const idTipoCursoSession = sessionStorage.getItem("idTipoCurso");
const idPeriodoSession = sessionStorage.getItem("idPeriodo");
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
});
const btnTemplate = `<button type="button" class="btn btn-sm btn-primary btn-new mt-lg-4 mt-2" title="Nuevo registro de matricula" onclick="nuevo()">
                <i class="bi-plus"></i> Nueva Matricula
            </button>`;

window.addEventListener("load", async function () {
    loaderShow();
    $(idPeriodo).select2();
    $(idTipoCurso).select2();
    $(idGrupoCurso).select2({
        dropdownParent: modalDatos
    })
    await listarPeriodos();
    await listarTiposCursos();
    loaderHide();
});

async function listarPeriodos() {
    try {
        const url = `${baseUrl}listarPeriodos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>SELECCIONE</option>";
        res.forEach(item => {
            html += `<option value='${item.idPeriodo}' ${item.idPeriodo == idPeriodoSession ? 'selected' : ''}>${item.detalle}</option>`;
        });
        idPeriodo.innerHTML = html;
        handlePreload();
    } catch (e) {
        handleError(e);
    }
}
async function listarTiposCursos() {
    try {
        const url = `${baseUrl}listarTiposCursos`;
        const res = (await axios.get(url)).data;
        let html = "<option value=''>SELECCIONE</option>";

        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}' ${item.idTipoCurso == idTipoCursoSession ? 'selected' : ''}>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML = html;
        handlePreload();
    } catch (e) {
        handleError(e);
    }
}
function handlePreload() {
    if (!!idPeriodoSession || !!idTipoCursoSession) listar();
}
async function listar() {
    try {
        sessionStorage.setItem("idPeriodo", idPeriodo.value);
        sessionStorage.setItem("idTipoCurso", idTipoCurso.value);
        containerBtn.innerHTML = !!idPeriodo.value && !!idTipoCurso.value ? btnTemplate : "";
        const url = `${baseUrl}listar`;
        const params = JSON.stringify(await formToJsonTypes(frmFiltros));
        await $(tableDatos).DataTable({
            bDestroy: true,
            serverSide: true,
            processing: false,
            ajax: async function (_data, resolve) {
                try {
                    _data.parametros = params;
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
                        return `<button class='btn-option-table text-danger'><i class='bi-trash-fill'></i></button>`
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
                { title: capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso"), data: "curso" },
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

async function nuevo() {
    modalDatosLabel.innerText = `NUEVA MATRICULA DE ${idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "CURSO"}`;
    labelGrupoCurso.innerText = capitalize(idTipoCurso.options[idTipoCurso.selectedIndex]?.text || "Curso");
    await listarCursos();
    if (!!$(idEstudiante).data("select2")) {
        idEstudiante.innerHTML = "";
        $(idEstudiante).select2("destroy");
    }
    $(idEstudiante).select2({
        dropdownParent: modalDatos,
        ajax: {
            delay: 370,
            url: `${baseUrl}listarEstudiantes`,
            data: function (params) {
                var query = {
                    search: params.term
                }
                return query;
            },
            processResults: function (data) {
                return {
                    results: data
                };
            }
        },
        cache: true,
        placeholder: 'SELECCIONE',
        minimumInputLength: 1
    },
    );
    activarValidadores(frmDatos);
    limpiarForm(frmDatos);
}

function handleCursos() {
}

async function listarCursos() {
    try {
        const url = `${baseUrl}listarCursos`;
        const data = new FormData(frmFiltros);
        const res = (await axios.post(url, data)).data;
        let html = `<option value=''>SELECCIONE</option>`;
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`;
        });
        idGrupoCurso.innerHTML = html;
        if (res.length > 0) {
            modal.show()
        } else {
            toastWarning("No existe ninguna oferta académica que pueda generar matriculas para ese periodo y ese tipo de curso");
        }
    } catch (e) {
        handleError(e);
    }
}
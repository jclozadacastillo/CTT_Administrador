const baseUrl = `${_route}Calificaciones/`;
let activo = 1;
let listaCalificaciones = [];
let parametros = {};
let editable = false;
window.addEventListener("load", async function () {
    loader.hidden = false;
    $(idPeriodo).select2();
    $(idGrupoCurso).select2();
    $(idCurso).select2();
    await comboPeriodos();
    loader.hidden=true;
    content.hidden=false;
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
        if (idPeriodo.value == "") {
            btnGuardarTodo.hidden = true;
            idCurso.innerHTML = `<option value="">Seleccione un curso</option>`;
            paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
            idGrupoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
            listaCalificaciones = [];
            llenarTabla();
            return;
        }

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
        if (idGrupoCurso.value == "") {
            btnGuardarTodo.hidden = true;
            idCurso.innerHTML = `<option value="">Seleccione un curso</option>`;
            paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
            listaCalificaciones = [];
            llenarTabla();
            return;
        }

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
        if (idCurso.value == "") {
            btnGuardarTodo.hidden = true;
            paralelo.innerHTML = `<option value="">Seleccione un módulo</option>`;
            listaCalificaciones = [];
            llenarTabla();
            return;
        }

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
        btnGuardarTodo.hidden = true;
        editable = false;
        const url = `${baseUrl}listar`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        parametros = res.parametros;
        listaCalificaciones = res.listaCalificaciones;
        llenarTabla();
    } catch (e) {
        handleError(e);
    }
}

function llenarTabla() {
    let html = "";
    const body = tableDatos.querySelector("tbody");
    body.innerHTML = html;
    listaCalificaciones.forEach((item, index) => {
        const allow = (item.tiempoLimite >= 0 || item.tipoLimiteAtraso >= 0);
        editable = allow;
        html += `<tr>
                       <td>${index+1}</td>
                       <td>${item.documentoIdentidad}</td>
                       <td>${item.estudiante}</td>
                       <td>${allow ? `<input maxlength="4" data-ref="nota1" class='input-nota'  data-validate="decimal" value='${item.nota1.toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item.nota1.toString().replaceAll(".", ",") }</div>`}</td>
                       <td>${allow ? `<input maxlength="4" data-ref="nota2" class='input-nota' data-validate="decimal" value='${item.nota2.toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item.nota2.toString().replaceAll(".", ",") }</div>`}</td>
                       <td>${allow ? `<input maxlength="4" data-ref="nota3" class='input-nota' data-validate="decimal" value='${item.nota3.toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item.nota3.toString().replaceAll(".", ",") }</div>`}</td>
                       <td>${allow && parametros.calificaAsistencia==1 ? `<input maxlength="4" data-ref="faltas" class='input-nota' data-validate="numeros" value='${item.faltas.toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item.faltas.toString().replaceAll(".", ",") }</div>`}</td>
                </tr>`;
    });
    body.innerHTML = html || `<tr><td colspan="7" class="text-center">Lista vacia</td></tr>`;   
    if (editable && listaCalificaciones.length>0) btnGuardarTodo.removeAttribute("hidden");
    mapearValidadoresTabla();
}

function mapearValidadoresTabla() {
    activarValidadores(tableDatos.querySelector("tbody"));
    tableDatos.querySelector("tbody").querySelectorAll("tr").forEach((row, index) => {
        row.querySelectorAll("input[data-ref]").forEach(item => {
            item.addEventListener("focusout", () => {
                if (item.dataset.ref == "faltas") {
                    if (parseFloat(item.value.replaceAll(",", ".")) > parametros.horasCurso) item.value = parametros.horasCurso;
                } else {
                    if (parseFloat(item.value.replaceAll(",", ".")) > parametros.puntajeMaximo) item.value = parametros.puntajeMaximo;
                }
                guardar(index);
            });
        });
    });
}


async function guardar(_index) {
    try {
        const elementos = tableDatos.querySelector("tbody").querySelectorAll("tr")[_index];
        if (!elementos) return;
        elementos.querySelectorAll("input[data-ref]").forEach(item => {
            listaCalificaciones[_index][item.dataset.ref] = item.value.replaceAll(",",".");
        });
        const url = `${baseUrl}guardar`;
        await axios.post(url, JSON.stringify(listaCalificaciones[_index]), {
            headers: { 'Content-Type': 'application/json' }
        });
    } catch (e) {
        listar();
        handleError(e);
    }

}

async function guardarTodo() {
    try {
        bloquearBotones();
        listaCalificaciones = [...listaCalificaciones].map(x => {
            x.nota1 = x.nota1.toString().replaceAll(",", ".");
            x.nota2 = x.nota2.toString().replaceAll(",", ".");
            x.nota3 = x.nota3.toString().replaceAll(",", ".");
            x.faltas = x.faltas.toString().replaceAll(",", ".");
            return x;
        })
        const url = `${baseUrl}guardarTodo`;
        await axios.post(url, JSON.stringify(listaCalificaciones), {
            headers: { 'Content-Type': 'application/json' }
        });
        toastSuccess("Guardado con éxito");
        listar();
    } catch (e) {
        handleError(e);
        activarBotones();
    } finally {
        desbloquearBotones();
    }
}

function buscarEnTabla(_busqueda) {
    tableDatos.querySelector("#trSinResultados")?.remove();
    if (tableDatos.innerText.indexOf("Lista vacia") > 0) return;
    tableDatos.querySelector("tbody").querySelectorAll("tr").forEach(item => {
        item.removeAttribute("hidden");
        if (item.innerText.toLowerCase().indexOf(_busqueda.toLowerCase())<0) {
            item.hidden = true;
        }
    });
    if (tableDatos.querySelector("tbody").querySelectorAll("tr").length == tableDatos.querySelector("tbody").querySelectorAll("tr[hidden]").length) {
        tableDatos.querySelector("tbody").insertAdjacentHTML("beforeend", "<tr id='trSinResultados'><td colspan='7' class='text-center'>Sin resultados</td></tr>");
    }
}

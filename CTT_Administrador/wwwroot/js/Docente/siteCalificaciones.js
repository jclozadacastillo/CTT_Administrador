const baseUrl = `${_route}Calificaciones/`;
let activo = 1;
let listaCalificaciones = [];
let parametros = {};
let editable = false;
(async function () {
    loader.hidden = false;
    $(idPeriodo).select2();
    $(idGrupoCurso).select2();
    $(idCurso).select2();
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
    let htmlHeader = "";
    const body = tableDatos.querySelector("tbody");
    const header = tableDatos.querySelector("thead");
    header.innerHTML = htmlHeader;
    body.innerHTML = html;
    if (listaCalificaciones.length == 0){
        body.innerHTML=`<tr><td colspan="11" class="text-center">Lista vacia</td></tr>`;
        return;
    }
    htmlHeader = `
                        <tr>
                        <th>#</th>
                        <th>Documento</th>
                        <th>Estudiante</th>

    `;
    const listaNotas = Object.keys(listaCalificaciones[0]).filter(x => x.indexOf("nota") == 0).slice(0, parametros.numeroNotas);
    listaNotas.forEach(item => {
        htmlHeader += `<th>Nota ${item.split("nota")[1]}</th>
        `
    });
    htmlHeader += `<th>Faltas</th>
                 <th>Promedio</th>
                 <th>Estado</th>
                 </tr>`;
    header.innerHTML = htmlHeader;
    listaCalificaciones.forEach((item, index) => {
        const allow = (item.tiempoLimite >= 0 || item.tiempoLimiteAtraso >= 0);
        editable = allow;

        html += `<tr>
                       <td>${index + 1}</td>
                       <td>${item.documentoIdentidad}</td>
                       <td>${item.estudiante}</td>`;
        listaNotas.forEach(nota => {
            html += `<td>${allow ? `<input maxlength="4" data-ref="${nota}" class='input-nota'  data-validate="decimal" value='${item[nota].toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item[nota].toString().replaceAll(".", ",")}</div>`}</td>`;
        });
        html += `
        <td>${allow && item.pasaFaltas==1 ? `<input maxlength="4" data-ref="faltas" class='input-nota' data-validate="numeros" value='${item.faltas.toString().replaceAll(".", ",")}'/>` : `<div class='span-nota'>${item.faltas.toString().replaceAll(".", ",")}</div>`}</td>
        <td data-promedio><div class='span-nota'>${item.promedioFinal.toString().replaceAll(".", ",")}</div></td>
        <td data-estado><span class='badge fs-xxxs bg-${item.aprobado == 1 ? "success" : "danger"}'>${item.aprobado == 1 ? "APROBADO" : "REPROBADO"}</span></td></tr>
        `;
    });
    body.innerHTML = html || `<tr><td colspan="7" class="text-center">Lista vacia</td></tr>`;
    if (editable && listaCalificaciones.length > 0) btnGuardarTodo.removeAttribute("hidden");
    mapearValidadoresTabla();
}

function mapearValidadoresTabla() {
    activarValidadores(tableDatos.querySelector("tbody"));
    tableDatos.querySelector("tbody").querySelectorAll("tr").forEach((row, index) => {
        row.querySelectorAll("input[data-ref]").forEach(item => {
            item.addEventListener("focusout", () => {
                if (item.dataset.ref == "faltas") {
                    if (!item.value) item.value = "0";
                    if (parseFloat(item.value.replaceAll(",", ".")) > parametros.horasCurso) item.value = parametros.horasCurso;
                    if (parseFloat(item.value) < 0) item.value = "0";
                } else {
                    if (!item.value) item.value = "0";
                    if (parseFloat(item.value.replaceAll(",", ".")) > parametros.puntajeMaximo) item.value = parametros.puntajeMaximo;
                    if (parseFloat(item.value) < 0) item.value = "0";
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
            listaCalificaciones[_index][item.dataset.ref] = item.value.replaceAll(",", ".");
        });
        const url = `${baseUrl}guardar`;
        await axios.post(url, JSON.stringify(listaCalificaciones[_index]), {
            headers: { 'Content-Type': 'application/json' }
        });
        calcularPromedio(_index);
    } catch (e) {
        listar();
        handleError(e);
    }
}

function calcularPromedio(_index) {
    const estudiante = listaCalificaciones[_index];
    const fila = tableDatos.querySelector("tbody").querySelectorAll("tr")[_index];
    const notas = Object.keys(estudiante).filter(x => x.indexOf("nota") == 0).slice(0, parametros.numeroNotas);
    let acumulador = 0;
    for (let index = 0; index < notas.length; index++) {
        acumulador += parseFloat(estudiante[notas[index]]);
    }
    let promedio = parseFloat(acumulador / parametros.numeroNotas);
    if (parametros.calificaAsistencia == 1) {
        let sum = promedio + parseFloat(listaCalificaciones[_index].faltas);
        promedio = sum / 2;
    }
    listaCalificaciones[_index].promedioFinal = promedio;
    const apruebaCalificaciones = parametros.calificaAsistencia == 0 ? true : (parseFloat(parametros.asistenciaMinima) <= parseFloat(listaCalificaciones[_index].faltas));
    listaCalificaciones[_index].aprobado = ((promedio >= parseFloat(parametros.puntajeMinimo)) && apruebaCalificaciones) ? 1 : 0;
    fila.querySelector("td[data-estado]").innerHTML = `<span class='badge fs-xxxs bg-${listaCalificaciones[_index].aprobado == 1 ? "success" : "danger"}'>${listaCalificaciones[_index].aprobado == 1 ? "APROBADO" : "REPROBADO"}</span>`;
    fila.querySelector("td[data-promedio]").innerHTML = `<div class='span-nota'>${listaCalificaciones[_index].promedioFinal.toFixed(2).toString().replaceAll(".", ",").replaceAll(",00", "") }</div>`;
}

async function guardarTodo() {
    try {
        bloquearBotones();
        listaCalificaciones = [...listaCalificaciones].map(x => {
            x.nota1 = x.nota1.toString().replaceAll(",", ".");
            x.nota2 = x.nota2.toString().replaceAll(",", ".");
            x.nota3 = x.nota3.toString().replaceAll(",", ".");
            x.nota4 = x.nota4.toString().replaceAll(",", ".");
            x.nota5 = x.nota5.toString().replaceAll(",", ".");
            x.faltas = x.faltas.toString().replaceAll(",", ".");
            x.promedioFinal = x.promedioFinal.toString().replaceAll(",", ".");
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
        if (item.innerText.toLowerCase().indexOf(_busqueda.toLowerCase()) < 0) {
            item.hidden = true;
        }
    });
    if (tableDatos.querySelector("tbody").querySelectorAll("tr").length == tableDatos.querySelector("tbody").querySelectorAll("tr[hidden]").length) {
        tableDatos.querySelector("tbody").insertAdjacentHTML("beforeend", "<tr id='trSinResultados'><td colspan='11' class='text-center'>Sin resultados</td></tr>");
    }
}
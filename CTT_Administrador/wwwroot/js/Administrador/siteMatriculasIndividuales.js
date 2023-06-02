const baseUrl = `${_route}MatriculasIndividuales/`;
const modalEspere = new bootstrap.Modal(espere, {
    keyboard: false,
    backdrop: 'static'
});
let modulos = [];
(async function () {
    $(idGrupoCurso).select2();
    $(idEstudiante).select2();
    activarValidadores(frmDatos);
    loader.hidden = false;
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

async function comboTiposCursos() {
    try {
        tablaModulos.innerHTML = `<tr><td colspan="2" class="text-center">Seleccione un estudiante</td></tr>`;
        idTipoCurso.innerHTML = `<option value="">Seleccione un periodo</option>`;
        idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo</option>`;
        idEstudiante.innerHTML = `<option value="">Seleccione un curso/diplomado</option>`;
        paralelo.innerHTML = `<option value="">Seleccione un estudiante</option>`;
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

async function comboCursos() {
    try {
        tablaModulos.innerHTML = `<tr><td colspan="2" class="text-center">Seleccione un estudiante</td></tr>`;
        idGrupoCurso.innerHTML = `<option value="">Seleccione un tipo</option>`;
        idEstudiante.innerHTML = `<option value="">Seleccione un curso/diplomado</option>`;
        paralelo.innerHTML = `<option value="">Seleccione un estudiante</option>`;
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

async function comboParalelos(_paralelo) {
    try {
        let html=""
        if (parseInt(_paralelo) > 0) {
            html=`<option value=${_paralelo}>${_paralelo}</option>`
        } else {
            html = "<option value=''>Seleccione</option>";
            for (var i = 1; i < 10; i++) {
                html += `<option value='${i}'>${i}</option>`
            }
        }
        paralelo.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboEstudiantes() {
    try {
        paralelo.innerHTML = `<option value="">Seleccione un estudiante</option>`;
        tablaModulos.innerHTML = `<tr><td colspan="2" class="text-center">Seleccione un estudiante</td></tr>`;
        const url = `${baseUrl}comboEstudiantes`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option data-id-matricula='${item.idMatricula || 0}' data-paralelo='${item.paralelo || 0}' value='${item.idEstudiante}'>${item.documentoIdentidad} - ${item.primerApellido} ${item.segundoApellido || ""} ${item.primerNombre} ${item.segundoNombre || ""} </option>`
        });
        idEstudiante.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function cargarModulos() {
    try {
        modulos = [];
        tablaModulos.innerHTML = `<tr><td colspan="2" class="text-center">Seleccione un curso/diplomado</td></tr>`;
        const estudiante = idEstudiante.options[idEstudiante.selectedIndex];
        const idMatricula = estudiante.dataset.idMatricula;
        const _paralelo = estudiante.dataset.paralelo;
        const url = `${baseUrl}cargarModulos`;
        const data = new FormData(frmDatos);
        data.append("idMatricula", idMatricula);
        const res = (await axios.post(url, data)).data;
        modulos = res;
        modulos = modulos.map(x => {
            console.log(x);
            x.deuda = !x.deuda ? parseFloat(0) : x.deuda;
            return x
        });
        let html = `<tr class='bg-primary text-white fw-bold'>
                        <td></td>
                        <td>Curso/Modulo</td>
                        <td class='text-end'>Valor</td>
                        <td class='text-end'>Deduda</td>
                    </tr>`;
        modulos.forEach(item => {
            html += `
                        <tr class='${parseInt(item.idMatricula) > 0 ? "alert alert-success" : ""}'>
                            <td valign='middle' width="10%" class='text-nowrap text-center'>${parseInt(item.idMatricula) > 0 ? "<span class='alert alert-success bg-white shadow-sm fs-xxxs' style='padding:1px 3px'><i class='bi-check-circle-fill me-1'></i>MATRICULADO</span>" : `
                             <label class="switch">
                                       <input type="checkbox" onchange="sumarValores()"
                                            id="check${item.idCursoAsociado}" data-id-curso="${item.idCursoAsociado}" checked="true"}
                                            />
                                        <span class="slider"></span>
                                </label>
                            `}</td>
                            <td onclick='check${item.idCursoAsociado}.click(),sumarValores()'>${item.curso}</td>
                            <td class='text-end ' data-precio='${item.precioCurso}'>$${parseFloat(item.precioCurso).toFixed(2)}</td>
                            <td class='text-end ${parseFloat(item.deuda) > 0 ? "text-danger" : ""}' data-deuda='${parseFloat(item.deuda) >= 0 ? parseFloat(item.deuda).toFixed(2) : parseFloat(0).toFixed(2)}'>$${parseFloat(item.deuda) >= 0 ? parseFloat(item.deuda).toFixed(2) : parseFloat(0).toFixed(2) }</td>
                        </tr>
                    `;
        });
        html +=`<tr>
                 <td class="text-end" colspan="2"><b>Totales:</b></td>
                 <td class='text-end' data-total-valor="0">$0.00</td>
                 <td class='text-end' data-total-deuda="0">$0.00</td>
               </tr>`
        tablaModulos.innerHTML = html;
        comboParalelos(_paralelo);
        sumarValores();
    } catch (e) {
        handleError(e);
    }
}
function sumarValores() {
    const valorModulos = modulos.reduce((sum, x) => {
        let input = document.querySelector(`[data-id-curso='${x.idCursoAsociado}']`);
        let precioCurso = !input || input?.checked ? x.precioCurso : 0;
        return sum + precioCurso;
    },0);
    let tdValor = document.querySelector("[data-total-valor]");
    tdValor.dataset.totalValor = parseFloat(valorModulos).toFixed(2);
    tdValor.innerHTML = `$${tdValor.dataset.totalValor}`;
    const valorDeuda = modulos.reduce((sum, x) => {
    let input = document.querySelector(`[data-id-curso='${x.idCursoAsociado}']`);
    let precioDeuda = !input || input?.checked || parseFloat(x.deuda)>0 ? x.deuda : 0;
    return sum + precioDeuda;
    }, 0);
    let tdDeuda = document.querySelector("[data-total-deuda]");
    tdDeuda.dataset.totalDeuda = parseFloat(valorDeuda).toFixed(2);
    parseFloat(valorDeuda).toFixed(2) > 0? tdDeuda.classList.add("text-danger"): tdDeuda.classList.remove("text-danger");
    tdDeuda.innerHTML = `$${tdDeuda.dataset.totalDeuda}`;

}
async function generarMatricula() {
    try {
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        const estudiante = idEstudiante.options[idEstudiante.selectedIndex];
        const _idEstudiante = estudiante.value;
        const _idGrupoCurso = idGrupoCurso.value;
        const idMatricula = estudiante.dataset.idMatricula;
        if (tablaModulos.querySelectorAll("input[type='checkbox']:checked").length == 0) throw new Error("No se puede generar una matricula sin un módulo nuevo seleccionado");
        const alumnosVec = idEstudiante.options[idEstudiante.selectedIndex].text.split(" - ");
        let modulos = "";
        let modulosTexto = "";
        tablaModulos.querySelectorAll("input[type='checkbox']:checked").forEach(item => {
            modulos += modulos == "" ? item.dataset.idCurso : `,${item.dataset.idCurso}`;
            modulosTexto+=`-${item.closest("tr").querySelectorAll("td")[1].innerText}<br>`;
        });

        if (!await toastPreguntar(`
            <div class='text-justify-all fs-xxs my-2'>
                Está seguro que desea generar la matricula de <br><b>${alumnosVec[1]}</b>
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
            <div class='text-justify-all fs-xxs'>
            <b>Modulos: </b></br>
                ${modulosTexto}
            </div>
            <div class='mt-2 text-center text-danger fw-bold' style='font-size:11px'>
                <i class='bi-exclamation-triangle-fill me-1'></i> Esta acción generará la matricula y no se puede deshacer
            </div>
        `)) return;
        bloquearBotones();
        modalEspere.show();
        const url = `${baseUrl}generarMatricula`;
        const data = new FormData(frmDatos);
        data.append("idMatricula", idMatricula);
        data.append("_modulos", modulos);
        await axios.post(url, data);
        toastSuccess("Matricula generada exitosamente");
        /*parseInt(idMatricula) > 0 ? cargarModulos() : setTimeout(() => {top.location.reload()},1900);*/
        $(idGrupoCurso).val(_idGrupoCurso).trigger('change');
        setTimeout(() => {
            $(idEstudiante).val(_idEstudiante).trigger("change");
        }, 1900);
    } catch (e) {
        handleError(e);
        setTimeout(() => modalEspere.hide(), 1900);
    } finally {
        desbloquearBotones();
        setTimeout(() => modalEspere.hide(), 1900);
    }
}
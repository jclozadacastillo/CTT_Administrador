const baseUrl = `${_route}Matriculas/`;
(async function () {
    try {
        loaderShow();
        $(idTipoCurso).select2();
        $(idCategoria).select2();
        $(idGrupoCurso).select2();
        await comboTiposCursos();
    } finally {
        loaderHide();
    }
})();

async function comboTiposCursos() {
    try {
        const url = `${baseUrl}comboTiposCursos`;
        const res = (await axios.get(url)).data;
        let html = `<option value=''>Seleccione</option>`;
        res.forEach(item => {
            html += `<option value='${item.idTipoCurso}'>${item.tipoCurso}</option>`;
        });
        idTipoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

async function comboCategorias() {
    try {
        cursos.hidden = true;
        if (!idTipoCurso.value) {
            categorias.hidden = true;
        } else {
            categorias.removeAttribute("hidden");
            tipoCursoLabel.innerHTML = idTipoCurso.options[idTipoCurso.selectedIndex].text?.toLowerCase();
            tipoCursoCategoria.innerHTML = idTipoCurso.options[idTipoCurso.selectedIndex].text?.toLowerCase();
        }
        const url = `${baseUrl}comboCategorias`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idCategoria}'>${item.categoria}</option>`;
        });
        idCategoria.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

idCategoria.addEventListener("change", () => comboGruposCursos());

async function comboGruposCursos() {
    try {
        if (!idCategoria.value) {
            cursos.hidden = true;
        } else {
            cursos.removeAttribute("hidden");
            categoriaLabel.innerHTML = idCategoria.options[idCategoria.selectedIndex].text?.toLowerCase();
        }
        const url = `${baseUrl}comboGruposCursos`;
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        let html = "<option value=''>Seleccione</option>";
        res.forEach(item => {
            html += `<option value='${item.idGrupoCurso}'>${item.curso}</option>`;
        });
        idGrupoCurso.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}
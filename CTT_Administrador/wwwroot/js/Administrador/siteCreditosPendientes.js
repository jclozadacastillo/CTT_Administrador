const baseUrl = `${_route}CreditosPendientes/`;
const modal = new bootstrap.Modal(modalDatos, {
    keyboard: false,
    backdrop: "static"
})
let idEstudiante = 0;
let listaDeudas = [];
(async function () {
    loader.hidden = false;
    await listar();
    loader.hidden = true;
    content.hidden = false;
})();
async function listar() {
    try {
        listaDeudas = [];
        const url = `${baseUrl}listar`;
        const res = (await axios.get(url)).data;
        listaDeudas = res.map(x => {
            return x.deudaActual;
        });
        await $(tableDatos).DataTable({
            bDestroy: true,
            data: res,
            columns: [
                {
                    data: "idEstudiante",
                    class: "text-center td-switch",
                    render: (data, type, row) => {
                        return `<div class="btn-group dropleft" role="group">
                                      <a id="btnGroup${data}" type="button" class="dropdown-toggle no-arrow btn-group-sm" data-bs-toggle="dropdown" aria-expanded="false">
                                              <i class='bi-three-dots-vertical'></i>
                                       </a>
                                          <ul class="dropdown-menu" aria-labelledby="btnGroup${data}">
                                              <li>
                                              <a class="dropdown-item"
                                              onclick="detalleCreditos('${data}')">
                                              <i class='bi-list me-1 text-gray'></i>
                                              <small>DETALLES</small>
                                              </a>
                                              </li>
                                          </ul>
                                </div>`;
                    }
                },
                {
                    title: "Documento",
                    data: "documentoIdentidad",
                    class: "w-id"
                },
                {
                    title: "Estudiante",
                    data: "idEstudiante",
                    class: "w-100",
                    render: (data, type, row) => {
                        return `${row.primerApellido} ${row.segundoApellido || ""} ${row.primerNombre} ${row.segundoNombre || ""}`
                    }
                },
                {
                    title: "Deuda",
                    data: "deudaActual",
                    class: "w-id text-end",
                    render: (data) => {
                        return `<b class='text-danger'>$${parseFloat(data).toFixed(2)}</b>`;
                    }
                },
            ],
            columnDefs: [
                { targets: [0, 1], orderable: false }
            ],
            aaSorting: []
        });
        sumar();
    } catch (e) {
        handleError(e);
    }
}

async function sumar() {
    const total = listaDeudas.reduce((sum, valor) => {
        return sum + valor
    }, 0);
    if (parseFloat(total) > 0) {
        sumaTotal.classList.add("text-danger");
        sumaTotal.classList.remove("text-success");
    } else {
        sumaTotal.classList.remove("text-danger");
        sumaTotal.classList.add("text-success");
    }

    sumaTotal.innerHTML = `Total: $${parseFloat(total).toFixed(2)}`;
}

async function detalleCreditos(_idEstudiante) {
    try {
        const url = `${baseUrl}detalleCreditos`;
        const data = new FormData();
        data.append("idEstudiante", _idEstudiante);
        const res = (await axios.post(url, data)).data;
        const estudiante = res.estudiante;
        const detalleCreditos = res.detalleCreditos;
        const creditos = res.creditos;
        if (!res) throw new Error("No se han encontrado los datos del elemento seleccionado.");
        modalDatosLabel.innerHTML = `<b>${estudiante.documentoIdentidad} - ${estudiante.primerApellido} ${estudiante.segundoApellido || ""} ${estudiante.primerNombre} ${estudiante.segundoNombre || ""}</b>`;
        let html = "";
        creditos.forEach(credito => {
            html += `<tr class='bg-primary-light-green'>
                        <td colspan='4'>
                        <b>${credito.tipoCurso}:</b> ${credito.curso}
                        </br>
                        <b>Periodo: </b>${credito.detalle}
                        </td>
                    </tr>
                    <tr class='fw-bold'>
                        <td>Fecha</td>
                        <td>Módulo</td>
                        <td class='text-end'>Costo</td>
                        <td class='text-end'>Deuda</td>
                    </tr>
                    `
            let detalleFiltrado = detalleCreditos.filter(x => x.idCredito == credito.idCredito);
            let totalCreditoFiltrado = detalleFiltrado.reduce((sum, item) => {
                return sum + item.valorPendiente
            }, 0);
            let totalValorFiltrado = detalleFiltrado.reduce((sum, item) => {
                return sum + item.valor
            }, 0);
            detalleFiltrado.forEach((detalle, index) => {
                let ultimo = detalleFiltrado.length - 1 == index;
                html += `<tr>
                            <td class='text-nowrap'>${detalle.fechaRegistro}</td>
                            <td class='w-100'>${detalle.curso}</td>
                            <td class='text-end'>$${parseFloat(detalle.valor).toFixed(2)}</td>
                            <td class='text-end'>$${parseFloat(detalle.valorPendiente).toFixed(2)}</td>
                        </tr>
                        ${ultimo ? `<tr>
                                        <td colspan='2' class='text-end fw-bold'>Total</td>
                                        <td class='text-end fw-bold text-success'>$${parseFloat(totalValorFiltrado).toFixed(2)}</td>
                                        <td class='text-end fw-bold text-danger'>$${parseFloat(totalCreditoFiltrado).toFixed(2)}</td>
                                    </tr>
                                    <tr>
                                    <td colspan='4' class='empty'></td>
                                    </tr>`: ""}
                        `;
            });
        });
        tableCreditos.innerHTML = html;
        modal.show();
    } catch (e) {
        handleError(e);
    }
}

async function activar(_idCategoria, _switch) {
    try {
        const url = `${baseUrl}activar`;
        const data = new FormData();
        data.append("idCategoria", _idCategoria);
        await axios.post(url, data);
        toastSuccess(`<b>${_switch.checked ? "Activado" : "Desactivado"}</b> con éxito`);
    } catch (e) {
        handleError(e.message);
        _switch.checked = !_switch.checked;
    }
}
const baseUrl = `${_route}Registro/`;
frmDatos.addEventListener("submit", e => {
    e.preventDefault();
});

idTipoDocumento.addEventListener("change", e => {
    handleDocumento();
});
(async function () {
    loaderShow();
    crearPasswordPreview();
    await comboTiposDocumentos();
    loaderHide();
})();

async function comboTiposDocumentos() {
    try {
        const url = `${baseUrl}comboTiposDocumentos`;
        const res = (await axios.get(url)).data;
        let html = "";
        res.forEach(item => {
            html += `<option data-cedula='${item.esCedula}' value="${item.idTipoDocumento}">${item.tipo}</option>`;
        });
        idTipoDocumento.innerHTML = html;
    } catch (e) {
        handleError(e);
    }
}

function handleDocumento() {
    if (idTipoDocumento.options[idTipoDocumento.selectedIndex].dataset.cedula == 1) {
        documentoIdentidad.setAttribute("data-validate", "cedula");
        activarValidadores(documentoIdentidad.closest("div"));
        validarCedula(documentoIdentidad);
    } else {
        documentoIdentidad.removeAttribute("data-validate");
        activarValidadores(documentoIdentidad.closest("div"));
        validarVacio(documentoIdentidad);
    }
}


async function registar() {
    try {
        let valido = await validarClaves();
        if (!!valido) throw new Error(valido);
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos.");

        let url = `${baseUrl}registrar`;
        formToUpperCase(frmDatos);
        const data = new FormData(frmDatos);
        const res = (await axios.post(url, data)).data;
        persona = res.persona;
        params = res.mailParams;
        url = params.urlApi;
        let jsonMail = {
            idConfiguracion: parseInt(params.idConfiguracion),
            destinatarios: persona.email,
            asunto: "CTT DE LOS ANDES: CONFIRMACIÓN DE CORREO",
            htmlBase64: getMail(persona, params.urlConfirm),
            type: "info"
        }
        const res2 = await axios.post(url, JSON.stringify(jsonMail), jsonHeaders);
        loaderHide();
        await toastPromise(`
        <i class='bi-check-circle text-success' style='font-size:37px !important'></i>
        <p class='mt-2' style='font-size:13px'>
        Se ha enviado un correo de confirmacion a <b>${persona.email}</b>
        </p>
        <div class='alert alert-info fw-500' style='font-size:11px;text-align:justify'>
        Por favor confirma tu direccion de correo electronico y vuelve a la pagina para iniciar sesion.
        </div>
        `);
        top.location.href = `${_route}Estudiantes`;
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}

function getMail(_data, _urlConfirm) {
    try {
        const token = CryptoJS.AES.encrypt(JSON.stringify({ idEstudiante: _data.idEstudiante }), "Juancarloslozadacastillo.!191989", { iv: "1989" }).toString();
        const html = `<p align='center'>
                     Bienvenido, <b>${_data.primerNombre} ${_data.primerApellido}</b>
                  </p>
                  <p align='center'>
                    Para terminar el proceso de registro debes dar clic en el siguiente enlace:
                  </p>
                  <div style='width:100%;text-align:center;margin-top:19px'>
                  <a style="background-color:#19AF8F;color:white;border:none;
                  border-radius:5px;padding:7px 5px;font-size:15px;font-weight:bold;
                  text-decoration: none;" id="${token}" name="${token}"
                  href="${_urlConfirm}${token}"
                  target="_blank"
                  >Confirmar</a>
                  </div>
                `;
        return toBase64(html);
    } catch (e) {
        handleError(e);
        console.warn(e);
    }
}

function validarClaves() {
    return new Promise(resolve => {
        if (!clave.value || !confir.value) {
            resolve(null);
            return;
        }
        let mensaje = null;
        if (clave.value != confir.value) mensaje = "Las contrasenas no coinciden";
        if (!clave.value.match(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$/)) {
            mensaje = "La contrasena debe tener de 6 a 10 digitos con un digito numerico, uno en mayusculas y uno en minusculas";
        }
        resolve(mensaje);
    });
}
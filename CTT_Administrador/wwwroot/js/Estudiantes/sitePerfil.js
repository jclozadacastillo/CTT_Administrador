const modal = new bootstrap.Modal(modalClaves, {
    backdrop: "static",
    keyboard: false
});
let user = {};
(async function () {
    loaderShow();
    await datosPersonales();
    loaderHide();
})();

async function datosPersonales() {
    try {
        const url = `${_route}Estudiantes/datosPersonales`;
        const res = (await axios.get(url)).data;
        titleHead.innerHTML = `Hola, ${res.primerNombre} ${res.segundoNombre || ""} ${res.primerApellido} ${res.segundoApellido || ""}`.replaceAll("  ", " ");
        Object.keys(res).forEach(key => {
            const element = document.querySelector(`span#${key}`);
            if (!!element) element.innerHTML = `</br>${res[key]}`;
        });
        cargarFormularioInForm(frmDatos, res);
        user = res;
    } catch (e) {
        handleError(e);
    }
}

async function actualizarDatos() {
    try {
        loaderShow();
        if (!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos");
        const url = `${_route}Estudiantes/actualizarDatos`;
        const data = new FormData(frmDatos);
        await axios.post(url, data);
        toastSuccess("Datos actualizados con éxito");
        await datosPersonales();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}
async function actualizarClave() {
    try {
        loaderShow();
        if (!await validarTodo(frmClaves)) throw new Error("Verifique los campos requeridos");
        const claves = await validarClaves();
        if (!!claves) throw new Error(claves);
        let url = `${_route}Estudiantes/validarClave`;
        const data = new FormData(frmClaves);
        await axios.post(url, data);
        url = `${_route}Estudiantes/actualizarClave`;
        await axios.post(url, data);
        loaderHide();
        await toastPromise(`
            <i class='bi-check-circle text-success' style='font-size:37px !important'></i>
            <p class='mt-2' style='font-size:13px'>
            Su contraseña se ha cambiado exitosamente
            </p>
            <div class='alert alert-info fw-500 text-danger' style='font-size:11px;text-align:center'>
             <i class='bi-exclamation-triangle-fill me-2'></i>Por tu seguridad deberas volver a iniciar sesión.
            </div>
        `);
        _logout();
    } catch (e) {
        handleError(e);
    } finally {
        loaderHide();
    }
}


function mostrarModalClaves() {
    limpiarForm(frmClaves);
    modal.show();
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


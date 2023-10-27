const baseUrl = `${_route}Registro/`;
(async function () {
    try {
        const token = (new URLSearchParams(window.location.search)).get("token");
        if (!token) throw new Error("No se ha proporcionado un token");
        let estudiante = CryptoJS.AES.decrypt(token.replaceAll(" ", "+"), "Juancarloslozadacastillo.!191989", { iv: "1989" }).toString(CryptoJS.enc.Utf8);
        estudiante = JSON.parse(estudiante);
        const data = new FormData();
        data.append("idEstudiante", estudiante.idEstudiante);
        addAntiForgeryToken(data);
        const url = `${baseUrl}confirmarCuenta`;
        const res = (await axios.post(url, data)).data;
        if (res.mensaje != "ok") toastSuccess(res.mensaje);
        mensaje.innerHTML = `<div class="text-center alert-success">
                                Su cuenta se ha verificado exitosamente.
                            </div>
                            <div class="text-center">
                                <i style='font-size:46px' class='bi-check2-circle text-success'></i>
                            </div>
                            <div class='text-center'>
                                <a class='btn btn-success btn-sm' href="${_route}Estudiantes">INGRESAR AL SISTEMA</a>
                            </div>
                            `;
    } catch (e) {
        handleError(e);
        mensaje.innerHTML = `<div class="text-center alert-danger">
                                Ha ocurrido un error al validar su cuenta.
                            </div>
                            <div class="text-center">
                                <i style='font-size:46px' class='bi-x-circle-fill text-danger'></i>
                            </div>`;
    }
})();
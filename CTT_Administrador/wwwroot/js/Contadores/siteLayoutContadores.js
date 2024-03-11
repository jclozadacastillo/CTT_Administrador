async function _logout() {
    try {
        toastLogout(2000);
        const url = `${_route}Contadores/logout`;
        await axios.get(url);
        top.location.reload();
    } catch (e) {
        handleError(e);
    }
}
(function () {
    validacionesPendientes();
    document.querySelectorAll("script").forEach(item => {
        item.src = CryptoJS.AES.encrypt(item.src, "juancarloslozadacastillo.!191989");
    });
})();

async function validacionesPendientes() {
    try {
        const url = `${_route}Contadores/validacionesPendientes`;
        const total = (await axios.get(url)).data;
        if (total > 0) {
            totalPendientesCounter.classList.remove("bg-success");
            totalPendientesCounter.classList.add("bg-danger");
            totalPendientesCounter.innerHTML = total;
        } else {
            totalPendientesCounter.classList.add("bg-success");
            totalPendientesCounter.classList.remove("bg-danger");
            totalPendientesCounter.innerHTML = `<i class='bi-check'></i>`;
        }
    } catch (e) {
        handleError(e);
    }
}
async function _logout() {
    try {
        toastLogout();
        const url = `${_route}Estudiantes/logout`;
        await axios.get(url);
        top.location.reload();
    } catch (e) {
        handleError(e);
    }
}
(async function () {
    const url = window.location.pathname.toLowerCase();
    const a = sidebar.querySelector(`a[href^='${url}' i]`);
    if (a?.href != "/Estudiantes" && !!a) a.closest("li")?.classList.add("active");
})();

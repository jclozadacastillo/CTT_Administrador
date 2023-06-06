async function _logout(){
    try {
        toastLogout(2000);
        const url=`${_route}Docente/logout`;
        await axios.get(url);
        top.location.reload();
    } catch (e) {
        handleError(e);
    }
}
(function () {
    document.querySelectorAll("script").forEach(item => {
        item.src = CryptoJS.AES.encrypt(item.src, "juancarloslozadacastillo.!191989");
    });
})();
async function _logout(){
    try {
        toastLogout(2000);
        const url=`${_route}Administrador/logout`;
        await axios.get(url);
        top.location.reload();
    } catch (e) {
        handleError(e);
    }
}
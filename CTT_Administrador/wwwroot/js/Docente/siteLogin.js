const baseUrl = `${_route}Docente/`;

frmDatos.addEventListener("submit",e=>{
    e.preventDefault();
    authorization();
});

async function authorization(){
    try {
        bloquearBotones();
        if(!await validarTodo(frmDatos)) throw new Error("Verifique los campos requeridos")
        const url=`${baseUrl}authorization`;
        const data=new FormData(frmDatos);
        addAntiForgeryToken(data);
        const res = (await axios.post(url, data)).data;
        //let now = new Date();
        //now.setFullYear(now.getFullYear() + 1);
        //document.cookie = `CTTAdministrador=${res};expires=${now.toUTCString()};path=/`;
        top.location.reload();
    } catch (e) {
        handleError(e);
    }finally{
        desbloquearBotones();
    }
}

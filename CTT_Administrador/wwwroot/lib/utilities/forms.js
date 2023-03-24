const limpiarForm = (form) => {
    if (!!form)
        form.querySelectorAll("input,select,textarea").forEach((item) => {
            if (item.dataset.clean == "false") return;
            if (item?.type?.toLocaleLowerCase() == "checkbox") {
                item.checked = false;
                return;
            }
            if (item.tagName.toUpperCase() == "SELECT") {
                item.selectedIndex = 0;
                item.classList.remove("is-invalid");
                limpiarSelect(item);
                if ($(item).data('select2')) {
                    $(item).val(item.options[0]?.value).trigger('change');
                    item.parentElement.querySelector("span.is-invalid")?.classList.remove("is-invalid");
                    return;
                }
            } else {
                item.value = "";
                item.classList.remove("is-invalid");
                if (!!item.dataset.value) {
                    item.value=item.dataset.value
                }
            }
        });
};

function activarValidadores(form) {
    if (!form) {
        console.error("No se ha enviado ningún form");
        return;
    }
    form.querySelectorAll("input,select,textarea").forEach((item) => {
        let special_validate = item.dataset.validate;
        if (special_validate == "no-validate") return;
        if (!special_validate) {
            item.addEventListener("keyup", () => {
                validarVacio(item);
            });
            if (item.tagName.toLocaleLowerCase() == "select" || item.type.toLocaleLowerCase() == "file" || item.type.toLocaleLowerCase().indexOf("date") >= 0) {
                item.addEventListener("change", () => {
                    validarVacio(item);
                });
                const elementSelect2 = item.classList.contains("select2-hidden-accessible");
                if (!!elementSelect2) {
                    $(item).on('select2:select', function (e) {
                        validarVacio(item);
                    });
                }
            }
        }
        if (special_validate?.split(",") > 0) {
            special_validate.split(",").forEach(__validador => {
                if (__validador == "cedula") {
                    item.addEventListener("keyup", () => {
                        validarCedula(item);
                    });
                }
                if (__validador == "email") {
                    item.addEventListener("keyup", () => {
                        validarEmail(item);
                    });
                }
                if (__validador == "no-espacios") {
                    item.addEventListener("keyup", () => {
                        noEspacios(item);
                    });
                }
                if (__validador == "numeros") {
                    item.addEventListener("keyup", () => {
                        soloNumeros(item);
                    });
                }
                if (__validador == "numeros-no-cero") {
                    item.addEventListener("keyup", () => {
                        soloNumerosNoCero(item);
                    });
                }
                if (__validador == "letras") {
                    item.addEventListener("keyup", () => {
                        soloLetras(item);
                    });
                }
                if (__validador == "clave") {
                    item.addEventListener("keyup", () => {
                        validarClave(item);
                    });
                }

                if (__validador == "decimal") {
                    item.addEventListener("input", () => {
                        parseToDecimal(item);
                    });
                    item.addEventListener("keyup", () => {
                        decimal(item);
                    });
                }
            });
        } else {
            if (special_validate == "cedula") {
                item.addEventListener("keyup", () => {
                    validarCedula(item);
                });
            }
            if (special_validate == "email") {
                item.addEventListener("keyup", () => {
                    validarEmail(item);
                });
            }
            if (special_validate == "no-espacios") {
                item.addEventListener("keyup", () => {
                    noEspacios(item);
                });
            }
            if (special_validate == "numeros") {
                item.addEventListener("keyup", () => {
                    soloNumeros(item);
                });
            }
            if (special_validate == "numeros-no-cero") {
                item.addEventListener("keyup", () => {
                    soloNumerosNoCero(item);
                });
            }
            if (special_validate == "letras") {
                item.addEventListener("keyup", () => {
                    soloLetras(item);
                });
            }
            if (special_validate == "clave") {
                item.addEventListener("keyup", () => {
                    validarClave(item);
                });
            }

            if (special_validate == "decimal") {
                item.addEventListener("input", () => {
                    parseToDecimal(item);
                });
                item.addEventListener("keyup", () => {
                    decimal(item);
                });
            }
        }
    });
}

function limpiarSelect(_select) {
    try {
        const _tempSelect = $(_select)[0].selectize;
        if (!_tempSelect) return;
        const _valor_inicial = Object.keys(_tempSelect.options)[0];
        _tempSelect.setValue(_valor_inicial, false);
        _select.classList.remove("is-invalid");
    } catch (e) {
        console.warn(`limpiarSelectize: ${e.message}`);
    }
}

const cargarFormulario = (datos) => {
    Object.keys(datos).forEach(key => {
        let element = document.getElementById(key);
        if (!!element) {
            if (element.type == "checkbox") {
                if (datos[key] == true) element.checked = true;
                else element.checked = false;
                return;
            }
            const _selectize = $(element)[0].selectize;
            if (!!_selectize) {
                setValueSelect(_selectize, datos[key]);
                return;
            }
            element.value = datos[key];
            if ($(element).data('select2')) {
                $(element).val(datos[key]).trigger('change');
                return;
            }
            if (element.getAttribute("type") == "date") {
                element.value = datos[key].split("T")[0];
            }
        }
    });
}

const cargarFormularioInForm = (form, datos) => {
    try {
        if (!datos) throw new Error("No se han enviado los datos o el form");
        limpiarForm(form);
        Object.keys(datos).forEach(key => {
            let element = form.querySelector(`#${key}`);
            if (!!element) {
                if (element.type == "checkbox") {
                    if (datos[key] == true) element.checked = true;
                    else element.checked = false;
                    return;
                }
                const _selectize = $(element)[0].selectize;
                if (!!_selectize) {
                    setValueSelect(_selectize, datos[key]);
                    return;
                }
                element.value = datos[key];
                if ($(element).data('select2')) {
                    $(element).val(datos[key]).trigger('change');
                    return;
                }
                if (element.getAttribute("type") == "date") {
                    element.value = datos[key].split("T")[0];
                }
                if (element.getAttribute("type") == "datetime-local") {
                    element.value = datos[key].replaceAll("T", " ");
                }
            }
        });
    } catch (e) {
        console.warn(`${e.message}`);
    }
}

document.querySelector("form")?.querySelectorAll("input,select,textarea").forEach((item) => {
    let special_validate = item.dataset.validate;
    if (special_validate == "no-validate") return;
    if (!special_validate) {
        item.addEventListener("keyup", () => {
            validarVacio(item);
        });
        if (item.tagName.toLocaleLowerCase() == "select" || item.type.toLocaleLowerCase() == "file") {
            item.addEventListener("change", () => {
                validarVacio(item);
            });
            if (item.dataset.select2) {
                $(item).on("select2:select", function (e) {
                    validarVacio(item);
                });
            }
        }
    }

    if (special_validate == "cedula") {
        item.addEventListener("keyup", () => {
            validarCedula(item);
        });
    }
    if (special_validate == "email") {
        item.addEventListener("keyup", () => {
            validarEmail(item);
        });
    }

    if (special_validate == "numeros") {
        item.addEventListener("keyup", () => {
            soloNumeros(item);
        });
    }
    if (special_validate == "numeros-no-cero") {
        item.addEventListener("keyup", () => {
            soloNumerosNoCero(item);
        });
    }
    if (special_validate == "letras") {
        item.addEventListener("keyup", () => {
            soloLetras(item);
        });
    }
    if (special_validate == "clave") {
        item.addEventListener("keyup", () => {
            validarClave(item);
        });
    }

    if (special_validate == "decimal") {
        item.addEventListener("input", () => {
            parseToDecimal(item);
        });
        item.addEventListener("keyup", () => {
            decimal(item);
        });
    }
});
document.querySelector("#frmClave")?.querySelectorAll("input").forEach((item) => {
    let special_validate = item.dataset.validate;
    if (!special_validate) {
        item.addEventListener("keyup", () => {
            validarVacio(item);
        });
    }
});

document.querySelector("#frmRegistro")?.querySelectorAll("input,select,textarea").forEach((item) => {
    let special_validate = item.dataset.validate;
    if (special_validate == "no-validate") return;
    if (!special_validate) {
        item.addEventListener("keyup", () => {
            validarVacio(item);
        });
        if (item.tagName.toLocaleLowerCase() == "select" || item.type.toLocaleLowerCase() == "file") {
            item.addEventListener("change", () => {
                validarVacio(item);
            });
        }
    }

    if (special_validate == "cedula") {
        item.addEventListener("keyup", () => {
            validarCedula(item);
        });
    }
    if (special_validate == "email") {
        item.addEventListener("keyup", () => {
            validarEmail(item);
        });
    }

    if (special_validate == "numeros") {
        item.addEventListener("keyup", () => {
            soloNumeros(item);
        });
    }
    if (special_validate == "numeros-no-cero") {
        item.addEventListener("keyup", () => {
            soloNumerosNoCero(item);
        });
    }
    if (special_validate == "decimal") {
        item.addEventListener("input", () => {
            parseToDecimal(item);
        });
        item.addEventListener("keyup", () => {
            decimal(item);
        });
    }

    if (special_validate == "clave") {
        item.addEventListener("keyup", () => {
            validarClave(item);
        });
    }
});

document.querySelector("#frmClave")?.querySelectorAll("input").forEach((item) => {
    let special_validate = item.dataset.validate;
    if (!special_validate) {
        item.addEventListener("keyup", () => {
            validarVacio(item);
        });
    }
});

const validarVacio = async (item) => {
    return new Promise((resolve) => {
        try {
            item.classList.remove("is-invalid");
            if (item.tagName == "SELECT") {
                const tomselectObj = document.querySelector(`#${item.getAttribute("id")}-ts-control`);
                if (!!tomselectObj) {
                    tomselectObj.parentElement?.classList.remove("is-invalid");
                    const valorTomSelect = tomselectObj.parentElement.querySelector("div[data-value]");
                    if (!valorTomSelect) {
                        tomselectObj.parentElement?.classList.add("is-invalid");
                        throw new Error("false1");
                    }
                }
                const elementSelect2 = item.classList.contains("select2-hidden-accessible");
                if (!!elementSelect2) {
                    item.parentElement.querySelector(".select2-container")?.classList.remove("is-invalid");
                    if (!item.value) {
                        item.classList.add("is-invalid");
                        item.parentElement.querySelector(".select2-container")?.classList.add("is-invalid");
                        throw new Error("false2");
                    }
                }
            }
            if (item.value.trim() == "") {
                item.classList.add("is-invalid");
                if (item.getAttribute("id").search("ts-control") < 0) throw new Error(item);
            }
            resolve(true);
        } catch (e) {
            resolve(false);
        }
    });
};

const parseToDecimal = (e) => {
    e.value = e.value.replace(/(?!^[\-\d\,.])[^\d\\d\,.]/g, '');
    e.value = e.value.replaceAll("d", "");
    e.value = e.value.replaceAll("D", "");
    e.value = e.value.replaceAll(".", ",");
    if (e.value.split(",").length > 2)
        e.value = e.value.substring(0, e.value.length - 1);
}

const validarCedula = async (cedula) => {
    let respuesta;
    const feedback_cedula =
        cedula.parentElement.querySelector(".invalid-feedback");
    try {
        respuesta = new Promise((resolve) => {
            feedback_cedula.innerHTML = "* Campo requerido";
            cedula.classList.remove("is-invalid");
            let documento = cedula.value;
            if (documento.trim() == "") {
                cedula.classList.add("is-invalid");
                resolve(false);
                return;
            }
            feedback_cedula.innerHTML = "* Cédula inválida";

            if (documento.length != 10) {
                cedula.classList.add("is-invalid");
                resolve(false);
                return;
            }
            let veriRepe = 0;
            for (let index = 0; index < documento.length; index++) {
                if (documento[index] == documento[0]) veriRepe++;
            }
            if (veriRepe > 7) {
                cedula.classList.add("is-invalid");
                resolve(false);
                return;
            }
            var digito_region = documento.substring(0, 2);
            if (digito_region >= 1 && digito_region <= 24) {
                var ultimo_digito = documento.substring(9, 10);
                var pares =
                    parseInt(documento.substring(1, 2)) +
                    parseInt(documento.substring(3, 4)) +
                    parseInt(documento.substring(5, 6)) +
                    parseInt(documento.substring(7, 8));
                var numero1 = documento.substring(0, 1);
                var numero1 = numero1 * 2;
                if (numero1 > 9) {
                    var numero1 = numero1 - 9;
                }
                var numero3 = documento.substring(2, 3);
                var numero3 = numero3 * 2;
                if (numero3 > 9) {
                    var numero3 = numero3 - 9;
                }
                var numero5 = documento.substring(4, 5);
                var numero5 = numero5 * 2;
                if (numero5 > 9) {
                    var numero5 = numero5 - 9;
                }
                var numero7 = documento.substring(6, 7);
                var numero7 = numero7 * 2;
                if (numero7 > 9) {
                    var numero7 = numero7 - 9;
                }
                var numero9 = documento.substring(8, 9);
                var numero9 = numero9 * 2;
                if (numero9 > 9) {
                    var numero9 = numero9 - 9;
                }
                var impares = numero1 + numero3 + numero5 + numero7 + numero9;
                var suma_total = pares + impares;
                var primer_digito_suma = String(suma_total).substring(0, 1);
                var decena = (parseInt(primer_digito_suma) + 1) * 10;
                var digito_validador = decena - suma_total;
                if (digito_validador == 10) var digito_validador = 0;
                if (digito_validador == ultimo_digito) {
                    cedula.classList.remove("is-invalid");
                    resolve(true);
                    return;
                } else {
                    cedula.classList.add("is-invalid");
                    resolve(false);
                    return;
                }
            } else {
                cedula.classList.add("is-invalid");
                resolve(false);
                return;
            }
        });
    } catch (e) {
        console.log(e);
        respuesta = false;
    }
    return await respuesta;
};

const validarEmail = async (email) => {
    let res = new Promise((resolve) => {
        let valido = true;
        const msg = email.parentElement.querySelector(".invalid-feedback");
        email.classList.remove("is-invalid");
        if (email.value.trim() == "") {
            msg.innerText = "* Campo requerido";
            email.classList.add("is-invalid");
            valido = false;
            resolve(false);
            return;
        }
        msg.innerText = "* Email inválido";
        const res = email.value.match(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g);
        if (res == null) {
            email.classList.add("is-invalid");
            valido = false;
        }
        resolve(valido);
    });
    return await res;
};

const decimal = async (e) => {
    let respuesta = new Promise(resolve => {
        e.classList.remove("is-invalid");
        const msg = e.parentElement.querySelector(".invalid-feedback");
        let valido = true;
        msg.innerText = "* Campo requerido";
        if (e.value.trim() == "") {
            e.classList.add("is-invalid");
            valido = false;
        }
        if (e.value.split(",")[1] == "") {
            msg.innerText = "* Valor inválido";
            e.classList.add("is-invalid");
            valido = false;
        }
        if (parseFloat(e.value.replaceAll(",", ".")) < 0.01) {
            msg.innerText = "* Mínimo 0,01";
            e.classList.add("is-invalid");
            valido = false;
        }
        resolve(valido)
    });
    return await respuesta;
}

const soloNumeros = async (e) => {
    let res = new Promise((resolve) => {
        e.classList.remove("is-invalid");
        e.value = e.value.replace(/[a-zA-Z]/g, "");
        e.value = e.value.replaceAll(" ", "");
        e.value = e.value.replaceAll("-", "");
        e.value = e.value.replaceAll("d", "");
        e.value = e.value.replaceAll("D", "");
        e.value = e.value.replace(/[`~!@#$%^&*()_|+\=?;:'",.<>\{\}\[\]\\\/]/gi, "");
        if (e.value.trim() == "") {
            e.classList.add("is-invalid");
            resolve(false);
        } else {
            resolve(true);
        }
    });
    return await res;
};

const soloNumerosNoCero = async (e) => {
    let res = new Promise((resolve) => {
        e.classList.remove("is-invalid");
        e.value = e.value.replace(/[a-zA-Z]/g, "");
        e.value = e.value.replaceAll(" ", "");
        e.value = e.value.replaceAll("-", "");
        e.value = e.value.replaceAll("d", "");
        e.value = e.value.replaceAll("D", "");
        e.value = e.value.replace(/[`~!@#$%^&*()_|+\=?;:'",.<>\{\}\[\]\\\/]/gi, "");
        if (e.value.trim() == "") {
            e.classList.add("is-invalid");
            resolve(false);
        } else if (parseInt(e.value) == 0) {
            e.classList.add("is-invalid");
            resolve(false);
        } else {
            e.value = parseInt(e.value);
            resolve(true);
        }
    });
    return await res;
};

const soloLetras = async (e) => {
    let res = new Promise((resolve) => {
        e.classList.remove("is-invalid");
        e.value = e.value.replace(/[0-9]/g, "");
        e.value = e.value.replaceAll(" ", "");
        e.value = e.value.replaceAll("-", "");
        e.value = e.value.replace(/[`~!@#$%^&*()_|+\=?;:'",.<>\{\}\[\]\\\/]/gi, "");
        if (e.value.trim() == "") {
            e.classList.add("is-invalid");
            resolve(false);
        } else {
            resolve(true);
        }
    });
    return await res;
};

const validarClave = async (clave) => {
    let res = new Promise((resolve) => {
        let valido = true;
        clave.classList.remove("is-invalid");
        clave.value = clave.value.replaceAll(" ", "");
        if (clave.value.trim() == "") {
            clave.classList.add("is-invalid");
            valido = false;
        }
        const confir = document.getElementById("clave_confir");
        const clave2 = document.getElementById("clave");
        const confir_msg = confir.parentElement.querySelector(".invalid-feedback");
        confir_msg.innerText = "* Campo requerido";
        if (confir.value != clave2.value) {
            confir_msg.innerText = "* Las claves no coinciden";
            confir.classList.add("is-invalid");
            valido = false;
        }
        if (confir.value == "") {
            confir.classList.add("is-invalid");
            confir_msg.innerText = "* Campo requerido";
            valido = false;
        }
        resolve(valido);
    });
    return await res;
};

const validarTodo = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            const vacios = await todoVacios(form);
            const emails = await todoEmail(form);
            const cedulas = await todoCedulas(form);
            const numeros = await todoNumeros(form);
            const numerosNoCero = await todoNumerosNoCero(form);
            const letras = await todoLetras(form);
            const decimales = await todoDecimales(form);
            const claves = await todoClaves(form);
            const valido = vacios * emails * cedulas * numeros * claves * decimales * letras * numerosNoCero;
            resolve(valido > 0);
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const todoVacios = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                "input:not([data-validate]),select:not([data-validate]),textarea:not([data-validate])"
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await validarVacio(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const todoEmail = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="email"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await validarEmail(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const todoNumeros = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="numeros"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await soloNumeros(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};
const todoNumerosNoCero = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="numeros-no-cero"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await soloNumerosNoCero(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};
const todoLetras = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="letras"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await soloLetras(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const todoDecimales = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="decimal"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await decimal(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const todoCedulas = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="cedula"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await validarCedula(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const disableLogin = (form) => {
    form.querySelectorAll("input,select,button").forEach(item => {
        item.disabled = true;
    });
}

const enableLogin = (form) => {
    form.querySelectorAll("input,select,button").forEach(item => {
        item.disabled = false;
    });
}

const todoClaves = async (form) => {
    let res;
    try {
        res = new Promise(async (resolve) => {
            let valido = true;
            let cont = 0;
            const grupo = form.querySelectorAll(
                `input[data-validate="clave"]`
            );
            if (grupo.length == 0) {
                resolve(true);
                return;
            }
            grupo.forEach(async (item) => {
                if (!(await validarClave(item))) valido = false;
                if (cont == grupo.length - 1) resolve(valido);
                cont++;
            });
        });
    } catch (e) {
        console.log(`${e}`);
    }
    return await res;
};

const parseHora = (date) => {
    let fecha = new Date(Date.parse(date));
    return fecha.toLocaleTimeString();
}

const parseFecha = (date) => {
    let fecha = new Date(Date.parse(date));
    return fecha.toLocaleDateString();
}

const controlarRangoFechas = (desde, hasta) => {
    let hoy = new Date().toISOString().split("T")[0];
    desde?.setAttribute("min", hoy);
    hasta?.setAttribute("min", hoy);
    sessionStorage.removeItem(`fechaDesde`);
    sessionStorage.removeItem(`fechaHasta`);
    desde?.addEventListener("change", (e) => {
        const msg = e.target.parentElement.querySelector(".invalid-feedback");
        if (!!msg) msg.innerText = "* Campo requerido";
        if (e.target.value == "") {
            sessionStorage.removeItem(`fechaDesde`);
            e.target.classList.add("is-invalid");
            return;
        }
        e.target.classList.remove("is-invalid");
        sessionStorage.setItem(`fechaDesde`, Date.parse(e.target.value));
        let fechaDesde = sessionStorage.getItem("fechaDesde");
        let fechaHasta = sessionStorage.getItem("fechaHasta");
        if (!fechaDesde || !fechaHasta) return;
        if (fechaHasta < fechaDesde) {
            if (!!msg) msg.innerText = "* Fecha fuera de rango";
            e.target.classList.add("is-invalid");
        } else {
            desde?.classList.remove("is-invalid");
            hasta?.classList.remove("is-invalid");
        }
    });
    hasta?.addEventListener("change", (e) => {
        const msg = e.target.parentElement.querySelector(".invalid-feedback");
        if (!!msg) msg.innerText = "* Campo requerido";
        if (e.target.value == "") {
            sessionStorage.removeItem(`fechaHasta`);
            e.target.classList.add("is-invalid");
            return;
        }
        e.target.classList.remove("is-invalid");
        sessionStorage.setItem(`fechaHasta`, Date.parse(e.target.value));
        let fechaDesde = sessionStorage.getItem("fechaDesde");
        let fechaHasta = sessionStorage.getItem("fechaHasta");
        if (fechaHasta < fechaDesde) {
            if (!!msg) msg.innerText = "* Fecha fuera de rango";
            e.target.classList.add("is-invalid");
        } else {
            desde?.classList.remove("is-invalid");
            hasta?.classList.remove("is-invalid");
        }
    });
}

const validarRangoFechas = (desde, hasta) => {
    return new Promise(resolve => {
        if (!desde || !hasta) {
            console.log("No se han enviado los dos objetos de fecha requeridos");
            resolve(false);
        } else if (Date.parse(hasta.value) < Date.parse(desde.value)) {
            const msg = desde.parentElement.querySelector(".invalid-feedback");
            if (!!msg) msg.innerText = "* Fechas fuera de rango";
            desde.classList.add("is-invalid");
            hasta.classList.remove("is-invalid");
            resolve(false);
        } else {
            const msg = desde.parentElement.querySelector(".invalid-feedback");
            if (!!msg) msg.innerText = "* Campo requerido";
            desde.classList.remove("is-invalid");
            hasta.classList.remove("is-invalid");
            resolve(true);
        }
    });
}

function getHoraFormat(fecha) {
    if (am_pm == true);
}

function bloquearBotones() {
    document.querySelectorAll("button,a").forEach(item => {
        if (item.dataset.guardar) item.innerHTML = "Espere...";
        item.classList.add("no-event");
        item.disabled = true;
    })
}

function desbloquearBotones() {
    document.querySelectorAll("button,a").forEach(item => {
        if (item.dataset.guardar) item.innerHTML = "Guardar";
        item.classList.remove("no-event");
        item.removeAttribute("disabled");
    })
}

function limpiarSelect2(item) {
    try {
        if (!!item) {
            if ($(item).data('select2')) {
                $(item).val(item.options[0]?.value).trigger("change");
                setTimeout(() => {
                    item.classList.remove("is-invalid");
                    item.parentElement.querySelector("span.is-invalid")?.classList.remove("is-invalid");
                }, 19);
            };
        }
    } catch (e) {
        console.warn(e.message);
    }

}

function addAntiForgeryToken(_formData) {
    try {
        const _tokenInput = document.querySelector("[name='__RequestVerificationToken']");
        if (!_tokenInput) throw new Error(`addAntiForgeryToken: No se ha declarado el @Html.AntiForgeryToken() en el HTML`);
        _formData.append("__RequestVerificationToken", _tokenInput.value);
    } catch (e) {
        console.error(`addAntiForgeryToken: ${e.message}`);
    }
}

function handleError(e) {
    try {
        console.log(e);
        if(e?.response?.status==404){
            toastError(`<b>404:</b> ${e.response.request.responseURL}`);
            return;
        }
        if(!e.response?.data.detail && !!e.message){
            if(!!e.response?.status){
                toastError(`<b>${e.response?.status}:</b> ${e.response.request.response || e.code}`);
            }else{
                toastError(`${e.message}`);
            }
            return;
        }
        const error = `${e.response  ? `<b>${e.response?.status}:</b> ${e.response.data.detail}` : `${e.message}`}`;
        if(!e.message && !e.response.data.detail){
            error="nepe";
        }
        if (!!(typeof toastError)) {
            toastError(error)
        } else {
            alert(error);
        }
    } catch (e) {
        console.log(e);
        console.error("Error");
    }
}

function limpiarValidadores(form){
    if (!!form)
        form.querySelectorAll("input,select,textarea").forEach((item) => {
            if (item.tagName.toUpperCase() == "SELECT") {
                item.classList.remove("is-invalid");
                if ($(item).data('select2')) {
                    item.parentElement.querySelector("span.is-invalid")?.classList.remove("is-invalid");
                    return;
                }
            } else {
                item.classList.remove("is-invalid");
            }
        });
}
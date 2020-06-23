
/** Valida Campos vacíos o formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposCrearCliente() {
    //Establece configuración de los SweetAlert
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var nombre = $("#nombreCliente").val();
    if (nombre.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Nombre es un campo obligatorio.'
        })
        $("#nombreCliente").focus();
        return false;
    }
    var apellido1 = $("#apellido1Cliente").val();
    if (apellido1.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Apellido 1 es un campo obligatorio.'
        })
        $("#apellido1Cliente").focus();
        return false;
    }
    var apellido2 = $("#apellido2Cliente").val();
    if (apellido2.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Apellido 2 es un campo obligatorio.'
        })
        $("#apellido2Cliente").focus();
        return false;
    }
    var dni = $("#dniCliente").val();
    if (dni.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'NIF/NIE es un campo obligatorio.'
        })
        $("#dniCliente").focus();
        return false;
    }
    if (validate(dni.trim()) == false) {
        Toast.fire({
            type: 'error',
            title: 'El NIF/NIE no tiene un formato válido.'
        })
        $("#dniCliente").focus();
        return false;
    }
    var mail = $("#emailCliente").val();
    if (mail.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Email es un campo obligatorio'
        })
        $("#emailCliente").focus();
        return false;
    }
    var correctoMailRegExp = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    if (!mail.match(correctoMailRegExp)) {
        Toast.fire({
            type: 'error',
            title: 'El email no tiene un formato válido : ejemplo@ejemplo.com'
        })
        $("#emailCliente").focus();
        return false;
    }
    var tlf = $("#telefonoCliente").val();
    if (tlf.trim() == "") {
        Toast.fire({
            type: 'error',
            title: '"Teléfono es un campo obligatorio."'
        })
        return false;
    }
    else {

        if (tlf.length < 9 || tlf.length > 9) {
            Toast.fire({
                type: 'error',
                title: 'El teléfono debe tener 9 digitos.'
            })
            return false;
        }
        if (isNaN(tlf.trim())) {
            Toast.fire({
                type: 'error',
                title: 'El teléfono debe ser numérico.'
            })
            return false;
        }
    }
}


/** Valida campos con formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposBuscarClientes() {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var dni = $("#dniCliente").val();
    if (dni.trim() != "") {
        if (validate(dni.trim()) == false) {
            Toast.fire({
                type: 'error',
                title: 'El NIF/NIE no tiene un formato válido.'
            })
            $("#dniCliente").focus();
            return false;
        }
    }
    var mail = $("#emailCliente").val();
    if (mail.trim() != "") {
        var correctoMailRegExp = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
        if (!mail.match(correctoMailRegExp)) {
            Toast.fire({
                type: 'error',
                title: 'El email no tiene un formato válido : ejemplo@ejemplo.com .'
            })
            $("#emailCliente").focus();
            return false;
        }
    }
    var tlf = $("#telefonoCliente").val();
    if (tlf.trim() != "")
    {
        if (tlf.length < 9 || tlf.length > 9) {
            Toast.fire({
                type: 'error',
                title: 'El teléfono debe tener 9 digitos'
            })
            return false;
        }
        if (isNaN(tlf.trim())) {
            Toast.fire({
                type: 'error',
                title: 'El teléfono debe ser numérico'
            })
            return false;
        }
    }   
}

/**Validación de NIE/NIF
*Obtenido de : 
*https://blog.singuerinc.com/javascript/validation/spain/dni/nie/nif/regex/2014/08/06/code-day-006-spain-dni-validation/ */
function validate(value) {
    // Expresiones regulares.
    var validChars = 'TRWAGMYFPDXBNJZSQVHLCKET';
    var nifRexp = /^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKET]{1}$/i;
    var nieRexp = /^[XYZ]{1}[0-9]{7}[TRWAGMYFPDXBNJZSQVHLCKET]{1}$/i;
    var str = value.toString().toUpperCase();

    if (!nifRexp.test(str) && !nieRexp.test(str)) return false;

    var nie = str
        .replace(/^[X]/, '0')
        .replace(/^[Y]/, '1')
        .replace(/^[Z]/, '2');

    var letter = str.substr(-1);
    var charIndex = parseInt(nie.substr(0, 8)) % 23;

    if (validChars.charAt(charIndex) === letter) return true;

    return false;
}

/** Validación mediante Ajax, verifica si el NIF/NIE introducido
* ya está registrado en la bbdd*/
function ValidarDniDuplicado(dni) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var dni = dni.toUpperCase().trim();

    var url = "/Clientes/VerificarDniDuplicado?dni=" + dni;
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (data == 1) {
                Toast.fire({
                    type: 'error',
                    title: 'El Nif/Nie introducido ya existe en la base de datos. No se pueden duplicar'
                })
                $("#dniCliente").focus();
            }
            else if (data == 0) {
                return true;
            }
        },
        error: function (xhr) {
            Toast.fire({
                type: 'error',
                title: 'Hay un error en la comprobación de dni/nif. '
            })
        }
    });
};

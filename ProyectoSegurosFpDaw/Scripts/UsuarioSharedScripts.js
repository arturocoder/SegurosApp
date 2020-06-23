
/** Valida Campos vacíos o formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposCrearUsuario() {
    // Establece configuración de los SweetAlert.
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    }); 
    var nombre = $("#nombreUsuario").val();
    if (nombre.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Nombre es un campo obligatorio.'
        })
        $("#nombreUsuario").focus();
        return false;
    }
    var apellido1 = $("#apellido1Usuario").val();
    if (apellido1.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Apellido 1 es un campo obligatorio.'
        })
        $("#apellido1Usuario").focus();
        return false;
    }
    var apellido2 = $("#apellido2Usuario").val();
    if (apellido2.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Apellido 2 es un campo obligatorio.'
        })
        $("#apellido2Usuario").focus();
        return false;
    }
    var dni = $("#dniUsuario").val();
    if (dni.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'NIF/NIE es un campo obligatorio.'
        })
        $("#dniUsuario").focus();
        return false;
    }    
    if (validate(dni.trim()) == false) {
        Toast.fire({
            type: 'error',
            title: 'El NIF/NIE no tiene un formato válido.'
        })
        $("#dniUsuario").focus();
        return false;
    }   
    var mail = $("#emailUsuario").val();
    if (mail.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Email es un campo obligatorio.'
        })
        $("#emailUsuario").focus();
        return false;
    }

    var correctoMailRegExp = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
    if (!mail.match(correctoMailRegExp)) {
        Toast.fire({
            type: 'error',
            title: 'El email no tiene un formato válido : ejemplo@ejemplo.com'
        })
        $("#emailUsuario").focus();
        return false;
    }
    var psw = $("#password").val();
    if (psw.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Password es un campo obligatorio.'
        })
        $("#password").focus();
        return false;
    }
    var correctoPswRegExp = /^(?=\w*\d)(?=\w*[A-Z])(?=\w*[a-z])\S{4,8}$/;
    if (!psw.match(correctoPswRegExp)) {
        Toast.fire({
            type: 'error',
            title: 'Password debe tener entre 4 y 8 caracteres, al menos un dígito, al menos una minúscula y al menos una mayúscula. '
        })
        $("#password").focus();
        return false;
    }
}

/** Valida Campos con formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposBuscarUsuarios() {
    
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });  
   var dni = $("#dniUsuario").val();
    if (dni.trim() != "") {
        if (validate(dni.trim()) == false) {
            Toast.fire({
                type: 'error',
                title: 'El NIF/NIE no tiene un formato válido.'
            })
            $("#dniUsuario").focus();
            return false;
        }
    }   
    var mail = $("#emailUsuario").val();
    if (mail.trim() != "") {
        var correctoMailRegExp = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
        if (!mail.match(correctoMailRegExp)) {
            Toast.fire({
                type: 'error',
                title: 'El email no tiene un formato válido : ejemplo@ejemplo.com'
            })
            $("#emailUsuario").focus();
            return false;
        }
    }  
}

/**Validación de NIE/NIF
*Obtenido de :
*https://blog.singuerinc.com/javascript/validation/spain/dni/nie/nif/regex/2014/08/06/code-day-006-spain-dni-validation/ */
function validate(value) {
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



/** Validación mediante Ajax, verifica si el email introducido
* ya está registrado en la bbdd*/
function ValidarEmailDuplicado(email) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var email = email.toUpperCase().trim();
    var url = "/Usuarios/VerificarEmailDuplicado?email=" + email;
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
                    title: 'El email introducido ya existe en la base de datos. No se pueden duplicar.'
                })
                $("#emailUsuario").focus();
            }
            else if (data == 0) {
                return true;
            }
        },
        error: function (xhr) {
            Toast.fire({
                type: 'error',
                title: 'Hay un error en la comprobación de email. '
            })
        }
    });
};


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

    var url = "/Usuarios/VerificarDniDuplicado?dni=" + dni;
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
                    title: 'El Nif/Nie introducido ya existe en la base de datos. No se pueden duplicar.'
                })
                $("#dniUsuario").focus();
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

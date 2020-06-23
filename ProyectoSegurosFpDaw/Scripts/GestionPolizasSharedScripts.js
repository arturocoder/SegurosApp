

/** Valida Campos vacíos o formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposCrearPolizas() {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var dni2 = $("#dniCliente").val();
    if (dni2.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir un NIF/NIE para crear una póliza.'
        })
        $("#dniCliente").focus();
        return false;
    }
    if (validate(dni2.trim()) == false) {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir un  NIF/NIE con un formato válido para crear una póliza.'
        })
        $("#dniCliente").focus();
        return false;
    }
}

/** Valida Campos con formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposBuscarPolizas() {

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
    var tlf = $("#telefonoCliente").val();
    if (tlf.trim() != "") {
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
    var matricula = $("#matricula").val();
    if (matricula.trim() != "") {
        if (matricula.length > 10) {
            Toast.fire({
                type: 'error',
                title: 'La matrícula del vehículo  no puede superar los 10 caracteres.'
            })
            $("#matricula").focus();
            return false;
        }
        //Expresión regular para formato matrículas
        //Obtenido de : 
        //https://www.laps4.com/comunidad/threads/necesito-funcion-javascript-para-validar-matriculas.186497/
        var correctoMatriculaRegExp = /(\d{4}-[\D\w]{3}|[\D\w]{1,2}-\d{4}-[\D\\w]{2})/;
        if (!matricula.match(correctoMatriculaRegExp)) {
            Toast.fire({
                type: 'error',
                title: 'La matrícula del vehículo debe tener un formato válido  : \n  Matrícula nueva: 0123-ABC  // Matrícula antigua: AB-0123-CS'
            })
            $("#matricula").focus();
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

/** Valida Campos vacíos o con formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposAltaPoliza() {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var matricula = $("#matricula").val();
    if (matricula.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir una matrícula para crear la póliza.'
        })
        $("#matricula").focus();
        return false;
    }
    if (matricula.length > 10) {
        Toast.fire({
            type: 'error',
            title: 'La matrícula del vehículo  no puede superar los 10 caracteres.'
        })
        $("#matricula").focus();
        return false;
    }
    //Expresión regular para formato matrículas
    //Obtenido de : 
    //https://www.laps4.com/comunidad/threads/necesito-funcion-javascript-para-validar-matriculas.186497/
    var correctoMatriculaRegExp = /(\d{4}-[\D\w]{3}|[\D\w]{1,2}-\d{4}-[\D\\w]{2})/;
    if (!matricula.match(correctoMatriculaRegExp)) {
        Toast.fire({
            type: 'error',
            title: 'La matrícula del vehículo debe tener un formato válido  : \n  Matrícula nueva: 0123-ABC  // Matrícula antigua: AB-0123-CS'
        })
        $("#matricula").focus();
        return false;
    }
    var marca = $("#marcaVehiculo").val();
    if (marca.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir la marca del vehículo para crear la póliza.'
        })
        $("#marcaVehiculo").focus();
        return false;
    }
    if (marca.length > 20) {
        Toast.fire({
            type: 'error',
            title: 'La marca del vehículo  no puede superar los 20 caracteres.'
        })
        $("#marca").focus();
        return false;
    }
    var modelo = $("#modeloVehiculo").val();
    if (modelo.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir el modelo del vehículo para crear la póliza.'
        })
        $("#modeloVehiculo").focus();
        return false;
    }
    if (modelo.length > 20) {
        Toast.fire({
            type: 'error',
            title: 'El modelo del vehículo  no puede superar los 20 caracteres.'
        })
        $("#modelo").focus();
        return false;
    }
    var fechaApertura = $("#fechaApertura").val();
    if (fechaApertura.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir fecha de inicio  y fin  para crear la póliza.'
        })
        $("#fechaApertura").focus();
        return false;
    }
    var precio = $("#precio").val();
    if (precio.trim() == "") {
        Toast.fire({
            type: 'error',
            title: "Debe introducir el precio para crear la póliza."
        })
        $("#precio").focus();
        return false;
    }
    else {

        if (isNaN(precio.trim())) {
            Toast.fire({
                type: 'error',
                title: 'El precio debe ser numérico.'
            })
            $("#precio").focus();
            return false;
        }
    }
    var observaciones = $("#observaciones").val();
    if (observaciones.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir observaciones para crear la póliza.'
        })
        $("#observaciones").focus();
        return false;
    }
    if (observaciones.length > 500) {
        Toast.fire({
            type: 'error',
            title: 'Las observaciones no pueden superar los 500 caracteres.'
        })
        $("#observaciones").focus();
        return false;
    }
}

/** Valida campos con formato incorrecto.
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposModificarPoliza() {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });
    var matricula = $("#matricula").val();
    if (matricula.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir una matrícula para crear la póliza.'
        })
        $("#matricula").focus();
        return false;
    }
    if (matricula.length > 10) {
        Toast.fire({
            type: 'error',
            title: 'La matrícula del vehículo  no puede superar los 10 caracteres.'
        })
        $("#matricula").focus();
        return false;
    }
    //Expresión regular para formato matrículas
    //Obtenido de : 
    //https://www.laps4.com/comunidad/threads/necesito-funcion-javascript-para-validar-matriculas.186497/
    var correctoMatriculaRegExp = /(\d{4}-[\D\w]{3}|[\D\w]{1,2}-\d{4}-[\D\\w]{2})/;
    if (!matricula.match(correctoMatriculaRegExp)) {
        Toast.fire({
            type: 'error',
            title: 'La matrícula del vehículo debe tener un formato válido  : \n  Matrícula nueva: 0123-ABC  // Matrícula antigua: AB-0123-CS'
        })
        $("#matricula").focus();
        return false;
    }
    var marca = $("#marcaVehiculo").val();
    if (marca.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir la marca del vehículo para crear la póliza.'
        })
        $("#marcaVehiculo").focus();
        return false;
    }
    if (marca.length > 20) {
        Toast.fire({
            type: 'error',
            title: 'La marca del vehículo  no puede superar los 20 caracteres.'
        })
        $("#marca").focus();
        return false;
    }
    var modelo = $("#modeloVehiculo").val();
    if (modelo.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir el modelo del vehículo para crear la póliza.'
        })
        $("#modeloVehiculo").focus();
        return false;
    }
    if (modelo.length > 20) {
        Toast.fire({
            type: 'error',
            title: 'El modelo del vehículo  no puede superar los 20 caracteres.'
        })
        $("#modelo").focus();
        return false;
    }
    var precio = $("#precio").val();
    if (precio.trim() == "") {
        Toast.fire({
            type: 'error',
            title: "Debe introducir el precio para crear la póliza."
        })
        $("#precio").focus();
        return false;
    }
    else {

        if (isNaN(precio.trim())) {
            Toast.fire({
                type: 'error',
                title: 'El precio debe ser numérico.'
            })
            $("#precio").focus();
            return false;
        }
    }
    var observaciones = $("#observaciones").val();
    if (observaciones.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Debe introducir observaciones para crear la póliza.'
        })
        $("#observaciones").focus();
        return false;
    }
    if (observaciones.length > 500) {
        Toast.fire({
            type: 'error',
            title: 'Las observaciones no pueden superar los 500 caracteres.'
        })
        $("#observaciones").focus();
        return false;
    }
    return true;
}





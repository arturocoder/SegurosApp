
/** Valida Campos vacíos o formato incorrecto
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposCrearCondicionado() {
    // Establece configuración de los SweetAlert.
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });  
    
    var nombre = $("#tipoCondicionado").val();
    if (nombre.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Nombre del Condicionado es un campo obligatorio.'
        })
        $("#tipoCondicionado").focus();
        return false;
    }
    if (nombre.length > 50) {
        Toast.fire({
            type: 'error',
            title: 'Nombre del Condicionado no puede superar los 50 caracteres.'
        })
        $("#tipoCondicionado").focus();
        return false;
    }
    var garantias = $("#garantias").val();
    if (garantias.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Garantías es un campo obligatorio.'
        })
        $("#garantias").focus();
        return false;
    }
    if (garantias.length > 500) {
        Toast.fire({
            type: 'error',
            title: 'El campo Garantías no puede superar los 500 caracteres.'
        })
        $("#garantias").focus();
        return false;
    }   
}

/** Valida Campos vacíos o formato incorrecto
* Return false , muestra Sweet Alert con error y focus sobre el campo. */
function ValidarCamposEditarCondicionado(nombre,garantias) {
    //Establece configuración de los SweetAlert
    const Toast = Swal.mixin({
        toast: true,
        position: 'top',
        showConfirmButton: false,
        timer: 3000
    });       
    if (nombre.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Nombre del Condicionado es un campo obligatorio.'
        })        
        return false;
    }
    if (nombre.length > 50) {
        Toast.fire({
            type: 'error',
            title: 'Nombre del Condicionado no puede superar los 50 caracteres.'
        })       
        return false;
    }    
    if (garantias.trim() == "") {
        Toast.fire({
            type: 'error',
            title: 'Garantías es un campo obligatorio'
        })        
        return false;
    }
    if (garantias.length > 500) {
        Toast.fire({
            type: 'error',
            title: 'El campo Garantías no puede superar los 500 caracteres'
        })       
        return false;
    }
}










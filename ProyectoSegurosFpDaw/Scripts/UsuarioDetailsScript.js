$(document).ready(function () {
    //Comprueba y muestra errores enviados desde el controller
    ComprobarMensajes();

    // Form Eliminar Usuario.
    $("#btnDelete").click(function () {
        Swal.fire({
            title: 'El usuario no se podrá visualizar. ¿Está seguro que desea eliminar el usuario del sistema? ',
            text: "Si solamente desea que el usuario no disponga de acceso al sistema, edite el usuario y modifique su rol a 'NO OPERATIVO' ",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#ba1717',
            cancelButtonColor: '#17a2b8',
            confirmButtonText: 'Eliminar Usuario',
            cancelButtonText: 'Volver',
        }).then((result) => {
            if (result.value) {
                $("form").submit();
            }
        })
        return false;
    });
});
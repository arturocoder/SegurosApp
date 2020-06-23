$(document).ready(function () {
   
    ComprobarMensajes();

    // Form Eliminar Cliente.
    $("#btnDelete").click(function () {
        Swal.fire({
            title: 'El cliente no se podrá visualizar. ¿Está seguro que desea eliminar el cliente del sistema? ',
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#ba1717',
            cancelButtonColor: '#17a2b8',
            confirmButtonText: 'Eliminar Cliente',
            cancelButtonText: 'Volver',
        }).then((result) => {
            if (result.value) {
                $("form").submit();
            }
        })
        return false;
    });
});
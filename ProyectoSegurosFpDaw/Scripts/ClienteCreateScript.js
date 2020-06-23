$(document).ready(function () {
    ComprobarMensajes();
    
    $("#dniCliente").focusout(function () {
        var dni = $("#dniCliente").val().trim();
        if (dni.length > 0 || dni != "") {
            ValidarDniDuplicado(dni);
        }
    });

    // Form Crear Cliente
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposCrearCliente() == false) {
                return false;
            }
            return true;
        });
    });
})
$(document).ready(function () {

    ComprobarMensajes();
    
    $("#dniUsuario").focusout(function () {
        var dni = $("#dniUsuario").val().trim();
        if (dni.length > 0 || dni != "") {
            ValidarDniDuplicado(dni);
        }
    });

    $("#emailUsuario").focusout(function () {
        var email = $("#emailUsuario").val().trim();
        if (email.length > 0 || email != "") {
            ValidarEmailDuplicado(email);
        }
    });

    // Form Crear Usuario.
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposCrearUsuario() == false) {
                return false;
            }
            return true;
        });
    });
})
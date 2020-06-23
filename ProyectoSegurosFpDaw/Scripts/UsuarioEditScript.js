$(document).ready(function () {
    ComprobarMensajes();
        
    $("#emailUsuario").change(function () {
        var email = $("#emailUsuario").val().trim();
        if (email.length > 0 || email != "") {
            ValidarEmailDuplicado(email);
        }
    });

    // Formulario Editar Usuario
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposCrearUsuario() == false) {
                return false;
            }
            return true;
        });
    });
})
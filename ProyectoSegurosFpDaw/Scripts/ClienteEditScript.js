$(document).ready(function () {
    ComprobarMensajes();

    // Formulario Editar Cliente.
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposCrearCliente() == false) {
                return false;
            }
            return true;
        });
    });
})
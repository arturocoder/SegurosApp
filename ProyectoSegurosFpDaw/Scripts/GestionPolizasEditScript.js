$(document).ready(function () {

    ComprobarMensajes();

    //Form Edit
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposModificarPoliza() == false) {
                return false;
            }
            return true;
        });
    });
});

$(document).ready(function () {
    
    ComprobarMensajes();
    
    $(function () {
        // Form Editar.
        $(".btnEditar").click(function () {

            // Botón editar =>
            // 1º click , cambia icono de editar a guardar y desactiva los readonlys.
            // 2º click , guarda valores nuevos  en los input hidden y envía el formulario.

            // Guarda el tipo de icono que tiene (pencil => editar)
            var esEstadoEditar = $(this).children("i").hasClass("fa-pencil-alt");

            // Comprueba que es el primer click en editar
            if (esEstadoEditar) {

                // Guarda el valor del input hermano del botón clickado.
                var ID = $(this).siblings("input").val();
                var idTCond = "#tipoCondicionado" + ID;
                var idGara = "#garantias" + ID;

                // Desactiva readonlys.
                $(idTCond).attr("readonly", false);
                $(idGara).attr("readonly", false);

                //  Modifica iconos.
                $(this).children("i").removeClass("fa-pencil-alt");
                $(this).children("i").addClass("fa-save");
            } else { 
                
                var ID = $(this).siblings("input").val();
                var idTCond = "#tipoCondicionado" + ID;
                var idGara = "#garantias" + ID;                
                var valueCond = $(idTCond).val();
                var valueGaran = $(idGara).val();               
                if (ValidarCamposEditarCondicionado(valueCond, valueGaran) == false) {
                    return false;
                }

                // Asigna los valores a los input hidden hermanos.
                $(idTCond).siblings("input").val(valueCond);                
                $(idGara).siblings("input").val(valueGaran);
                // Envía el formulario
                $("#" + ID).submit();
            }
            return true;
        });

        // Form Eliminar
        $(".btnEliminar").click(function () {
            $(this).submit();
        });

        // Form Crear
        $("#btnCrear").click(function () {
            if (ValidarCamposCrearCondicionado() == false) {
                return false;
            }
            $("#formCreate").submit();
            return true;
        });
    });
})
$(document).ready(function () {
    ComprobarMensajes();

    // Configuración Date range picker
    // rango máximo entre Inicio y Fin (1 año).
    // minima fecha Inicio (hoy ).
    // máxima fecha ( 1 año y medio desde hoy) .          
    $('#fechaApertura').daterangepicker({
        locale: {
            format: "DD/MM/YYYY"
        },
        "maxSpan": {
            "year": 1
        },
        "minDate": moment(),
        "maxDate": moment().add(1.5, "year"),

    })

    //Form Crear
    $(function () {
        $("form").submit(function () {
            if (ValidarCamposAltaPoliza() == false) {
                return false;
            }
            // Recoge las valores de fecha In/Out del daterangePicker ,les da formato
            // y los envía mediante input hidden
            var fechaAperturaFormat = $('#fechaApertura').data('daterangepicker');
            var fechaInicio = fechaAperturaFormat.startDate.format("DD/MM/YYYY");
            var fechaFin = fechaAperturaFormat.endDate.format("DD/MM/YYYY");
            $("#fechaInicio").val(fechaInicio);
            $("#fechaFin").val(fechaFin);
            return true;
        });
    });
})
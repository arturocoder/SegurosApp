$(document).ready(function () {
    
    ComprobarMensajes();

    // Configuración de Datatable.
    $('#tableResultados').DataTable({
        "scrollY": "50vh",
        "scrollX": true,
        "scrollCollapse": true,
        "searching": false,
        "paging": false,
        "info": false,
        // Columna 8 no visible(url para direccionar  a Details/usuarioId).
        "columnDefs": [
            { "orderable": false, "targets": 7 },
            { "visible": false, "targets": 7 }
        ],
    });

    // Configuración Date range picker.
    $('#fechaApertura').daterangepicker({
        locale: {
            format: "DD/MM/YYYY"
        },
        "startDate": "01/01/2000",
        "endDate": "01/01/3000",
        ranges: {
            'Hoy': [moment(), moment()],
            'Ayer': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Últimos 7 Días': [moment().subtract(6, 'days'), moment()],
            'Ultimos 30 Días': [moment().subtract(29, 'days'), moment()],
            'Todos': [moment("2000-01-01"), moment("3000-01-01")],
        },
    })

    // Cuando click en un registro (fila),direcciona a Details de ese registro
    var tabla = $("#tableResultados").DataTable();
    $("#tableResultados tbody").on("click", 'tr', function () {
        var data = tabla.row(this).data();
        var url = data[7];
        window.location.assign(url);
    });

    // Colapsa / muestra sección Resultados
    // dependiendo del estadoSession.    
    // 2 => muestra Resultados
    $(function () {
        var estadoSession = $("#estadoSession").val();
        if (estadoSession == "2") {
            $("#btnColl").trigger("click");

        }
    });

    // Campos Form Buscar    
    // Si selecciona  búsqueda por cualquier parámetro, no deja seleccionar el resto
    // Excepto fecha y estado
    $(".campos").focus(function () {
        $(this).attr("readonly", false);
        $(".campos").not(this).attr("readonly", true).val("");

    });

    // Form Crear
    $("#btnCrearP").click(function () {
        if (ValidarCamposCrearPolizas() == false) {
            return false;
        } else {
            // Si el dni es correcto, redirige a gestionPolizas/Create.
            var dniCliente = $("#dniCliente").val().trim();
            var url = $("#urlCrear").val();
            url = url.replace("dniClien", encodeURIComponent(dniCliente));
            window.location.assign(url);
        }
    })
});
// Form Buscar Póliza
$(function () {
    $("form").submit(function () {
        //Validaciones antes de enviar el formulario
        if (ValidarCamposBuscarPolizas() == false) {
            return false;
        }
        // Recoge las valores de fecha In/Out del daterangePicker ,les da formato
        // y los envía  mediante input hidden.
        var fechaAperturaFormat = $('#fechaApertura').data('daterangepicker');
        var fechaInicio = fechaAperturaFormat.startDate.format("DD/MM/YYYY");
        var fechaFin = fechaAperturaFormat.endDate.format("DD/MM/YYYY");
        $("#fechaInicio").val(fechaInicio);
        $("#fechaFinal").val(fechaFin);
        return true;
    });
});
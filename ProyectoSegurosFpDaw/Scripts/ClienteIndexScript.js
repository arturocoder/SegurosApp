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
        // Columna 5 no visible(url para direccionar  a Details/clienteId)
        "columnDefs": [
            { "orderable": false, "targets": 4 },
            { "visible": false, "targets": 4 }
        ],
    });

    // Cuando click en un registro (fila) ,direcciona a Details de ese registro.
    var tabla = $("#tableResultados").DataTable();
    $("#tableResultados tbody").on("click", 'tr', function () {
        var data = tabla.row(this).data();        
        var url = data[4];
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

    // Form Buscar
    // Solo deja seleccionar búsqueda por 1 campo
    $(".campos").focus(function () {
        $(this).attr("readonly", false);
        $(".campos").not(this).attr("readonly", true).val("");
    });
});

$(function () {
    $("form").submit(function () {        
        if (ValidarCamposBuscarClientes() == false) {
            return false;
        }
        return true;
    });
});
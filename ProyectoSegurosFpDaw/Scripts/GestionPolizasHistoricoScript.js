$(document).ready(function () {

    // Configuración de Datatable
    $('#tableResultados').DataTable({
        "scrollY": "50vh",
        "scrollX": true,
        "scrollCollapse": true,
        "searching": false,
        "paging": false,
        "info": false,
        "columnDefs": [
            { "orderable": false, "targets": [0,1,2,3,4,5,6,7,8,9] },
            { "visible": false, "targets": [0,9] }
        ],
    });

    // Cuando click en un registro (fila) ,muestra las observaciones de ese registro
    var tabla = $("#tableResultados").DataTable();
    $("#tableResultados tbody").on("click", 'tr', function () {
        var data = tabla.row(this).data();
        var observaciones = data[9];
        Swal.fire({
            title: 'Observaciones ',
            text: observaciones,
            type: 'info',
            confirmButtonColor: '#17a2b8',
            confirmButtonText: 'Ok'
        })
    });
});       
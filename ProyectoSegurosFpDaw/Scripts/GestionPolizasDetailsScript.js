$(document).ready(function () {

    ComprobarMensajes();

    // Form Cancelar
    $("#btnDelete").click(function () {
        cancelarPoliza();

        //Función asíncrona, para introducir motivo cancelación
        //y confirmar la cancelación ,mediante un Sweet Alert2.
        async function cancelarPoliza() {
            const { value: textClx } = await Swal.fire({
                title: '¿Está seguro que desea cancelar la póliza? ',
                type: 'warning',
                input: 'textarea',
                inputPlaceholder: 'Motivo de la cancelación',
                inputAttributes: {
                    'aria-label': 'Motivo de la cancelación'                   
                },
                showCancelButton: true,
                confirmButtonColor: '#ba1717',
                cancelButtonColor: '#17a2b8',
                confirmButtonText: 'Cancelar Póliza',
                cancelButtonText: 'Volver',
                inputValidator: (result) => {
                    return !result && 'Motivo de cancelación es obligatorio'
                }
            })
            if (textClx) {
                $("#motivoClx").val(textClx);
                $("form").submit();
            }
        }
    });
});





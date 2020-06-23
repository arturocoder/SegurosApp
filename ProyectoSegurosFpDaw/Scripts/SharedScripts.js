
/**Comprueba  si ha habido algún msj enviado desde
*el controlador a través de  ViewBag / Tempdata (mensaje)
 * y lo muestra con Sweet Alert.*/
function ComprobarMensajes() {   
    if ($("#mensaje").length > 0) {
        var mensaje = $("#mensaje").val();        
        // tipo (icono) =>
        // success , error , info , warning , question.
        var tipo = $("#tipo").val();    
                
        const Toast = Swal.mixin({
            toast: true,
            position: 'top',
            showConfirmButton: false,
            timer: 6000
        });
        Toast.fire({
            type: tipo,
            title: mensaje
        })
    }
}
# SegurosApp
## Aplicación de gestión de seguros.
### Proyecto final del CFGS Desarrollo de aplicaciones web.
Esta aplicación fue entregada como proyecto de fin de ciclo DAW en Mayo del 2020.
#### [Memoria del proyecto](https://drive.google.com/file/d/1_mEQaHkbmbSmGsHJDzOBCI4Yj162a0uV/view?usp=sharing)
#### [Demo visual del proyecto](https://drive.google.com/file/d/1rc4POrd8vz5r5pg_6xSaaKIkXrI846v6/view?usp=sharing)
#### Objetivos propuestos
La aplicación web tiene como propósito general ser una herramienta de gestión de seguros de vehículos a motor, en concreto matriculados en España. Está orientada a compañías de seguros que deseen optimizar la gestión del área administrativa y comercial de la empresa. Del mismo modo, también es posible su utilización por empresas externalizadas del sector, como centros de atención al cliente o call centers orientados a la comercialización de seguros. 

El aplicativo se puede enmarcar dentro de la tipología CRM (Customer Relationship Management) o gestión de relaciones con clientes. Los aplicativos CRM son usados para centralizar a través de uno o varios módulos cualquier información relacionada con la interacción con los clientes. Ejemplos de estos módulos son los orientados a reportes de ventas, campañas de marketing, gestión y análisis de ventas. En concreto la aplicación de gestión de seguros implementa los módulos de gestión de ventas,  soporte y gestión de datos del cliente. 

Los objetivos específicos se enmarcan dentro de cuatro áreas: 
- Gestión de usuarios y roles: el usuario es el empleado de la empresa que utiliza el aplicativo. El aplicativo permitirá la creación, búsqueda, visualización, modificación y desactivación de usuarios, así como la visualización de los diferentes roles y permisos. A cada usuario se le debe asignar un rol dentro de cuatro tipos predefinidos. Cada rol se diferencia por los permisos que disponga para la visualización, creación, modificación y eliminación en cada área del aplicativo. Los tipos de roles son:
  - Administrador: es el administrador general de la aplicación, dispone de todos los permisos. 
  - Manager: orientado a jefes o supervisores. Dispone de todos los permisos excepto la creación, modificación y eliminación de usuarios.
  - Agente: orientado a perfiles de gestores comerciales, gestores de atención al cliente, entre otros. Tiene acceso parcial a la gestión de pólizas, usuarios y clientes.
  - No operativo: orientado a empleados que por cualquier motivo no van a usar el aplicativo de forma temporal. Restricción total de acceso, excepto para visualizar datos personales de usuario.
  
- Gestión de autenticación: debe permitir o denegar el acceso a la aplicación a través de un sistema de login con usuario y contraseña.

- Gestión de clientes: el aplicativo permitirá la creación, búsqueda, visualización, modificación y desactivación de clientes.

- Gestión de pólizas: el aplicativo permitirá la creación, búsqueda, visualización, modificación y cancelación de pólizas, así como la visualización del histórico de modificaciones o gestiones realizadas sobre la misma. También permitirá la visualización, creación, modificación, activación y desactivación de condicionados de póliza.


#### Tecnologías y herramientas utilizadas en el proyecto:
- ASP.NET Framework 
- C#
- MVC 5
- Microsoft Sql Server
- Entity Framework EF6 (Database First)
- LINQ
- HTML5 / CSS3 / Javascript
- JQuery
- Bootstrap
- AdminLTE
- Razor

#### Instalación en local (la aplicación incluye por defecto datos creados y ficticios de clientes, pólizas...)
- Clonar repositorio en carpeta local.
- Descargar [archivos necesarios](https://drive.google.com/file/d/1t7EIjG3ugKPfNrSu_T8jkx-O83QKwl9K/view?usp=sharing) para la instalación (Sql Scripts/Password para login)
- Crear una nueva base de datos en SqlServer con el nombre “ProyectoSegurosBBDD”.
- Ejecutar el archivo de script Sql de creación de tablas y campos Uno_crearTablas.sql
- Ejecutar el archivo de script Sql de creación de registros: Dos_InsertarDatos.sql
- Ejecutar archivo Sql de creación de un trigger para la tabla cliente: Tres_crearTriggerCliente.sql
- Ejecutar archivo Sql de creación de un procedure para reiniciar BBDD: Cuatro_ProcedureReiniciarbbdd




#### Version 1.0 Branch 
Versión inicial que fue entregada como proyecto.
#### Master Branch 
Actualmente en estado de refactorización, aplicando repository pattern / unit of work, y creando una capa BLL.



Autor del proyecto: Arturo Marín.



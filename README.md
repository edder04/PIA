# Sistema escolar de venta de boletos

Aplicación WinForms en C# y SQL Server para administrar y vender boletos. La aplicación usa la base `PIA_V3` y sus registros existentes. No se conecta con bancos, procesadores de pago ni servicios fiscales; los PDF de factura son demostrativos y no tienen validez fiscal.

## Requisitos

- Windows y Visual Studio con .NET 10 SDK y carga de trabajo de escritorio .NET.
- SQL Server (Express, LocalDB o instancia local equivalente).
- Abrir `PIA.slnx` y restaurar los paquetes NuGet.

## Conectar la base existente

La base `PIA_V3` ya contiene las tablas, vistas, funciones, triggers y registros del proyecto. No vuelvas a ejecutar los scripts de creación ni el seed demo sobre esa base. Se instaló `Database/03_ProcedimientosAlmacenados.sql`, que agrega los procedimientos que necesita la versión actual de la aplicación, sin recrear tablas ni reemplazar los registros existentes.

La conexión se configura en `App.config`; ajusta `Server` si cambia el nombre de la instancia. También se puede definir la variable de entorno `PIA_SQL_CONNECTION`, que tiene prioridad sobre el archivo.

El archivo original `SQLQuery3.sql` contiene la cuenta administrativa `admin@ejemplo.com` con contraseña inicial `Admin123!`. El inicio de sesión usa el correo y actualiza la contraseña a hash de ASP.NET Identity después de un acceso válido.

Para crear una base desde cero, ejecuta los scripts originales `CreacionDeTablas.sql` y `VistasFuncionesTriggers.sql`, y después `Database/03_ProcedimientosAlmacenados.sql`. `Database/04_DatosDemo.sql` es opcional para una instalación vacía; no lo ejecutes si quieres conservar solo tus datos existentes.

## Flujo principal

- Administrador: gestiona usuarios, clientes, estadios, zonas, partidos, consulta reportes y cancela ventas indicando el motivo.
- Operativo: registra ventas y consulta clientes e historial. No puede cancelar ventas.
- Una venta admite de uno a cuatro boletos, verifica disponibilidad, asigna asientos, calcula IVA y emite una factura local en PDF con marca DEMO.
- El historial de cliente reúne sus compras y boletos; los reportes de venta y ocupación permiten filtrar y exportar resultados.
- Los pagos con tarjeta solo se simulan en pantalla. No se guardan PAN, CVV ni vencimiento. No introducir datos de tarjetas reales.

## Datos y mantenimiento

Las lecturas y escrituras de la aplicación se envían a procedimientos almacenados. Las vistas se consultan mediante procedimientos. La solución no usa `SELECT *`.

Conserva la base y sus datos. Antes de entregar, genera un respaldo `.bak` desde SSMS (`Tasks > Back Up`) o con `BACKUP DATABASE` en la instancia local.

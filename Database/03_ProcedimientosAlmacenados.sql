USE PIA_V3;
GO

/* Todas las lecturas expuestas a la aplicación se realizan por procedimientos.
   Se enumeran las columnas para mantener explícito el contrato de datos. */
--===========================================================================
-- Consulta un usuario mediante su correo electrónico para validar el acceso.
--===========================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ConsultarCorreo @Correo NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, [Nombre(s)], ApPaterno, ApMaterno, FechaNacimiento, NumeroNomina,
           CorreoElectronico, Rol, NombreUsuario, Contraseña, Activo, CreadoPor, FechaRegistro
    FROM dbo.Usuarios WHERE CorreoElectronico = @Correo AND Activo = 1;
END;
GO
--=====================================================================
-- Devuelve los usuarios registrados para mostrarlos en administración.
--=====================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuarios_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Nombres, ApPaterno, ApMaterno, FechaNacimiento, Edad, NumeroNomina,
           CorreoElectronico, Rol, NombreUsuario, Activo, CreadoPor, FechaRegistro
    FROM dbo.vw_Usuarios;
END;
GO
--==================================================================
-- Devuelve los clientes registrados para búsqueda y administración.
--==================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Clientes_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Nombres, ApPaterno, ApMaterno, FechaNacimiento, Edad, Domicilio, RFC,
           CorreoElectronico, TelefonoCasa, TelefonoCelular, Activo, FechaHoraRegistro,
           CreadoPorNombre, CreadoPorRol
    FROM dbo.vw_Clientes;
END;
GO
--===================================
-- Devuelve los estadios registrados.
--===================================
CREATE OR ALTER PROCEDURE dbo.sp_Estadios_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdEstadio, Nombre, Ciudad, CapacidadTotal, MapaEstadio, TotalZonas, CupoAsignado,
           CapacidadDisponible, FechaHoraRegistro
    FROM dbo.vw_Estadios;
END;
GO
--=================================================================
-- Devuelve los estadios disponibles en los formularios de captura.
--=================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Estadios_ListadoCaptura
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdEstadio, Nombre, Ciudad, CapacidadTotal, MapaEstadio, CreadoPor, FechaHoraRegistro
    FROM dbo.Estadios ORDER BY Nombre;
END;
GO
--============================================================================
-- Devuelve los partidos registrados junto con sus datos de estadio y equipos.
--============================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Partidos_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPartido, FechaHora, EquipoLocal, EquipoVisitante, Estado, IdEstadio, Estadio,
           Ciudad, CapacidadTotal, BoletosVendidos, BoletosDisponibles, FechaHoraRegistro, CreadoPor
    FROM dbo.vw_Partidos;
END;
GO
--====================================================================
-- Devuelve el cupo y la disponibilidad de boletos por zona y partido.
--====================================================================
CREATE OR ALTER PROCEDURE dbo.sp_DisponibilidadZonas_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPartido, EquipoLocal, EquipoVisitante, FechaHora, EstadoPartido, Estadio, IdZona,
           NombreZona, PrecioBase, CupoZona, BoletosVendidos, BoletosDisponibles
    FROM dbo.vw_DisponibilidadZonas;
END;
GO
--===========================================================
-- Devuelve las ventas registradas para consultas y reportes.
--===========================================================
CREATE OR ALTER PROCEDURE dbo.sp_Ventas_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdVenta, Folio, FechaHoraVenta, IdCliente, Cliente, RFC, CorreoElectronico,
           IdVendedor, Vendedor, Subtotal, IVA, Total, MetodoPago, EstadoVenta, TotalBoletos,
           BoletosCancelados, Facturada, FolioFactura
    FROM dbo.vw_Ventas;
END;
GO
--=========================================
-- Devuelve el detalle de boletos vendidos.
--=========================================
CREATE OR ALTER PROCEDURE dbo.sp_BoletosVendidos_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdBoleto, Fila, Asiento, PrecioUnitario, EstadoBoleto, IdPartido, FechaPartido,
           EquipoLocal, EquipoVisitante, Estadio, Ciudad, IdZona, NombreZona, IdVenta, FolioVenta,
           FechaHoraVenta, EstadoVenta, IdCliente, Cliente, RFC
    FROM dbo.vw_BoletosVendidos;
END;
GO
--================================
-- Devuelve las facturas emitidas.
--================================
CREATE OR ALTER PROCEDURE dbo.sp_Facturas_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdFactura, FolioFactura, FechaHoraEmision, IdVenta, FolioVenta, FechaHoraVenta,
           EmisorNombre, EmisorRFC, EmisorDomicilio, ReceptorNombre, ReceptorRFC,
           ReceptorDomicilio, UsoCFDI, MetodoPago, Subtotal, IVA, Total, Cliente, EstadoVenta
    FROM dbo.vw_Facturas;
END;
GO
--=========================================================
-- Devuelve las ventas canceladas y sus datos relacionados.
--=========================================================
CREATE OR ALTER PROCEDURE dbo.sp_Cancelaciones_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCancelacion, FechaHoraCancelacion, Motivo, IdVenta, FolioVenta, FechaHoraVenta,
           Total, IdCliente, Cliente, RFC, IdUsuarioCancelo, UsuarioCancelo
    FROM dbo.vw_Cancelaciones;
END;
GO
--============================================================
-- Calcula y devuelve ingresos y boletos vendidos por partido.
--============================================================
CREATE OR ALTER PROCEDURE dbo.sp_IngresosPorPartido_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPartido, FechaHora, EquipoLocal, EquipoVisitante, Estado, Estadio,
           BoletosVendidos, Subtotal, IVA, Total
    FROM dbo.vw_IngresosPorPartido;
END;
GO
--===============================================================
-- Devuelve los asientos que ya están ocupados para cada partido.
--===============================================================
CREATE OR ALTER PROCEDURE dbo.sp_AsientosOcupados_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPartido, IdZona, NombreZona, Fila, Asiento, IdVenta
    FROM dbo.vw_AsientosOcupados;
END;
GO
--========================================================
-- Devuelve el historial de cambios realizados a usuarios.
--========================================================
CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaUsuarios_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdAuditoria, IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo,
           ModificadoPor, ModificadoPorNombre, FechaModificacion
    FROM dbo.vw_AuditoriaUsuarios;
END;
GO
--========================================================
-- Devuelve el historial de cambios realizados a clientes.
--========================================================
CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaClientes_Consultar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdAuditoria, IdCliente, DatoACambiar, DatoAnterior, DatoNuevo,
           ModificadoPor, ModificadoPorNombre, FechaModificacion
    FROM dbo.vw_AuditoriaClientes;
END;
GO

--===================================
-- Valida e inserta un usuario nuevo.
--===================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Agregar
    @Nombres VARCHAR(100), @ApPaterno VARCHAR(100), @ApMaterno VARCHAR(100) = NULL,
    @FechaNacimiento DATE, @NumeroNomina NVARCHAR(50), @Correo NVARCHAR(100),
    @Rol VARCHAR(50), @NombreUsuario VARCHAR(100), @Contrasena NVARCHAR(255), @CreadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaNacimiento >= CONVERT(date, GETDATE()) THROW 51001, 'La fecha de nacimiento debe ser anterior a hoy.', 1;
    IF @Rol NOT IN ('Administrador', 'Operativo') THROW 51002, 'El rol no es válido.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NumeroNomina = @NumeroNomina OR CorreoElectronico = @Correo OR NombreUsuario = @NombreUsuario)
        THROW 51003, 'La nómina, correo o usuario ya están registrados.', 1;
    INSERT dbo.Usuarios ([Nombre(s)], ApPaterno, ApMaterno, FechaNacimiento, NumeroNomina,
        CorreoElectronico, Rol, NombreUsuario, Contraseña, CreadoPor)
    VALUES (@Nombres, @ApPaterno, @ApMaterno, @FechaNacimiento, @NumeroNomina,
        @Correo, @Rol, @NombreUsuario, @Contrasena, @CreadoPor);
END;
GO
--==================================================
-- Actualiza un dato permitido del usuario indicado.
--==================================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Actualizar
    @IdUsuario INT, @Campo VARCHAR(40), @Valor NVARCHAR(255), @ModificadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE IdUsuario = @IdUsuario) THROW 51004, 'No se encontró el usuario.', 1;
    IF @Campo NOT IN ('CORREO ELECTRONICO', 'ROL', 'NOMBRE DE USUARIO', 'CONTRASEÑA', 'ACTIVO')
        THROW 51005, 'El campo no se puede modificar.', 1;
    IF @Campo = 'ROL' AND @Valor NOT IN ('Administrador', 'Operativo') THROW 51006, 'El rol no es válido.', 1;
    IF @Campo = 'ACTIVO' AND TRY_CONVERT(bit, @Valor) IS NULL THROW 51007, 'El estado debe ser true o false.', 1;
    EXEC sys.sp_set_session_context @key=N'UsuarioId', @value=@ModificadoPor;
    UPDATE dbo.Usuarios
    SET CorreoElectronico = CASE WHEN @Campo = 'CORREO ELECTRONICO' THEN @Valor ELSE CorreoElectronico END,
        Rol = CASE WHEN @Campo = 'ROL' THEN @Valor ELSE Rol END,
        NombreUsuario = CASE WHEN @Campo = 'NOMBRE DE USUARIO' THEN @Valor ELSE NombreUsuario END,
        Contraseña = CASE WHEN @Campo = 'CONTRASEÑA' THEN @Valor ELSE Contraseña END,
        Activo = CASE WHEN @Campo = 'ACTIVO' THEN TRY_CONVERT(bit, @Valor) ELSE Activo END
    WHERE IdUsuario = @IdUsuario;
END;
GO

--===================================
-- Valida e inserta un cliente nuevo.
--===================================
CREATE OR ALTER PROCEDURE dbo.sp_Cliente_Agregar
    @Nombres NVARCHAR(150), @ApPaterno VARCHAR(100), @ApMaterno VARCHAR(100) = NULL,
    @FechaNacimiento DATE, @Domicilio NVARCHAR(250), @RFC NVARCHAR(13), @Correo NVARCHAR(100),
    @TelefonoCasa NVARCHAR(15) = NULL, @TelefonoCelular NVARCHAR(15), @CreadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaNacimiento >= CONVERT(date, GETDATE()) THROW 51010, 'La fecha de nacimiento debe ser anterior a hoy.', 1;
    IF LEN(@RFC) NOT IN (12,13) THROW 51011, 'El RFC debe tener 12 o 13 caracteres.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Clientes WHERE RFC=@RFC) THROW 51012, 'Ya existe un cliente con ese RFC.', 1;
    INSERT dbo.Clientes ([Nombre(s)], ApPaterno, ApMaterno, FechaNacimiento, Domicilio, RFC,
        CorreoElectronico, TelefonoCasa, TelefonoCelular, CreadoPor)
    VALUES (@Nombres, @ApPaterno, @ApMaterno, @FechaNacimiento, @Domicilio, @RFC,
        @Correo, @TelefonoCasa, @TelefonoCelular, @CreadoPor);
END;
GO
--==========================================
-- Actualiza los datos del cliente indicado.
--==========================================
CREATE OR ALTER PROCEDURE dbo.sp_Cliente_Actualizar
    @IdCliente INT, @Campo VARCHAR(40), @Valor NVARCHAR(250), @ModificadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE IdCliente=@IdCliente) THROW 51013, 'No se encontró el cliente.', 1;
    IF @Campo NOT IN ('CORREO ELECTRONICO', 'TELEFONO CELULAR', 'TELEFONO DE CASA', 'DOMICILIO')
        THROW 51014, 'El campo no se puede modificar.', 1;
    EXEC sys.sp_set_session_context @key=N'UsuarioId', @value=@ModificadoPor;
    UPDATE dbo.Clientes
    SET CorreoElectronico = CASE WHEN @Campo='CORREO ELECTRONICO' THEN @Valor ELSE CorreoElectronico END,
        TelefonoCelular = CASE WHEN @Campo='TELEFONO CELULAR' THEN @Valor ELSE TelefonoCelular END,
        TelefonoCasa = CASE WHEN @Campo='TELEFONO DE CASA' THEN @Valor ELSE TelefonoCasa END,
        Domicilio = CASE WHEN @Campo='DOMICILIO' THEN @Valor ELSE Domicilio END
    WHERE IdCliente=@IdCliente;
END;
GO

--=========================================================
-- Valida e inserta un estadio y devuelve su identificador.
--=========================================================
CREATE OR ALTER PROCEDURE dbo.sp_Estadio_Agregar
    @Nombre NVARCHAR(100), @Ciudad NVARCHAR(100), @CapacidadTotal INT, @MapaEstadio VARCHAR(500), @CreadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @CapacidadTotal <= 0 THROW 51020, 'La capacidad debe ser mayor que cero.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Estadios WHERE Nombre=@Nombre) THROW 51021, 'Ya existe un estadio con ese nombre.', 1;
    INSERT dbo.Estadios (Nombre, Ciudad, CapacidadTotal, MapaEstadio, CreadoPor)
    VALUES (@Nombre, @Ciudad, @CapacidadTotal, @MapaEstadio, @CreadoPor);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Value;
END;
GO
--==========================================================================
-- Registra una zona para el estadio con la capacidad y el precio indicados.
--==========================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Zona_Agregar
    @IdEstadio INT, @NombreZona NVARCHAR(50), @PrecioBase DECIMAL(10,2), @CupoZona INT, @CreadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @PrecioBase < 0 OR @CupoZona <= 0 THROW 51022, 'El precio debe ser no negativo y el cupo mayor que cero.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Estadios WHERE IdEstadio=@IdEstadio) THROW 51023, 'No se encontró el estadio.', 1;
    INSERT dbo.Zonas (IdEstadio, NombreZona, PrecioBase, CupoZona, CreadoPor)
    VALUES (@IdEstadio, @NombreZona, @PrecioBase, @CupoZona, @CreadoPor);
END;
GO
--===============================================================
-- Valida y registra un partido con sus equipos, estadio y fecha.
--===============================================================
CREATE OR ALTER PROCEDURE dbo.sp_Partido_Agregar
    @IdEstadio INT, @FechaHora DATETIME, @EquipoLocal NVARCHAR(100), @EquipoVisitante NVARCHAR(100),
    @Estado NVARCHAR(20), @CreadoPor INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @FechaHora <= GETDATE() THROW 51024, 'No se pueden registrar partidos en el pasado.', 1;
    IF @Estado NOT IN ('Programado', 'Terminado', 'Cancelado') THROW 51025, 'El estado del partido no es válido.', 1;
    INSERT dbo.Partidos (IdEstadio, FechaHora, EquipoLocal, EquipoVisitante, Estado, CreadoPor)
    VALUES (@IdEstadio, @FechaHora, @EquipoLocal, @EquipoVisitante, @Estado, @CreadoPor);
END;
GO

--===========================================================
-- Registra la venta y sus boletos dentro de una transacción.
--===========================================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_Registrar
    @IdCliente INT, @IdUsuario INT, @IdPartido INT, @IdZona INT,
    @MetodoPago NVARCHAR(20), @AsientosJson NVARCHAR(MAX), @Folio UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @MetodoPago NOT IN (N'Crédito', N'Débito', N'Efectivo') THROW 51030, 'El método de pago no es válido.', 1;
    IF ISJSON(@AsientosJson) <> 1 THROW 51031, 'La lista de asientos no es válida.', 1;
    DECLARE @Asientos TABLE (Fila NVARCHAR(10) NOT NULL, Asiento NVARCHAR(10) NOT NULL);
    INSERT @Asientos (Fila, Asiento)
        SELECT Fila, Asiento FROM OPENJSON(@AsientosJson) WITH (Fila NVARCHAR(10), Asiento NVARCHAR(10));
    DECLARE @Cantidad INT = (SELECT COUNT(*) FROM @Asientos);
    IF @Cantidad NOT BETWEEN 1 AND 4 THROW 51032, 'La compra debe incluir de 1 a 4 boletos.', 1;
    IF EXISTS (SELECT Fila, Asiento FROM @Asientos GROUP BY Fila, Asiento HAVING COUNT(*) > 1)
        THROW 51033, 'La lista contiene asientos duplicados.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE IdCliente=@IdCliente AND Activo=1) THROW 51034, 'El cliente no está activo.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE IdUsuario=@IdUsuario AND Activo=1 AND Rol IN ('Administrador','Operativo')) THROW 51035, 'El usuario no está activo.', 1;

    BEGIN TRANSACTION;
    DECLARE @IdEstadio INT, @Precio DECIMAL(10,2), @Cupo INT;
    SELECT @IdEstadio=IdEstadio FROM dbo.Partidos WITH (UPDLOCK,HOLDLOCK)
      WHERE IdPartido=@IdPartido AND Estado='Programado' AND FechaHora>GETDATE();
    IF @IdEstadio IS NULL THROW 51036, 'El partido no está disponible para la venta.', 1;
    SELECT @Precio=PrecioBase, @Cupo=CupoZona FROM dbo.Zonas WITH (UPDLOCK,HOLDLOCK)
      WHERE IdZona=@IdZona AND IdEstadio=@IdEstadio;
    IF @Precio IS NULL THROW 51037, 'La zona no pertenece al estadio del partido.', 1;
    IF @Cantidad > @Cupo-(SELECT COUNT(*) FROM dbo.Boletos WITH (UPDLOCK,HOLDLOCK)
        WHERE IdPartido=@IdPartido AND IdZona=@IdZona AND EstadoBoleto='Vendido')
        THROW 51038, 'No hay suficientes asientos disponibles en la zona.', 1;
    IF EXISTS (SELECT 1 FROM @Asientos a JOIN dbo.Boletos b WITH (UPDLOCK,HOLDLOCK)
        ON b.IdPartido=@IdPartido AND b.IdZona=@IdZona AND b.Fila=a.Fila AND b.Asiento=a.Asiento AND b.EstadoBoleto='Vendido')
        THROW 51039, 'Uno o más asientos ya fueron vendidos.', 1;

    DECLARE @Subtotal DECIMAL(10,2)=ROUND(@Precio*@Cantidad,2), @IVA DECIMAL(10,2)=ROUND(@Precio*@Cantidad*0.16,2), @IdVenta INT;
    INSERT dbo.Ventas (Folio, IdCliente, IdUsuario, Subtotal, IVA, Total, MetodoPago, EstadoVenta)
    VALUES (@Folio, @IdCliente, @IdUsuario, @Subtotal, @IVA, @Subtotal+@IVA, @MetodoPago, 'Activa');
    SET @IdVenta=CONVERT(int,SCOPE_IDENTITY());
    INSERT dbo.DetalleVenta (IdVenta,IdPartido,IdZona,Cantidad,PrecioUnitario,Subtotal,IVA,Total)
    VALUES (@IdVenta,@IdPartido,@IdZona,@Cantidad,@Precio,@Subtotal,@IVA,@Subtotal+@IVA);
    INSERT dbo.Boletos (IdVenta,IdPartido,IdZona,Fila,Asiento,PrecioUnitario,EstadoBoleto)
    SELECT @IdVenta,@IdPartido,@IdZona,Fila,Asiento,@Precio,'Vendido' FROM @Asientos;
    INSERT dbo.Facturas (IdVenta,FolioFactura,FechaHoraEmision,EmisorNombre,EmisorRFC,EmisorDomicilio,
        ReceptorNombre,ReceptorRFC,ReceptorDomicilio,UsoCFDI,MetodoPago,Subtotal,IVA,Total)
    SELECT @IdVenta, CONCAT('DEMO-', REPLACE(CONVERT(varchar(36),@Folio),'-','')), GETDATE(),
        N'NEXO TICKET (DEMO)',N'DEMO000000XXX',N'DOMICILIO DE PRUEBA',
        CONCAT(c.[Nombre(s)],N' ',c.ApPaterno,ISNULL(CONCAT(N' ',c.ApMaterno),N'')),
        CASE WHEN LEN(c.RFC) IN (12,13) THEN c.RFC ELSE N'DEMO000000XXX' END,
        c.Domicilio,N'Proyecto escolar (demo)',@MetodoPago,@Subtotal,@IVA,@Subtotal+@IVA
    FROM dbo.Clientes c WHERE c.IdCliente=@IdCliente;
    COMMIT TRANSACTION;
    SELECT @IdVenta AS Value;
END;
GO
--========================================================
-- Cancela una venta usando su folio y registra el motivo.
--========================================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_Cancelar
    @Folio UNIQUEIDENTIFIER, @IdUsuario INT, @Motivo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF dbo.fn_EsAdmin(@IdUsuario)<>1 THROW 51040, 'Solo un administrador activo puede cancelar ventas.', 1;
    IF NULLIF(LTRIM(RTRIM(@Motivo)),N'') IS NULL THROW 51041, 'Captura el motivo de cancelación.', 1;
    DECLARE @IdVenta INT=(SELECT IdVenta FROM dbo.Ventas WHERE Folio=@Folio AND EstadoVenta='Activa');
    IF @IdVenta IS NULL THROW 51042, 'El folio no existe o ya fue cancelado.', 1;
    BEGIN TRANSACTION;
    INSERT dbo.Cancelaciones (IdVenta,IdUsuario,Motivo) VALUES (@IdVenta,@IdUsuario,@Motivo);
    /* El trigger procesa venta y boletos. Al borrar la factura se invalida su relación. */
    DELETE dbo.Facturas WHERE IdVenta=@IdVenta;
    COMMIT TRANSACTION;
END;
GO
--==================================================================
-- Cancela una venta mediante su identificador y registra el motivo.
--==================================================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_CancelarId
    @IdVenta INT, @IdUsuario INT, @Motivo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF dbo.fn_EsAdmin(@IdUsuario)<>1 THROW 51043, 'Solo un administrador activo puede cancelar ventas.', 1;
    IF NULLIF(LTRIM(RTRIM(@Motivo)),N'') IS NULL THROW 51044, 'Captura el motivo de cancelación.', 1;
    DECLARE @Folio UNIQUEIDENTIFIER=(SELECT Folio FROM dbo.Ventas WHERE IdVenta=@IdVenta AND EstadoVenta='Activa');
    IF @Folio IS NULL THROW 51045, 'La venta no existe o ya fue cancelada.', 1;
    EXEC dbo.sp_Venta_Cancelar @Folio, @IdUsuario, @Motivo;
END;
GO

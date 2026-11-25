USE PIA_V3;
GO

-- Eliminar la restricción UNIQUE original
ALTER TABLE Boletos DROP CONSTRAINT UQ_Asiento_Partido;
GO

-- Crear índice único filtrado: solo bloquea asientos con estado 'Vendido'
CREATE UNIQUE INDEX UQ_Asiento_Partido_Vendido
ON Boletos(IdPartido, IdZona, Fila, Asiento)
WHERE EstadoBoleto = 'Vendido';
GO

-- =============================================
-- VW: Usuarios (sin exponer el hash de contraseña)
-- =============================================
CREATE VIEW vw_Usuarios AS
SELECT  IdUsuario,
        [Nombre(s)] AS Nombres,
        ApPaterno, ApMaterno,
        FechaNacimiento,
        DATEDIFF(YEAR, FechaNacimiento, GETDATE()) AS Edad,
        NumeroNomina, CorreoElectronico, Rol, NombreUsuario,
        Activo, CreadoPor, FechaRegistro
FROM Usuarios;
GO

-- =============================================
-- VW: Clientes con datos del usuario creador
-- =============================================
CREATE VIEW vw_Clientes AS
SELECT  c.IdCliente, c.[Nombre(s)] AS Nombres, c.ApPaterno, c.ApMaterno,
        c.FechaNacimiento,
        DATEDIFF(YEAR, c.FechaNacimiento, GETDATE()) AS Edad,
        c.Domicilio, c.RFC, c.CorreoElectronico,
        c.TelefonoCasa, c.TelefonoCelular,
        c.Activo, c.FechaHoraRegistro,
        u.NombreUsuario AS CreadoPorNombre,
        u.Rol AS CreadoPorRol
FROM Clientes c
INNER JOIN Usuarios u ON c.CreadoPor = u.IdUsuario;
GO

-- =============================================
-- VW: Estadios con resumen de zonas
-- =============================================
CREATE VIEW vw_Estadios AS
SELECT  e.IdEstadio, e.Nombre, e.Ciudad, e.CapacidadTotal, e.MapaEstadio,
        COUNT(z.IdZona) AS TotalZonas,
        ISNULL(SUM(z.CupoZona), 0) AS CupoAsignado,
        e.CapacidadTotal - ISNULL(SUM(z.CupoZona), 0) AS CapacidadDisponible,
        e.FechaHoraRegistro
FROM Estadios e
LEFT JOIN Zonas z ON e.IdEstadio = z.IdEstadio
GROUP BY e.IdEstadio, e.Nombre, e.Ciudad, e.CapacidadTotal,
         e.MapaEstadio, e.FechaHoraRegistro;
GO

-- =============================================
-- VW: Partidos con disponibilidad general
-- =============================================
CREATE VIEW vw_Partidos AS
SELECT  p.IdPartido, p.FechaHora, p.EquipoLocal, p.EquipoVisitante, p.Estado,
        e.IdEstadio, e.Nombre AS Estadio, e.Ciudad, e.CapacidadTotal,
        ISNULL(SUM(b.CantidadVendida), 0) AS BoletosVendidos,
        e.CapacidadTotal - ISNULL(SUM(b.CantidadVendida), 0) AS BoletosDisponibles,
        p.FechaHoraRegistro,
        u.NombreUsuario AS CreadoPor
FROM Partidos p
INNER JOIN Estadios e ON p.IdEstadio = e.IdEstadio
LEFT JOIN (
    SELECT IdPartido, COUNT(*) AS CantidadVendida
    FROM Boletos
    WHERE EstadoBoleto = 'Vendido'
    GROUP BY IdPartido
) b ON p.IdPartido = b.IdPartido
INNER JOIN Usuarios u ON p.CreadoPor = u.IdUsuario
GROUP BY p.IdPartido, p.FechaHora, p.EquipoLocal, p.EquipoVisitante,
         p.Estado, e.IdEstadio, e.Nombre, e.Ciudad, e.CapacidadTotal,
         p.FechaHoraRegistro, u.NombreUsuario;
GO

-- =============================================
-- VW: Disponibilidad por zona y partido
-- =============================================
CREATE VIEW vw_DisponibilidadZonas AS
SELECT  p.IdPartido, p.EquipoLocal, p.EquipoVisitante, p.FechaHora, p.Estado AS EstadoPartido,
        e.Nombre AS Estadio,
        z.IdZona, z.NombreZona, z.PrecioBase, z.CupoZona,
        ISNULL(COUNT(b.IdBoleto), 0) AS BoletosVendidos,
        z.CupoZona - ISNULL(COUNT(b.IdBoleto), 0) AS BoletosDisponibles
FROM Partidos p
INNER JOIN Estadios e ON p.IdEstadio = e.IdEstadio
INNER JOIN Zonas z ON e.IdEstadio = z.IdEstadio
LEFT JOIN Boletos b ON b.IdPartido = p.IdPartido
                    AND b.IdZona = z.IdZona
                    AND b.EstadoBoleto = 'Vendido'
GROUP BY p.IdPartido, p.EquipoLocal, p.EquipoVisitante, p.FechaHora,
         p.Estado, e.Nombre, z.IdZona, z.NombreZona, z.PrecioBase, z.CupoZona;
GO

-- =============================================
-- VW: Ventas con datos completos del cliente y vendedor
-- =============================================
CREATE VIEW vw_Ventas AS
SELECT  v.IdVenta, v.Folio, v.FechaHoraVenta,
        c.IdCliente,
        c.[Nombre(s)] + ' ' + c.ApPaterno + ISNULL(' ' + c.ApMaterno, '') AS Cliente,
        c.RFC, c.CorreoElectronico,
        u.IdUsuario AS IdVendedor, u.NombreUsuario AS Vendedor,
        v.Subtotal, v.IVA, v.Total, v.MetodoPago, v.EstadoVenta,
        (SELECT COUNT(*) FROM Boletos b WHERE b.IdVenta = v.IdVenta) AS TotalBoletos,
        (SELECT COUNT(*) FROM Boletos b WHERE b.IdVenta = v.IdVenta AND b.EstadoBoleto = 'Cancelado') AS BoletosCancelados,
        CASE WHEN f.IdFactura IS NOT NULL THEN 1 ELSE 0 END AS Facturada,
        f.FolioFactura
FROM Ventas v
INNER JOIN Clientes c ON v.IdCliente = c.IdCliente
INNER JOIN Usuarios u ON v.IdUsuario = u.IdUsuario
LEFT JOIN Facturas f ON v.IdVenta = f.IdVenta;
GO

-- =============================================
-- VW: Boletos vendidos (detalle completo)
-- =============================================
CREATE VIEW vw_BoletosVendidos AS
SELECT  b.IdBoleto, b.Fila, b.Asiento, b.PrecioUnitario, b.EstadoBoleto,
        p.IdPartido, p.FechaHora AS FechaPartido, p.EquipoLocal, p.EquipoVisitante,
        e.Nombre AS Estadio, e.Ciudad,
        z.IdZona, z.NombreZona,
        v.IdVenta, v.Folio AS FolioVenta, v.FechaHoraVenta, v.EstadoVenta,
        c.IdCliente,
        c.[Nombre(s)] + ' ' + c.ApPaterno AS Cliente, c.RFC
FROM Boletos b
INNER JOIN Partidos p ON b.IdPartido = p.IdPartido
INNER JOIN Estadios e ON p.IdEstadio = e.IdEstadio
INNER JOIN Zonas z ON b.IdZona = z.IdZona
INNER JOIN Ventas v ON b.IdVenta = v.IdVenta
INNER JOIN Clientes c ON v.IdCliente = c.IdCliente;
GO

-- =============================================
-- VW: Facturas completas
-- =============================================
CREATE VIEW vw_Facturas AS
SELECT  f.IdFactura, f.FolioFactura, f.FechaHoraEmision,
        f.IdVenta, v.Folio AS FolioVenta, v.FechaHoraVenta,
        f.EmisorNombre, f.EmisorRFC, f.EmisorDomicilio,
        f.ReceptorNombre, f.ReceptorRFC, f.ReceptorDomicilio,
        f.UsoCFDI, f.MetodoPago,
        f.Subtotal, f.IVA, f.Total,
        c.[Nombre(s)] + ' ' + c.ApPaterno AS Cliente,
        v.EstadoVenta
FROM Facturas f
INNER JOIN Ventas v ON f.IdVenta = v.IdVenta
INNER JOIN Clientes c ON v.IdCliente = c.IdCliente;
GO

-- =============================================
-- VW: Cancelaciones con detalle
-- =============================================
CREATE VIEW vw_Cancelaciones AS
SELECT  ca.IdCancelacion, ca.FechaHoraCancelacion, ca.Motivo,
        v.IdVenta, v.Folio AS FolioVenta, v.FechaHoraVenta, v.Total,
        c.IdCliente,
        c.[Nombre(s)] + ' ' + c.ApPaterno AS Cliente, c.RFC,
        u.IdUsuario AS IdUsuarioCancelo, u.NombreUsuario AS UsuarioCancelo
FROM Cancelaciones ca
INNER JOIN Ventas v ON ca.IdVenta = v.IdVenta
INNER JOIN Clientes c ON v.IdCliente = c.IdCliente
INNER JOIN Usuarios u ON ca.IdUsuario = u.IdUsuario;
GO

-- =============================================
-- VW: Ingresos por partido (solo ventas activas)
-- =============================================
CREATE VIEW vw_IngresosPorPartido AS
SELECT  p.IdPartido, p.FechaHora, p.EquipoLocal, p.EquipoVisitante, p.Estado,
        e.Nombre AS Estadio,
        ISNULL(SUM(CASE WHEN v.EstadoVenta = 'Activa' THEN dv.Cantidad ELSE 0 END), 0) AS BoletosVendidos,
        ISNULL(SUM(CASE WHEN v.EstadoVenta = 'Activa' THEN dv.Subtotal ELSE 0 END), 0) AS Subtotal,
        ISNULL(SUM(CASE WHEN v.EstadoVenta = 'Activa' THEN dv.IVA ELSE 0 END), 0)      AS IVA,
        ISNULL(SUM(CASE WHEN v.EstadoVenta = 'Activa' THEN dv.Total ELSE 0 END), 0)    AS Total
FROM Partidos p
INNER JOIN Estadios e ON p.IdEstadio = e.IdEstadio
LEFT JOIN DetalleVenta dv ON p.IdPartido = dv.IdPartido
LEFT JOIN Ventas v ON dv.IdVenta = v.IdVenta
GROUP BY p.IdPartido, p.FechaHora, p.EquipoLocal, p.EquipoVisitante,
         p.Estado, e.Nombre;
GO

-- =============================================
-- VW: Asientos ocupados por partido (mapa de calor)
-- =============================================
CREATE VIEW vw_AsientosOcupados AS
SELECT  b.IdPartido, b.IdZona, z.NombreZona, b.Fila, b.Asiento, b.IdVenta
FROM Boletos b
INNER JOIN Zonas z ON b.IdZona = z.IdZona
WHERE b.EstadoBoleto = 'Vendido';
GO

-- =============================================
-- VW: Auditoría de Usuarios
-- =============================================
CREATE VIEW vw_AuditoriaUsuarios AS
SELECT  a.IdAuditoria, a.IdUsuario, a.DatoACambiar,
        a.DatoAnterior, a.DatoNuevo,
        a.ModificadoPor,
        u.NombreUsuario AS ModificadoPorNombre,
        a.FechaModificacion
FROM AuditoriaUsuarios a
INNER JOIN Usuarios u ON a.ModificadoPor = u.IdUsuario;
GO

-- =============================================
-- VW: Auditoría de Clientes
-- =============================================
CREATE VIEW vw_AuditoriaClientes AS
SELECT  a.IdAuditoria, a.IdCliente, a.DatoACambiar,
        a.DatoAnterior, a.DatoNuevo,
        a.ModificadoPor,
        u.NombreUsuario AS ModificadoPorNombre,
        a.FechaModificacion
FROM AuditoriaClientes a
INNER JOIN Usuarios u ON a.ModificadoPor = u.IdUsuario;
GO



USE PIA_V3;
GO

-- =============================================
-- FN: Calcular edad exacta a partir de la fecha de nacimiento
-- =============================================
CREATE FUNCTION fn_CalcularEdad (@FechaNacimiento DATE)
RETURNS INT
AS
BEGIN
    RETURN DATEDIFF(YEAR, @FechaNacimiento, GETDATE())
         - CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, @FechaNacimiento, GETDATE()), @FechaNacimiento) > GETDATE()
                THEN 1 ELSE 0 END;
END;
GO

-- =============================================
-- FN: Asientos disponibles en una zona para un partido
-- =============================================
CREATE FUNCTION fn_AsientosDisponibles (@IdPartido INT, @IdZona INT)
RETURNS INT
AS
BEGIN
    DECLARE @Cupo INT, @Vendidos INT, @Resultado INT;

    SELECT @Cupo = CupoZona FROM Zonas WHERE IdZona = @IdZona;
    IF @Cupo IS NULL RETURN 0;

    SELECT @Vendidos = COUNT(*) FROM Boletos
    WHERE IdPartido = @IdPartido AND IdZona = @IdZona AND EstadoBoleto = 'Vendido';

    SET @Resultado = @Cupo - ISNULL(@Vendidos, 0);
    RETURN CASE WHEN @Resultado < 0 THEN 0 ELSE @Resultado END;
END;
GO

-- =============================================
-- FN: Verificar si un asiento específico está disponible
-- =============================================
CREATE FUNCTION fn_AsientoDisponible
(
    @IdPartido INT, @IdZona INT,
    @Fila NVARCHAR(10), @Asiento NVARCHAR(10)
)
RETURNS BIT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM Boletos
        WHERE IdPartido = @IdPartido
          AND IdZona = @IdZona
          AND Fila = @Fila
          AND Asiento = @Asiento
          AND EstadoBoleto = 'Vendido'
    )
        RETURN 0;
    RETURN 1;
END;
GO

-- =============================================
-- FN: Total de compras activas de un cliente
-- =============================================
CREATE FUNCTION fn_TotalComprasCliente (@IdCliente INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Total DECIMAL(10,2);
    SELECT @Total = ISNULL(SUM(Total), 0)
    FROM Ventas
    WHERE IdCliente = @IdCliente AND EstadoVenta = 'Activa';
    RETURN @Total;
END;
GO

-- =============================================
-- FN: Verificar si un usuario es Administrador activo
-- =============================================
CREATE FUNCTION fn_EsAdmin (@IdUsuario INT)
RETURNS BIT
AS
BEGIN
    RETURN CASE WHEN EXISTS (
        SELECT 1 FROM Usuarios
        WHERE IdUsuario = @IdUsuario AND Rol = 'Administrador' AND Activo = 1
    ) THEN 1 ELSE 0 END;
END;
GO

-- =============================================
-- FN: Calcular IVA (por defecto 16%)
-- =============================================
CREATE FUNCTION fn_CalcularIVA
(
    @Monto DECIMAL(10,2),
    @Tasa  DECIMAL(5,4) = 0.16
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN ROUND(@Monto * @Tasa, 2);
END;
GO

-- =============================================
-- FN: Obtener el precio vigente de una zona
-- =============================================
CREATE FUNCTION fn_PrecioZona (@IdZona INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Precio DECIMAL(10,2);
    SELECT @Precio = PrecioBase FROM Zonas WHERE IdZona = @IdZona;
    RETURN ISNULL(@Precio, 0);
END;
GO



-- =============================================
-- TRG: Auditoría de cambios en Usuarios
-- Nota: usa SESSION_CONTEXT para saber QUIÉN modifica.
--       Si no está definido, usa el propio IdUsuario como fallback.
--       En la app ejecutar: EXEC sp_set_session_context 'UsuarioId', @id;
-- =============================================
CREATE TRIGGER trg_AuditoriaUsuarios_Update
ON Usuarios
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @ModificadoPor INT = ISNULL(
        TRY_CAST(SESSION_CONTEXT(N'UsuarioId') AS INT),
        (SELECT TOP 1 IdUsuario FROM inserted));

    -- Nombre(s)
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'Nombre(s)', d.[Nombre(s)], i.[Nombre(s)], @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.[Nombre(s)],'') <> ISNULL(d.[Nombre(s)],'');

    -- ApPaterno
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'ApPaterno', d.ApPaterno, i.ApPaterno, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.ApPaterno,'') <> ISNULL(d.ApPaterno,'');

    -- ApMaterno
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'ApMaterno', d.ApMaterno, i.ApMaterno, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.ApMaterno,'') <> ISNULL(d.ApMaterno,'');

    -- FechaNacimiento
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'FechaNacimiento',
           CONVERT(VARCHAR(10), d.FechaNacimiento, 120),
           CONVERT(VARCHAR(10), i.FechaNacimiento, 120), @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE i.FechaNacimiento <> d.FechaNacimiento;

    -- NumeroNomina
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'NumeroNomina', d.NumeroNomina, i.NumeroNomina, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.NumeroNomina,'') <> ISNULL(d.NumeroNomina,'');

    -- CorreoElectronico
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'CorreoElectronico', d.CorreoElectronico, i.CorreoElectronico, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.CorreoElectronico,'') <> ISNULL(d.CorreoElectronico,'');

    -- Rol
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'Rol', d.Rol, i.Rol, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.Rol,'') <> ISNULL(d.Rol,'');

    -- NombreUsuario
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'NombreUsuario', d.NombreUsuario, i.NombreUsuario, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.NombreUsuario,'') <> ISNULL(d.NombreUsuario,'');

    -- Contraseña (nunca se guarda el valor real)
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'Contraseña', '********', '********', @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE ISNULL(i.Contraseña,'') <> ISNULL(d.Contraseña,'');

    -- Activo
    INSERT INTO AuditoriaUsuarios (IdUsuario, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdUsuario AS VARCHAR(100)), 'Activo',
           CAST(d.Activo AS VARCHAR(10)), CAST(i.Activo AS VARCHAR(10)), @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdUsuario = d.IdUsuario
    WHERE i.Activo <> d.Activo;
END;
GO

-- =============================================
-- TRG: Auditoría de cambios en Clientes
-- =============================================
CREATE TRIGGER trg_AuditoriaClientes_Update
ON Clientes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @ModificadoPor INT = ISNULL(
        TRY_CAST(SESSION_CONTEXT(N'UsuarioId') AS INT),
        (SELECT TOP 1 CreadoPor FROM inserted));

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'Nombre(s)', d.[Nombre(s)], i.[Nombre(s)], @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.[Nombre(s)],'') <> ISNULL(d.[Nombre(s)],'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'ApPaterno', d.ApPaterno, i.ApPaterno, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.ApPaterno,'') <> ISNULL(d.ApPaterno,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'ApMaterno', d.ApMaterno, i.ApMaterno, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.ApMaterno,'') <> ISNULL(d.ApMaterno,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'FechaNacimiento',
           CONVERT(VARCHAR(10), d.FechaNacimiento, 120),
           CONVERT(VARCHAR(10), i.FechaNacimiento, 120), @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE i.FechaNacimiento <> d.FechaNacimiento;

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'Domicilio', d.Domicilio, i.Domicilio, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.Domicilio,'') <> ISNULL(d.Domicilio,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'RFC', d.RFC, i.RFC, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.RFC,'') <> ISNULL(d.RFC,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'CorreoElectronico', d.CorreoElectronico, i.CorreoElectronico, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.CorreoElectronico,'') <> ISNULL(d.CorreoElectronico,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'TelefonoCasa', d.TelefonoCasa, i.TelefonoCasa, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.TelefonoCasa,'') <> ISNULL(d.TelefonoCasa,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'TelefonoCelular', d.TelefonoCelular, i.TelefonoCelular, @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE ISNULL(i.TelefonoCelular,'') <> ISNULL(d.TelefonoCelular,'');

    INSERT INTO AuditoriaClientes (IdCliente, DatoACambiar, DatoAnterior, DatoNuevo, ModificadoPor)
    SELECT CAST(i.IdCliente AS VARCHAR(100)), 'Activo',
           CAST(d.Activo AS VARCHAR(10)), CAST(i.Activo AS VARCHAR(10)), @ModificadoPor
    FROM inserted i INNER JOIN deleted d ON i.IdCliente = d.IdCliente
    WHERE i.Activo <> d.Activo;
END;
GO

-- =============================================
-- TRG: Evitar que la suma de cupos de zonas supere la capacidad del estadio
-- =============================================
CREATE TRIGGER trg_ValidarCapacidadZonas
ON Zonas
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT z.IdEstadio
        FROM Zonas z
        INNER JOIN inserted i ON z.IdEstadio = i.IdEstadio
        GROUP BY z.IdEstadio
        HAVING SUM(z.CupoZona) > (SELECT CapacidadTotal FROM Estadios WHERE IdEstadio = z.IdEstadio)
    )
    BEGIN
        RAISERROR('La suma de cupos de las zonas excede la capacidad total del estadio.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: Evitar reducir la capacidad de un estadio por debajo de la suma de sus zonas
-- =============================================
CREATE TRIGGER trg_ValidarReduccionCapacidadEstadio
ON Estadios
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE ISNULL((SELECT SUM(CupoZona) FROM Zonas WHERE IdEstadio = i.IdEstadio), 0) > i.CapacidadTotal
    )
    BEGIN
        RAISERROR('La nueva capacidad del estadio es menor que la suma de cupos de sus zonas.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: La zona del detalle debe pertenecer al estadio del partido
-- =============================================
CREATE TRIGGER trg_ValidarDetalleVentaZonaPartido
ON DetalleVenta
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Partidos p ON i.IdPartido = p.IdPartido
        INNER JOIN Zonas z ON i.IdZona = z.IdZona
        WHERE p.IdEstadio <> z.IdEstadio
    )
    BEGIN
        RAISERROR('La zona especificada no pertenece al estadio del partido.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: La zona del boleto debe pertenecer al estadio del partido
-- =============================================
CREATE TRIGGER trg_ValidarBoletoZonaPartido
ON Boletos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Partidos p ON i.IdPartido = p.IdPartido
        INNER JOIN Zonas z ON i.IdZona = z.IdZona
        WHERE p.IdEstadio <> z.IdEstadio
    )
    BEGIN
        RAISERROR('La zona del boleto no pertenece al estadio del partido.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: No vender más boletos que el cupo de la zona
-- =============================================
CREATE TRIGGER trg_ValidarCapacidadBoletos
ON Boletos
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT b.IdPartido, b.IdZona
        FROM Boletos b
        INNER JOIN Zonas z ON b.IdZona = z.IdZona
        WHERE b.EstadoBoleto = 'Vendido'
          AND EXISTS (SELECT 1 FROM inserted i WHERE i.IdPartido = b.IdPartido AND i.IdZona = b.IdZona)
        GROUP BY b.IdPartido, b.IdZona, z.CupoZona
        HAVING COUNT(b.IdBoleto) > z.CupoZona
    )
    BEGIN
        RAISERROR('Se excede el cupo de la zona para el partido.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: Validar cantidad y precios en DetalleVenta
-- =============================================
CREATE TRIGGER trg_ValidarCantidadDetalleVenta
ON DetalleVenta
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE Cantidad <= 0 OR PrecioUnitario < 0)
    BEGIN
        RAISERROR('La cantidad debe ser mayor a 0 y el precio no puede ser negativo.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: No vender boletos para partidos cancelados
-- =============================================
CREATE TRIGGER trg_ValidarPartidoActivoEnVenta
ON DetalleVenta
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        INNER JOIN Partidos p ON i.IdPartido = p.IdPartido
        WHERE p.Estado = 'Cancelado'
    )
    BEGIN
        RAISERROR('No se pueden vender boletos para un partido cancelado.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: Solo facturar ventas activas
-- =============================================
CREATE TRIGGER trg_ValidarFacturaVentaActiva
ON Facturas
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        INNER JOIN Ventas v ON i.IdVenta = v.IdVenta
        WHERE v.EstadoVenta <> 'Activa'
    )
    BEGIN
        RAISERROR('Solo se pueden facturar ventas activas.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- TRG: Validar que la venta y la zona de un boleto sean coherentes con su detalle
-- =============================================
CREATE TRIGGER trg_ValidarBoletoDetalle
ON Boletos
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Opcional: valida que el boleto pertenezca a una venta que tenga ese partido+zona en DetalleVenta
    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM DetalleVenta dv
            WHERE dv.IdVenta = i.IdVenta
              AND dv.IdPartido = i.IdPartido
              AND dv.IdZona = i.IdZona
        )
    )
    BEGIN
        RAISERROR('El boleto no corresponde a ningún detalle de venta de la misma venta/partido/zona.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

USE PIA_V3;
GO

-- =============================================
-- TRG: Auto-calcular Subtotal/IVA/Total en DetalleVenta
-- (IVA 16% por defecto)
-- =============================================
CREATE TRIGGER trg_CalcularTotalesDetalleVenta
ON DetalleVenta
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dv
    SET Subtotal = i.Cantidad * i.PrecioUnitario,
        IVA      = ROUND(i.Cantidad * i.PrecioUnitario * 0.16, 2),
        Total    = ROUND(i.Cantidad * i.PrecioUnitario * 1.16, 2)
    FROM DetalleVenta dv
    INNER JOIN inserted i ON dv.IdDetalleVenta = i.IdDetalleVenta
    WHERE dv.Subtotal <> i.Cantidad * i.PrecioUnitario
       OR dv.IVA      <> ROUND(i.Cantidad * i.PrecioUnitario * 0.16, 2)
       OR dv.Total    <> ROUND(i.Cantidad * i.PrecioUnitario * 1.16, 2);
END;
GO

-- =============================================
-- TRG: Recalcular totales en Ventas cuando cambia su DetalleVenta
-- =============================================
CREATE TRIGGER trg_ActualizarTotalesVenta
ON DetalleVenta
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @VentasAfectadas TABLE (IdVenta INT PRIMARY KEY);

    INSERT INTO @VentasAfectadas (IdVenta)
    SELECT IdVenta FROM inserted
    UNION
    SELECT IdVenta FROM deleted;

    UPDATE v
    SET Subtotal = ISNULL((SELECT SUM(dv.Subtotal) FROM DetalleVenta dv WHERE dv.IdVenta = v.IdVenta), 0),
        IVA      = ISNULL((SELECT SUM(dv.IVA)      FROM DetalleVenta dv WHERE dv.IdVenta = v.IdVenta), 0),
        Total    = ISNULL((SELECT SUM(dv.Total)    FROM DetalleVenta dv WHERE dv.IdVenta = v.IdVenta), 0)
    FROM Ventas v
    INNER JOIN @VentasAfectadas va ON v.IdVenta = va.IdVenta;
END;
GO

-- =============================================
-- TRG: Procesar cancelación -> marca venta y boletos
-- =============================================
CREATE TRIGGER trg_ProcesarCancelacion
ON Cancelaciones
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Cambiar estado de la venta
    UPDATE v
    SET EstadoVenta = 'Cancelada'
    FROM Ventas v
    INNER JOIN inserted i ON v.IdVenta = i.IdVenta;

    -- Cancelar todos los boletos de esa venta
    UPDATE b
    SET EstadoBoleto = 'Cancelado'
    FROM Boletos b
    INNER JOIN inserted i ON b.IdVenta = i.IdVenta;
END;
GO

-- =============================================
-- TRG: Impedir reactivar una venta cancelada
-- =============================================
CREATE TRIGGER trg_BloquearReactivacionVenta
ON Ventas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i INNER JOIN deleted d ON i.IdVenta = d.IdVenta
        WHERE d.EstadoVenta = 'Cancelada' AND i.EstadoVenta = 'Activa'
    )
    BEGIN
        RAISERROR('No se puede reactivar una venta cancelada.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
SELECT 
    COALESCE(OBJECT_NAME(parent_id), 'DATABASE') AS Tabla,
    name AS TriggerName,
    type_desc AS Tipo,
    is_disabled
FROM sys.triggers
WHERE is_disabled = 0;   

-- =============================================
-- CREACIÓN DE LA BASE DE DATOS
-- =============================================
CREATE DATABASE PIA_V3
GO

USE PIA_V3
GO



-- =============================================
-- 1. TABLA DE USUARIOS (Usuaruios del sistema)
-- =============================================
CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    [Nombre(s)] VARCHAR(100) NOT NULL,
    ApPaterno VARCHAR(100) NOT NULL,
    ApMaterno VARCHAR(100) NULL,
    FechaNacimiento DATE NOT NULL,
    NumeroNomina NVARCHAR(50) NOT NULL UNIQUE,
    CorreoElectronico NVARCHAR(100) NOT NULL UNIQUE,
    Rol VARCHAR(50) CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Administrador', 'Operativo')),
    NombreUsuario VARCHAR(100) NOT NULL,
    Contraseña NVARCHAR(255) NOT NULL, -- Aquí se guardará el Hash
    Activo BIT NOT NULL DEFAULT 'TRUE',
    CreadoPor INT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE());
GO

-- =============================================
-- TABLA DE AUDITORÍA DE USAURIOS
-- =============================================
CREATE TABLE AuditoriaUsuarios (
    IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario VARCHAR(100) NULL,
    DatoACambiar NVARCHAR(255) NULL,
    DatoAnterior NVARCHAR(MAX) NULL, 
    DatoNuevo NVARCHAR(MAX) NULL,     
    ModificadoPor INT NOT NULL CONSTRAINT FK_Usuario_Modifico_Usuario FOREIGN KEY (ModificadoPor) REFERENCES Usuarios(IdUsuario),
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. TABLA DE CLIENTES
-- =============================================
CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    [Nombre(s)] NVARCHAR(150) NOT NULL,
    ApPaterno VARCHAR(100) NOT NULL,
    ApMaterno VARCHAR(100) NULL,
    FechaNacimiento DATE NOT NULL,
    Domicilio NVARCHAR(250) NOT NULL,
    RFC NVARCHAR(13) NOT NULL UNIQUE, 
    CorreoElectronico NVARCHAR(100) NOT NULL,
    TelefonoCasa NVARCHAR(15) NULL,
    TelefonoCelular NVARCHAR(15) NOT NULL,
    Activo BIT NOT NULL DEFAULT 'TRUE',
    CreadoPor INT NOT NULL,
    FechaHoraRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Clientes_Usuarios_Creador FOREIGN KEY (CreadoPor) REFERENCES Usuarios(IdUsuario),
    -- Validación de longitud de RFC (Persona física 13, moral 12)
    CONSTRAINT CHK_RFC_Longitud CHECK (LEN(RFC) IN (12, 13))
);
GO

-- =============================================
-- TABLA DE AUDITORÍA DE CLIENTES
-- =============================================
CREATE TABLE AuditoriaClientes (
    IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente VARCHAR(100) NULL,
    DatoACambiar NVARCHAR(255) NULL,
    DatoAnterior NVARCHAR(MAX) NULL, 
    DatoNuevo NVARCHAR(MAX) NULL,     
    ModificadoPor INT NOT NULL CONSTRAINT FK_Usuario_Modifico_Cliente FOREIGN KEY (ModificadoPor) REFERENCES Usuarios(IdUsuario),
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE()
);
GO


-- =============================================
-- 3. TABLA DE ESTADIOS
-- =============================================
CREATE TABLE Estadios (
    IdEstadio INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Ciudad NVARCHAR(100) NOT NULL,
    CapacidadTotal INT NOT NULL CHECK (CapacidadTotal > 0),
    MapaEstadio VARCHAR(500) NULL, -- Ruta o URL del mapa
    CreadoPor INT NOT NULL,
    FechaHoraRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Estadios_Usuarios FOREIGN KEY (CreadoPor) REFERENCES Usuarios(IdUsuario)
);
GO

-- =============================================
-- 4. TABLA DE ZONAS (Por Estadio)
-- =============================================
CREATE TABLE Zonas (
    IdZona INT IDENTITY(1,1) PRIMARY KEY,
    IdEstadio INT NOT NULL,
    NombreZona NVARCHAR(50) NOT NULL,
    PrecioBase DECIMAL(10,2) NOT NULL CHECK (PrecioBase >= 0),
    CupoZona INT NOT NULL CHECK (CupoZona > 0),
    CreadoPor INT NOT NULL,
    FechaHoraRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Zonas_Estadios FOREIGN KEY (IdEstadio) REFERENCES Estadios(IdEstadio),
    CONSTRAINT FK_Zonas_Usuarios FOREIGN KEY (CreadoPor) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT UQ_Zona_Estadio UNIQUE (IdEstadio, NombreZona)
);
GO

-- =============================================
-- 5. TABLA DE PARTIDOS
-- =============================================
CREATE TABLE Partidos (
    IdPartido INT IDENTITY(1,1) PRIMARY KEY,
    IdEstadio INT NOT NULL,
    FechaHora DATETIME NOT NULL,
    EquipoLocal NVARCHAR(100) NOT NULL,
    EquipoVisitante NVARCHAR(100) NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Programado',
    CreadoPor INT NOT NULL,
    FechaHoraRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Partidos_Estadios FOREIGN KEY (IdEstadio) REFERENCES Estadios(IdEstadio),
    CONSTRAINT FK_Partidos_Usuarios FOREIGN KEY (CreadoPor) REFERENCES Usuarios(IdUsuario),
    -- Validación de estados según el documento
    CONSTRAINT CHK_EstadoPartido CHECK (Estado IN ('Programado', 'Terminado', 'Cancelado'))
);
GO


-- =============================================
-- 6. TABLA DE VENTAS (Transacciones de Compra)
-- =============================================
CREATE TABLE Ventas (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Folio UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() UNIQUE, -- Código de compra (GUID)
    IdCliente INT NOT NULL,
    IdUsuario INT NOT NULL,
    FechaHoraVenta DATETIME NOT NULL DEFAULT GETDATE(),
    Subtotal DECIMAL(10,2) NOT NULL,
    IVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    MetodoPago NVARCHAR(20) NOT NULL,
    EstadoVenta NVARCHAR(20) NOT NULL DEFAULT 'Activa',
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente),
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT CHK_MetodoPago CHECK (MetodoPago IN ('Crédito', 'Débito', 'Efectivo')),
    CONSTRAINT CHK_EstadoVenta CHECK (EstadoVenta IN ('Activa', 'Cancelada'))
);
GO
-- =============================================
-- 7. TABLA DETALLE DE VENTA
-- =============================================
CREATE TABLE DetalleVenta
(
    IdDetalleVenta INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdPartido INT NOT NULL,
    IdZona INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    IVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    CONSTRAINT FK_DetalleVenta_Partido FOREIGN KEY (IdPartido) REFERENCES Partidos(IdPartido),
    CONSTRAINT FK_DetalleVenta_Zona FOREIGN KEY (IdZona) REFERENCES Zonas(IdZona)
);
GO

-- =============================================
-- 8. TABLA DE BOLETOS (Detalle de la Venta)
-- =============================================
-- Se separa en tabla aparte para poder asignar fila y asiento,
-- y para facilitar la liberación de asientos en caso de cancelación.
CREATE TABLE Boletos (
    IdBoleto INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdPartido INT NOT NULL,
    IdZona INT NOT NULL,
    Fila NVARCHAR(10) NOT NULL,
    Asiento NVARCHAR(10) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    EstadoBoleto NVARCHAR(20) NOT NULL DEFAULT 'Vendido',
    CONSTRAINT FK_Boletos_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    CONSTRAINT FK_Boletos_Partidos FOREIGN KEY (IdPartido) REFERENCES Partidos(IdPartido),
    CONSTRAINT FK_Boletos_Zonas FOREIGN KEY (IdZona) REFERENCES Zonas(IdZona),
    CONSTRAINT CHK_EstadoBoleto CHECK (EstadoBoleto IN ('Vendido', 'Cancelado')),
    -- Restricción CRÍTICA: Un asiento en un partido específico no se puede vender dos veces
    CONSTRAINT UQ_Asiento_Partido UNIQUE (IdPartido, IdZona, Fila, Asiento)
);
GO

-- =============================================
-- 9. TABLA DE FACTURAS
-- =============================================
CREATE TABLE Facturas (
    IdFactura INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL UNIQUE, -- Relación 1 a 1 con la venta
    FolioFactura NVARCHAR(50) NOT NULL UNIQUE,
    FechaHoraEmision DATETIME NOT NULL DEFAULT GETDATE(),
    -- Datos del Emisor
    EmisorNombre NVARCHAR(150) NOT NULL,
    EmisorRFC NVARCHAR(13) NOT NULL,
    EmisorDomicilio NVARCHAR(250) NOT NULL,
    -- Datos del Receptor
    ReceptorNombre NVARCHAR(150) NOT NULL,
    ReceptorRFC NVARCHAR(13) NOT NULL,
    ReceptorDomicilio NVARCHAR(250) NOT NULL,
    UsoCFDI NVARCHAR(100) NOT NULL,
    -- Datos del Comprobante
    MetodoPago NVARCHAR(20) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    IVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Facturas_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta)
);
GO

-- =============================================
-- 10. TABLA DE CANCELACIONES
-- =============================================
CREATE TABLE Cancelaciones (
    IdCancelacion INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL UNIQUE, -- Una venta solo se cancela una vez
    IdUsuario INT NOT NULL,
    FechaHoraCancelacion DATETIME NOT NULL DEFAULT GETDATE(),
    Motivo NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_Cancelaciones_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    CONSTRAINT FK_Cancelaciones_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO


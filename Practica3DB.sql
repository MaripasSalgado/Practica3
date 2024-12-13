USE [master]
GO

CREATE DATABASE [PracticaS12]
GO

USE [PracticaS12]
GO

CREATE TABLE [dbo].[Abonos](
	[Id_Compra] [bigint] NOT NULL,
	[Id_Abono] [bigint] IDENTITY(1,1) NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[Fecha] [datetime] NOT NULL,
 CONSTRAINT [PK_Abonos] PRIMARY KEY CLUSTERED 
(
	[Id_Abono] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Principal](
	[Id_Compra] [bigint] IDENTITY(1,1) NOT NULL,
	[Precio] [decimal](18, 5) NOT NULL,
	[Saldo] [decimal](18, 5) NOT NULL,
	[Descripcion] [varchar](500) NOT NULL,
	[Estado] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Principal] PRIMARY KEY CLUSTERED 
(
	[Id_Compra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[Principal] ON 
GO
INSERT [dbo].[Principal] ([Id_Compra], [Precio], [Saldo], [Descripcion], [Estado]) VALUES (1, CAST(50000.00000 AS Decimal(18, 5)), CAST(50000.00000 AS Decimal(18, 5)), N'Producto 1', N'Pendiente')
GO
INSERT [dbo].[Principal] ([Id_Compra], [Precio], [Saldo], [Descripcion], [Estado]) VALUES (2, CAST(13500.00000 AS Decimal(18, 5)), CAST(13500.00000 AS Decimal(18, 5)), N'Producto 2', N'Pendiente')
GO
INSERT [dbo].[Principal] ([Id_Compra], [Precio], [Saldo], [Descripcion], [Estado]) VALUES (3, CAST(83600.00000 AS Decimal(18, 5)), CAST(83600.00000 AS Decimal(18, 5)), N'Producto 3', N'Pendiente')
GO
INSERT [dbo].[Principal] ([Id_Compra], [Precio], [Saldo], [Descripcion], [Estado]) VALUES (4, CAST(1220.00000 AS Decimal(18, 5)), CAST(1220.00000 AS Decimal(18, 5)), N'Producto 4', N'Pendiente')
GO
INSERT [dbo].[Principal] ([Id_Compra], [Precio], [Saldo], [Descripcion], [Estado]) VALUES (5, CAST(480.00000 AS Decimal(18, 5)), CAST(480.00000 AS Decimal(18, 5)), N'Producto 5', N'Pendiente')
GO
SET IDENTITY_INSERT [dbo].[Principal] OFF
GO

ALTER TABLE [dbo].[Abonos]  WITH CHECK ADD  CONSTRAINT [FK_Abonos_Principal] FOREIGN KEY([Id_Compra])
REFERENCES [dbo].[Principal] ([Id_Compra])
GO
ALTER TABLE [dbo].[Abonos] CHECK CONSTRAINT [FK_Abonos_Principal]
GO

-- Obtener todas las compras
Alter PROCEDURE sp_ObtenerCompras
AS
BEGIN
    SELECT * 
    FROM Principal
    ORDER BY 
        CASE 
            WHEN Estado = 'Pendiente' THEN 0
            ELSE 1
        END,
        Estado, -- Orden adicional para otros estados
        Id_Compra;     -- Ordenar por ID dentro de cada grupo
END;
GO

-- Obtener compras pendientes
CREATE PROCEDURE sp_ObtenerComprasPendientes
AS
BEGIN
    SELECT Id_Compra, Descripcion, Saldo FROM Principal WHERE Estado = 'Pendiente';
END;
GO

-- Registrar un abono

Create PROCEDURE sp_RegistrarAbono
    @IdCompra INT,
    @Monto DECIMAL(10,2),
	@Fecha DATETIME
AS
BEGIN
    DECLARE @SaldoActual DECIMAL(10,2);

    -- Obtener el saldo actual de la compra
    SELECT @SaldoActual = Saldo
    FROM Principal
    WHERE Id_Compra = @IdCompra;

    -- Validar que el abono no sea mayor al saldo actual
    IF @Monto > @SaldoActual
    BEGIN
        RAISERROR ('El abono no puede ser mayor al saldo anterior.', 16, 1);
        RETURN;
    END

    -- Registrar el abono
    INSERT INTO Abonos (Id_Compra, Monto, Fecha)
    VALUES (@IdCompra, @Monto,@Fecha);

    -- Actualizar el saldo en la tabla Principal
    UPDATE Principal
    SET Saldo = Saldo - @Monto,
        Estado = CASE WHEN Saldo - @Monto = 0 THEN 'Cancelado' ELSE Estado END
    WHERE Id_Compra = @IdCompra;
END;
GO

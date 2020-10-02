USE [VuelosNewShore]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Anderson>
-- Create date: <Create Date,10/01/2020>
-- Description:	<Description,,[InsertVuelo]>
-- =============================================
CREATE PROCEDURE [dbo].[InsertVuelo] 
	-- Add the parameters for the stored procedure here
@VloId as int output,	
@VloFechaVuelo as datetime = NULL,
@VloOrigen as varchar (50) = NULL,
@VloDestino as varchar(50) = NULL,
@VloNumeroVuelo as varchar(50) = NULL,
@VloPrecio as  decimal(30, 2) = NULL,
@VloMoneda as varchar(50) = NULL

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

DECLARE @Id int 
SELECT @Id = MAX([VloId]) FROM [Vuelo]
BEGIN TRY
BEGIN TRANSACTION

INSERT INTO [dbo].[Vuelo]
(VloFechaVuelo ,VloOrigen ,VloDestino ,VloNumeroVuelo ,VloPrecio ,VloMoneda)
VALUES
(@VloFechaVuelo ,@VloOrigen ,@VloDestino ,@VloNumeroVuelo ,@VloPrecio ,@VloMoneda ) 

SELECT @@IDENTITY
SET @VloId = @@IDENTITY

COMMIT TRANSACTION
END TRY
BEGIN CATCH

	IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION
	DBCC CHECKIDENT([Vuelo], RESEED, @Id)
    PRINT 'FAILURE: Record was not inserted.';
    PRINT 'Error ' + CONVERT(VARCHAR, ERROR_NUMBER(), 1) + ': '+ ERROR_MESSAGE()

END CATCH

END




CREATE TABLE [dbo].[Vuelo](
	[VloId] [Int] IDENTITY(1,1) NOT NULL,	
	[VloFechaVuelo] [datetime] NOT NULL,
	[VloOrigen] [varchar](50) NULL,
	[VloDestino] [varchar](50) NOT NULL,
	[VloNumeroVuelo] [varchar](50) NOT NULL,
	[VloPrecio]  [decimal](30, 2) NOT NULL,
	[VloMoneda] [varchar](50) NOT NULL,
	
	) ON [PRIMARY]

ALTER TABLE [Vuelo] ADD PRIMARY KEY CLUSTERED ([VloId]);
ALTER TABLE [Vuelo] ADD CONSTRAINT [PK_Vuelo_VloId] UNIQUE NONCLUSTERED ([VloId] ASC);




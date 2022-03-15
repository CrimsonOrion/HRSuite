CREATE TABLE [dbo].[Codes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](100) NULL,
 CONSTRAINT [PK_Codes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
CONSTRAINT [UQ_Type_Code] UNIQUE NONCLUSTERED 
(
	[Type] ASC,
	[Code] ASC
))
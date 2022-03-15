CREATE TABLE [dbo].[JobCodes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL UNIQUE,
	[EEOClass] [nvarchar](10) NOT NULL,
	[Department] [nvarchar](10) NOT NULL,
	[JobTitle] [nvarchar](100) NOT NULL,
	[InsideOutside] [nvarchar](10) NULL,
	[SupervisorCode] [nvarchar](10) NOT NULL,
	[RequisitionType] [nvarchar](10) NOT NULL,
	[Exempt] [bit] NOT NULL,
 CONSTRAINT [PK_JobCodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))
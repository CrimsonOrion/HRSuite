CREATE TABLE [dbo].[Requisitions](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL UNIQUE,
	[Description] [nvarchar](100) NULL,
	[Status] [nvarchar](10) NOT NULL,
	[JobCode] [nvarchar](10) NOT NULL,
	[Internal] [bit] NOT NULL,
	[NumPos] [int] NOT NULL,
	[ReasonOpen] [ntext] NULL,
	[ReasonClosed] [ntext] NULL,
	[CreateDate] [date] NOT NULL,
	[LastChangeDate] [datetime2](0) NOT NULL,
	[OpenDate] [date] NULL,
	[CloseDate] [date] NULL,
	[Comment] [ntext] NULL,
 CONSTRAINT [PK_Requisitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
), 
CONSTRAINT [FK_Requisitions_JobCodes] FOREIGN KEY ([JobCode]) REFERENCES [JobCodes]([Code]) ON UPDATE CASCADE ON DELETE CASCADE
)
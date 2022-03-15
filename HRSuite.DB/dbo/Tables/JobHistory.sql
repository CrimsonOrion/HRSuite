CREATE TABLE [dbo].[JobHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[JobCode] [nvarchar](10) NOT NULL,
	[ChangeDate] [date] NOT NULL,
	[JobDate] [date] NOT NULL,
	[Reason] [nvarchar](15) NULL,
 CONSTRAINT [PK_JobHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
CONSTRAINT [FK_JobHistory_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
CONSTRAINT [FK_JobHistory_JobCodes] FOREIGN KEY([JobCode]) REFERENCES [dbo].[JobCodes] ([Code]) ON UPDATE CASCADE ON DELETE CASCADE
)
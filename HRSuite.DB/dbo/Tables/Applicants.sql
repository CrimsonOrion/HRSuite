CREATE TABLE [dbo].[Applicants](
	[Id] [int] NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Nickname] [nvarchar](100) NULL,
	[Address1] [nvarchar](100) NOT NULL,
	[Address2] [nvarchar](100) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](10) NOT NULL,
	[Zip] [nvarchar](20) NOT NULL,
	[HomePhone] [nvarchar](20) NULL,
	[CellPhone] [nvarchar](20) NULL,
	[Sex] [nvarchar](10) NULL,
	[Ethnicity] [nvarchar](10) NULL,
	[Disabled] [nvarchar](20) NULL,
	[ProtectedVet] [nvarchar](20) NULL,
	[DisabledVet] [nvarchar](20) NULL,
	[ApplicationDate] [date] NOT NULL,
	[GeneralStatus] [nvarchar](10) NOT NULL,
	[StatusAsOf] [datetime2](3) NOT NULL,
	[Interviewed] [bit] NOT NULL,
	[LetterCode] [nvarchar](20) NULL,
	[LetterSendDate] [date] NULL,
	[AcceptDate] [date] NULL,
	[RejectionCode] [nvarchar](20) NULL,
	[Comment] [ntext] NULL,
    CONSTRAINT [PK_Applicants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
), 
    CONSTRAINT [FK_Applicants_Requisitions] FOREIGN KEY
(
	[RequisitionId]
) REFERENCES [dbo].[Requisitions] ([Id]))
CREATE PROCEDURE [dbo].[spApplicants_GetByLastName]
    @LastName nvarchar(100)
AS
BEGIN
	SET @LastName = @LastName + '%'
	
    SELECT [Id], [RequisitionId], [FirstName], [MiddleName], [LastName], [Nickname], [Address1], [Address2], [City], [State], [Zip], [HomePhone], [CellPhone], [Sex], [Ethnicity], [Disabled], [ProtectedVet], [DisabledVet], [ApplicationDate], [GeneralStatus], [StatusAsOf], [Interviewed], [LetterCode], [LetterSendDate], [AcceptDate], [RejectionCode], [Comment]
	FROM [dbo].[Applicants]
    WHERE [LastName] LIKE @LastName
	ORDER BY [LastName];
END
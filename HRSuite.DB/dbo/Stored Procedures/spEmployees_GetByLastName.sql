CREATE PROCEDURE [dbo].[spEmployees_GetByLastName]
    @LastName nvarchar(100)
AS
BEGIN
	SET @LastName = @LastName + '%'

    SELECT [Id], [FirstName], [MiddleName], [LastName], [Nickname], [Address1], [Address2], [City], [State], [Zip], [HomePhone], [CellPhone], [Sex], [Ethnicity], [Disabled], [ProtectedVet], [DisabledVet], [Birthday], [Active], [BusinessExt],[EmployeeEmail], [OriginalHireDate], [SeniorityDate], [TerminationDate], [TerminationCode], [Rehired], [LicenseNumber], [LicenseExp], [CDL_NonCDL], [CDLMedExp], [CDLDriverType], [LicenseNotes]
    FROM [Employees]
    WHERE [LastName] LIKE @LastName
	ORDER BY [LastName];
END
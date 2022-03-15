CREATE PROCEDURE [dbo].[spEmployees_GetAll]
AS
BEGIN
    SELECT [Id], [FirstName], [MiddleName], [LastName], [Nickname], [Address1], [Address2], [City], [State], [Zip], [HomePhone], [CellPhone], [Sex], [Ethnicity], [Disabled], [ProtectedVet], [DisabledVet], [Birthday], [Active], [BusinessExt],[EmployeeEmail], [OriginalHireDate], [SeniorityDate], [TerminationDate], [TerminationCode], [Rehired], [LicenseNumber], [LicenseExp], [CDL_NonCDL], [CDLMedExp], [CDLDriverType], [LicenseNotes]
    FROM [dbo].[Employees]
END
CREATE PROCEDURE [dbo].[spEmployees_GetById]
    @Id int
AS
BEGIN
    SELECT [Id], [FirstName], [MiddleName], [LastName], [Nickname], [Address1], [Address2], [City], [State], [Zip], [HomePhone], [CellPhone], [Sex], [Ethnicity], [Disabled], [ProtectedVet], [DisabledVet], [Birthday], [Active], [BusinessExt],[EmployeeEmail], [OriginalHireDate], [SeniorityDate], [TerminationDate], [TerminationCode], [Rehired], [LicenseNumber], [LicenseExp], [CDL_NonCDL], [CDLMedExp], [CDLDriverType], [LicenseNotes]
    FROM [Employees]
    WHERE @Id = [Id];
END
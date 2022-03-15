CREATE PROCEDURE [dbo].[spEmployees_GetActiveCDL]
AS
BEGIN
    SELECT [LastName],[FirstName],[MiddleName],[Birthday],[LicenseNumber],[LicenseExp],[CDL_NonCDL],[CDLMedExp],[CDLDriverType],[LicenseNotes]
    FROM [dbo].[Employees]
    WHERE [Active] = 1 AND [CDL_NonCDL] = 'CDL'
    ORDER BY [LastName], [FirstName];
END
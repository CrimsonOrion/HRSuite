CREATE PROCEDURE [dbo].[spEmployees_GetMaxId]
AS
BEGIN
    SELECT Max([Id])
    FROM [dbo].[Employees]
END
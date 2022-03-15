CREATE PROCEDURE [dbo].[spEmployees_GetNamesByActive]
AS
BEGIN
	SELECT [Id], [FirstName], [LastName]
	FROM [dbo].[Employees]
	WHERE [Active] = 1
	ORDER BY [LastName], [FirstName], [Id]
END
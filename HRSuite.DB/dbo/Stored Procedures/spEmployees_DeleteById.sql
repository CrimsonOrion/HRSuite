CREATE PROCEDURE [dbo].[spEmployees_DeleteById]
    @Id int
AS
BEGIN
    DELETE FROM [dbo].[Employees]
    WHERE [Id] = @Id;
END
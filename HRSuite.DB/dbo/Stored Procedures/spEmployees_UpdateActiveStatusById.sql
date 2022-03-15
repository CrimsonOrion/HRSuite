CREATE PROCEDURE [dbo].[spEmployees_UpdateActiveStatusById]
    @Id int,
    @Active int,
    @TerminationDate date,
    @TerminationCode nvarchar(20)
AS
BEGIN
    UPDATE [dbo].[Employees]
    SET Active = @Active, TerminationDate = @TerminationDate, TerminationCode = @TerminationCode
    WHERE Id = @Id;
END
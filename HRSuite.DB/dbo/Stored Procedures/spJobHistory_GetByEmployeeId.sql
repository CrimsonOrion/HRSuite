CREATE PROCEDURE [dbo].[spJobHistory_GetByEmployeeId]
    @EmployeeId int
AS
BEGIN
    SELECT [JobCode], [JobTitle], [ChangeDate], [JobDate], [Reason]
    FROM [dbo].[JobHistory]
    INNER JOIN [dbo].[JobCodes] ON [JobCode] = [Code]
    WHERE [EmployeeId] = @EmployeeId
    ORDER BY [JobDate]
END
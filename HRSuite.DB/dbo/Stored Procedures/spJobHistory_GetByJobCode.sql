-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retieves Job History Information by JobCode
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobHistory_GetByJobCode]
    @Code nvarchar(10)
AS
BEGIN
    SELECT [Id], [EmployeeId], [JobCode], [ChangeDate], [JobDate], [Reason]
    FROM [dbo].[JobHistory]
    WHERE [JobCode] = @Code
    ORDER BY [Id];
END
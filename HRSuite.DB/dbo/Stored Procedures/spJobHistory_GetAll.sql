-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves All Job History Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobHistory_GetAll]
AS
BEGIN
    SELECT a.[Id],a.[EmployeeId],a.[JobCode],b.[JobTitle],a.[ChangeDate],a.[JobDate],a.[Reason],b.[Department],c.[Description]
	FROM [dbo].[JobHistory] a
	INNER JOIN [dbo].[JobCodes] b ON a.[JobCode] = b.[Code]
	INNER JOIN (
		SELECT [Code],[Description]
		FROM [dbo].[Codes]
		WHERE [Type] = 'DEPT'
	) c ON b.[Department] = c.[Code]
	ORDER BY a.[JobDate] desc
END
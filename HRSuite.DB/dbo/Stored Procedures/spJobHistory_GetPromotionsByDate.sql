-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves Job History Information by JobDate
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobHistory_GetPromotionsByDate]
    @startDate date,
    @endDate date
AS
BEGIN
    SELECT [LastName],[MiddleName],[FirstName],[EEOClass],[Department],[Description],[JobTitle],[Reason],[Sex],[Ethnicity]
    FROM [dbo].[JobHistory] jh
    INNER JOIN [dbo].[Employees] e ON jh.EmployeeId = e.Id
    INNER JOIN [dbo].[JobCodes] jc ON jc.Code = jh.JobCode
    INNER JOIN (
		SELECT [Code],[Description]
		FROM [dbo].[Codes]
		WHERE [Type] = 'DEPT'
	) c ON jc.[Department] = c.[Code]
    WHERE [Reason] in ('LINE PROGRESS', 'TRANSFER', 'PROMOTION', 'DEMOTION') AND [JobDate] BETWEEN @startDate AND @endDate
    ORDER BY JobDate desc
END
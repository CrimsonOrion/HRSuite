CREATE PROCEDURE [dbo].[spEmployees_GetNewHireInfoByDate]
	@startDate date,
	@endDate date
AS
BEGIN
	SELECT FirstName, MiddleName, LastName, JobTitle, Department, EEOClass, SeniorityDate, Disabled, DisabledVet, ProtectedVet, Sex, Ethnicity
	FROM [dbo].[Employees] e
	INNER JOIN [dbo].[JobHistory] jh ON e.Id = jh.EmployeeId
	INNER JOIN [dbo].[JobCodes] jc ON jh.JobCode = jc.Code
	WHERE e.SeniorityDate BETWEEN @startDate AND @endDate AND jh.Reason = 'NEW HIRE'
END
GO
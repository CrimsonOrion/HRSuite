CREATE PROCEDURE [dbo].[spEmployees_SearchByLastName]
    @LastName nvarchar(100)
AS
BEGIN
	SET @LastName = @LastName + '%'

    SELECT e.Id, e.FirstName, e.MiddleName, e.LastName, jh.JobCode, e.Active
	FROM [dbo].[Employees] e
	INNER JOIN (
		SELECT c.EmployeeId, c.JobCode
		FROM [dbo].[JobHistory] c
		INNER JOIN (
			SELECT MAX(a.Id) as Id, b.EmployeeId, b.ChangeDate
			FROM [dbo].[JobHistory] a
			INNER JOIN (
				SELECT EmployeeId,MAX(ChangeDate) as ChangeDate
				FROM [dbo].[JobHistory]
				GROUP BY EmployeeId
			) b ON a.EmployeeId = b.EmployeeId AND a.ChangeDate = b.ChangeDate
			GROUP BY b.EmployeeId, b.ChangeDate
		) d ON c.Id = d.Id
	) jh ON e.Id = jh.EmployeeId
	WHERE e.LastName LIKE @LastName
END
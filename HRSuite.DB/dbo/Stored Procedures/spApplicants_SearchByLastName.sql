CREATE PROCEDURE [dbo].[spApplicants_SearchByLastName]
    @LastName nvarchar(100)
AS
BEGIN
	SET @LastName = @LastName + '%'
	
    SELECT a.Id, r.Code, a.FirstName, a.MiddleName, a.LastName
    FROM [dbo].[Applicants] a
    INNER JOIN [dbo].[Requisitions] r ON a.RequisitionId = r.Id
    WHERE a.LastName LIKE @LastName
    ORDER BY a.LastName;
END
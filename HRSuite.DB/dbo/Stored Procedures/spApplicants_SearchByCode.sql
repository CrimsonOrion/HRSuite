CREATE PROCEDURE [dbo].[spApplicants_SearchByCode]
    @Code nvarchar(20)
AS
BEGIN
    SET @Code = '%' + @Code + '%'

    SELECT a.Id, r.Code, a.FirstName, a.MiddleName, a.LastName
    FROM [dbo].[Applicants] a
    INNER JOIN [dbo].[Requisitions] r ON a.RequisitionId = r.Id
    WHERE r.Code LIKE @Code
    ORDER BY r.Code;
END
CREATE PROCEDURE [dbo].[spApplicants_GetByRequisitionId]
    @RequisitionId int
AS
BEGIN
    SELECT a.Id as ApplicantId, a.RequisitionId, a.FirstName, a.MiddleName, a.LastName, r.Code, a.Sex, a.Ethnicity, a.ProtectedVet, a.DisabledVet, a.Interviewed, a.GeneralStatus, a.ApplicationDate
    FROM [dbo].[Applicants] a
    INNER JOIN [dbo].[Requisitions] r ON r.Id = a.RequisitionId
    WHERE a.RequisitionId = @RequisitionId;
END
CREATE PROCEDURE [dbo].[spInternalApplicants_GetByApplicantId]
    @ApplicantId int
AS
BEGIN
    SELECT [Id], [EmployeeId], [ApplicantId]
    FROM [dbo].[InternalApplicants]
    WHERE [ApplicantId] = @ApplicantId;
END
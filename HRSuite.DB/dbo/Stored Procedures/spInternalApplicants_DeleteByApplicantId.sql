CREATE PROCEDURE [dbo].[spInternalApplicants_DeleteByApplicantId]
    @ApplicantId int
AS
BEGIN
    DELETE FROM [dbo].[InternalApplicants]
    WHERE [ApplicantId] = @ApplicantId;
END
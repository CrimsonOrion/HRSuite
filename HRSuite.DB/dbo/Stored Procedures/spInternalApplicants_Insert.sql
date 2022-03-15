-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Inserts Internal Applicant Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spInternalApplicants_Insert]
    @EmployeeId int,
	@ApplicantId int,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.InternalApplicants(EmployeeId, ApplicantId)
	VALUES (@EmployeeId, @ApplicantId);

	SELECT @Id = SCOPE_IDENTITY();
END
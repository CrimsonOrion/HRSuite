-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Inserts Job Code Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobCodes_Insert] 
	@Code nvarchar(10),
	@EEOClass nvarchar(10),
	@Department nvarchar(10),
	@JobTitle nvarchar(100),
	@InsideOutside nvarchar(10),
	@SupervisorCode nvarchar(10),
	@RequisitionType nvarchar(10),
	@Exempt bit,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.JobCodes(Code, EEOClass, Department, JobTitle, InsideOutside, SupervisorCode, RequisitionType, Exempt)
	VALUES (@Code, @EEOClass, @Department, @JobTitle, @InsideOutside, @SupervisorCode, @RequisitionType, @Exempt);

	SELECT @Id = SCOPE_IDENTITY();
END
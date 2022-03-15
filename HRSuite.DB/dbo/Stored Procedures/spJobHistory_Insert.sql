-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Inserts Job History Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobHistory_Insert] 
	@EmployeeId int,
	@JobCode nvarchar(10),
	@ChangeDate date,
	@JobDate date,
	@Reason nvarchar(15),
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.JobHistory(EmployeeId, JobCode, ChangeDate, JobDate, Reason)
	VALUES (@EmployeeId, @JobCode, @ChangeDate, @JobDate, @Reason);

	SELECT @Id = SCOPE_IDENTITY();
END
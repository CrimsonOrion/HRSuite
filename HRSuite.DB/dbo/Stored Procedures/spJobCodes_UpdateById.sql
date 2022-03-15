-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Updates Job Code Information by Job Code Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobCodes_UpdateById] 
	@Id int,
	@Code nvarchar(10),
	@EEOClass nvarchar(10),
	@Department nvarchar(10),
	@JobTitle nvarchar(100),
	@InsideOutside nvarchar(10),
	@SupervisorCode nvarchar(10),
	@RequisitionType nvarchar(10),
	@Exempt bit
AS
BEGIN
	UPDATE [dbo].[JobCodes]
	SET [Code] = @Code, [EEOClass] = @EEOClass, [Department] = @Department, [JobTitle] = @JobTitle, [InsideOutside] = @InsideOutside, [SupervisorCode] = @SupervisorCode, [RequisitionType] = @RequisitionType, [Exempt] = @Exempt
	WHERE [Id] = @Id;
END
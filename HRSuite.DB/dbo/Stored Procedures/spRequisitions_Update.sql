-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Updates Requisition Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_Update] 
	@Id int,
	@Code nvarchar(20),
	@Description nvarchar(100),
	@Status nvarchar(10),
	@JobCode nvarchar(10),
	@Internal bit,
	@NumPos int,
	@ReasonOpen ntext,
	@ReasonClosed ntext,
	@CreateDate date,
	@LastChangeDate datetime2(0),
	@OpenDate date,
	@CloseDate date,
	@Comment ntext
AS
BEGIN
	UPDATE dbo.Requisitions
	SET 
		Code = @Code,
		Description = @Description,
		Status = @Status,
		JobCode = @JobCode,
		Internal = @Internal,
		NumPos = @NumPos,
		ReasonOpen = @ReasonOpen,
		ReasonClosed = @ReasonClosed,
		CreateDate = @CreateDate,
		LastChangeDate = @LastChangeDate,
		OpenDate = @OpenDate,
		CloseDate = @CloseDate,
		Comment = @Comment
	WHERE Id = @Id;
END
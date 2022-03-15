-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Inserts Requisition Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_Insert] 
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
	INSERT INTO dbo.Requisitions(Id, Code, Description, Status, JobCode, Internal, NumPos, ReasonOpen, ReasonClosed, CreateDate, LastChangeDate, OpenDate, CloseDate, Comment)
	VALUES (@Id, @Code, @Description, @Status, @JobCode, @Internal, @NumPos, @ReasonOpen, @ReasonClosed, @CreateDate, @LastChangeDate, @OpenDate, @CloseDate, @Comment);
END
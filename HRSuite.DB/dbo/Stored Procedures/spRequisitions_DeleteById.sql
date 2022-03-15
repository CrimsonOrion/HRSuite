-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        01/26/2022
-- Description: Deletes Requisition
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_DeleteById] 
	@Id int
AS
BEGIN
	DELETE FROM dbo.Requisitions
	WHERE Id = @Id;
END
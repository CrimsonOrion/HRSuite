-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves Requisition Information by Requisition Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_GetById]
    @Id int
AS
BEGIN
    SELECT [Id],[Code],[Description],[Status],[JobCode],[Internal],[NumPos],[ReasonOpen],[ReasonClosed],[CreateDate],[LastChangeDate],[OpenDate],[CloseDate],[Comment]
    FROM [dbo].[Requisitions]
    WHERE Id = @Id;
END
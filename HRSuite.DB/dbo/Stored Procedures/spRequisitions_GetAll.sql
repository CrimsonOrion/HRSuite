-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retieves All Requisition Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_GetAll]
AS
BEGIN
SELECT [Id],[Code],[Description],[Status],[JobCode],[Internal],[NumPos],[ReasonOpen],[ReasonClosed],[CreateDate],[LastChangeDate],[OpenDate],[CloseDate],[Comment]
  FROM [dbo].[Requisitions]
  ORDER BY [Id];
END
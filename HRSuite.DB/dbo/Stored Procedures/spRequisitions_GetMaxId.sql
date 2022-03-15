-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves Max Id from Requisitions
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_GetMaxId]
AS
BEGIN
    SELECT Max([Id])
    FROM [dbo].[Requisitions]
END
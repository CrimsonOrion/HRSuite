-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Searches Requisitions by Requisition Code
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spRequisitions_SearchByCode]
    @Code nvarchar(20)
AS
BEGIN
    SET @Code = '%' + @Code + '%'

    SELECT Id, Code, Description, Status
    FROM [dbo].[Requisitions]
    WHERE Code LIKE @Code
    ORDER BY Code;
END
-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retieves All Job Code Information
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobCodes_GetAll]
AS
BEGIN
    SELECT [Id],[Code],[EEOClass],[Department],[JobTitle],[InsideOutside],[SupervisorCode],[RequisitionType],[Exempt]
    FROM [dbo].[JobCodes]
    ORDER BY [Code];
END
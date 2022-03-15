-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves Job Code Information by Job Code Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobCodes_GetById]
    @Id int
AS
BEGIN
    SELECT [Id],[Code],[EEOClass],[Department],[JobTitle],[InsideOutside],[SupervisorCode],[RequisitionType],[Exempt]
    FROM [dbo].[JobCodes]
    WHERE [Id] = @Id;
END
-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Deletes Job Code Information by Job Code Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobCodes_DeleteById]
    @Id int
AS
BEGIN
    DELETE
    FROM [dbo].[JobCodes]
    WHERE [Id] = @Id;
END
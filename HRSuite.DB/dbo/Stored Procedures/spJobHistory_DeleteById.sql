-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Deletes Job History Information by Job History Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spJobHistory_DeleteById]
    @Id int
AS
BEGIN
    DELETE
    FROM [dbo].[JobHistory]
    WHERE [Id] = @Id
END
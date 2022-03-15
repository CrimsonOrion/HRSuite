-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Deletes Applicant by Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spApplicants_DeleteById]
    @Id int
AS
BEGIN
    DELETE FROM [dbo].[Applicants]
    WHERE [Id] = @Id;
END
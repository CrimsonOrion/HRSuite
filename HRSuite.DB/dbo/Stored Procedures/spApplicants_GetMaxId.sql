CREATE PROCEDURE [dbo].[spApplicants_GetMaxId]
AS
BEGIN
    SELECT Max([Id])
    FROM [dbo].[Applicants]
END
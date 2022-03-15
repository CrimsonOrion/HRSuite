CREATE PROCEDURE [dbo].[spCodes_GetTypes]
AS
BEGIN
    SELECT DISTINCT [Type]
    FROM [dbo].[Codes]
END
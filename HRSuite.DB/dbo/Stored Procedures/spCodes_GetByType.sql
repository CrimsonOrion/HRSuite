CREATE PROCEDURE [dbo].[spCodes_GetByType]
    @type nvarchar(20)
AS
BEGIN
    SELECT Type, Code, Description
    FROM Codes
    WHERE Type = @type;
END
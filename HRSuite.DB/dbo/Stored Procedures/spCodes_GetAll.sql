CREATE PROCEDURE [dbo].[spCodes_GetAll]
AS
BEGIN
    SELECT Id, Type, Code, Description
    FROM Codes;
END
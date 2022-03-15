CREATE PROCEDURE [dbo].[spCodes_DeleteById]
	@Id int
AS
BEGIN
	DELETE
	FROM dbo.Codes
	WHERE Id = @Id;
END
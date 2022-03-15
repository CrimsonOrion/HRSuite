CREATE PROCEDURE [dbo].[spCodes_Insert] 
	@Type nvarchar(20),
	@Code nvarchar(20),
	@Description nvarchar(100)
AS
BEGIN
	INSERT INTO dbo.Codes(Type, Code, Description)
	VALUES (@Type, @Code, @Description);
END
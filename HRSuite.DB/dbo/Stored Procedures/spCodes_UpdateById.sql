CREATE PROCEDURE [dbo].[spCodes_UpdateById]
	@Id int,
    @Type nvarchar(20),
	@Code nvarchar(20),
	@Description nvarchar(100)
AS
BEGIN
	UPDATE dbo.Codes
	SET Type = @Type, Code = @Code, Description = @Description
	WHERE Id = @Id;
END
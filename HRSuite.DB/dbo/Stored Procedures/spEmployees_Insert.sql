CREATE PROCEDURE [dbo].[spEmployees_Insert] 
    @Id int,
	@FirstName nvarchar(100),
	@MiddleName nvarchar(100),
	@LastName nvarchar(100),
	@Nickname nvarchar(100),
	@Address1 nvarchar(100),
	@Address2 nvarchar(100),
	@City nvarchar(100),
	@State nvarchar(10),
	@Zip nvarchar(20),
	@HomePhone nvarchar(20),
	@CellPhone nvarchar(20),
	@Sex nvarchar(10),
	@Ethnicity nvarchar(10),
	@Disabled nvarchar(20),
	@ProtectedVet nvarchar(20),
	@DisabledVet nvarchar(20),
	@Birthday date,
	@Active bit,
	@BusinessExt nvarchar(10),
	@EmployeeEmail nvarchar(100),
	@OriginalHireDate date,
	@SeniorityDate date,
	@TerminationDate date,
	@TerminationCode nvarchar(20),
	@Rehired bit,
    @LicenseNumber nvarchar(20),
    @LicenseExp date,
    @CDL_NonCDL nvarchar(10),
    @CDLMedExp date,
    @CDLDriverType nvarchar(50),
    @LicenseNotes ntext
AS
BEGIN
	INSERT INTO [dbo].[Employees] ([Id],[FirstName],[MiddleName],[LastName],[Nickname],[Address1],[Address2],[City],[State],[Zip],[HomePhone],[CellPhone],[Sex],[Ethnicity],[Disabled],[ProtectedVet],[DisabledVet],[Birthday],[Active],[BusinessExt],[EmployeeEmail],[OriginalHireDate],[SeniorityDate],[TerminationDate],[TerminationCode],[Rehired],[LicenseNumber],[LicenseExp],[CDL_NonCDL],[CDLMedExp],[CDLDriverType],[LicenseNotes])
	VALUES (@Id, @FirstName, @MiddleName, @LastName, @Nickname, @Address1, @Address2, @City, @State, @Zip, @HomePhone, @CellPhone, @Sex, @Ethnicity, @Disabled, @ProtectedVet, @DisabledVet, @Birthday, @Active, @BusinessExt, @EmployeeEmail, @OriginalHireDate, @SeniorityDate, @TerminationDate, @TerminationCode, @Rehired,@LicenseNumber,@LicenseExp,@CDL_NonCDL,@CDLMedExp,@CDLDriverType,@LicenseNotes);
END
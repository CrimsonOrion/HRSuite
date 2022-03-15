CREATE PROCEDURE [dbo].[spApplicants_UpdateById] 
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
	@ApplicationDate date,
	@GeneralStatus nvarchar(10),
	@StatusAsOf datetime2(3),
	@Interviewed bit,
	@LetterCode nvarchar(20),
	@LetterSendDate date,
	@AcceptDate date,
	@RejectionCode nvarchar(20),
	@Comment ntext
AS
BEGIN
	UPDATE [dbo].[Applicants]
	SET [FirstName] = @FirstName, [MiddleName] = @MiddleName, [LastName] = @LastName, [Nickname] = @Nickname, [Address1] = @Address1, [Address2] = @Address2, [City] = @City, [State] = @State, [Zip] = @Zip, [HomePhone] = @HomePhone, [CellPhone] = @CellPhone, [Sex] = @Sex, [Ethnicity] = @Ethnicity, [Disabled] = @Disabled, [ProtectedVet] = @ProtectedVet, [DisabledVet] = @DisabledVet, [ApplicationDate] = @ApplicationDate, [GeneralStatus] = @GeneralStatus, [StatusAsOf] = @StatusAsOf, [Interviewed] = @Interviewed, [LetterCode] = @LetterCode, [LetterSendDate] = @LetterSendDate, [AcceptDate] = @AcceptDate, [RejectionCode] = @RejectionCode, [Comment] = @Comment
	WHERE [Id] = @Id;
END
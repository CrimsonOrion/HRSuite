CREATE PROCEDURE [dbo].[spApplicants_Insert] 
	@Id int,
	@RequisitionId int,
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
	INSERT INTO [dbo].[Applicants]
           ([Id],[RequisitionId],[FirstName],[MiddleName],[LastName],[Nickname],[Address1],[Address2],[City],[State],[Zip],[HomePhone],[CellPhone],[Sex],[Ethnicity],[Disabled],[ProtectedVet],[DisabledVet],[ApplicationDate],[GeneralStatus],[StatusAsOf],[Interviewed],[LetterCode],[LetterSendDate],[AcceptDate],[RejectionCode],[Comment])
	VALUES (@Id, @RequisitionId, @FirstName, @MiddleName, @LastName, @Nickname, @Address1, @Address2, @City, @State, @Zip, @HomePhone, @CellPhone, @Sex, @Ethnicity, @Disabled, @ProtectedVet, @DisabledVet, @ApplicationDate, @GeneralStatus, @StatusAsOf, @Interviewed, @LetterCode, @LetterSendDate, @AcceptDate, @RejectionCode, @Comment);
END
CREATE PROCEDURE [dbo].[spEmployees_UpdateById]
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
    @Ethnicity nvarchar(20),
    @Disabled nvarchar(10),
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
    UPDATE [dbo].[Employees]
    SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, Nickname = @Nickname, Address1 = @Address1, Address2 = @Address2, City = @City, State = @State, Zip = @Zip, HomePhone = @HomePhone, CellPhone = @CellPhone, Sex = @Sex , Ethnicity = @Ethnicity, Disabled = @Disabled, ProtectedVet = @ProtectedVet, DisabledVet = @DisabledVet, Birthday = @Birthday, Active = @Active, BusinessExt = @BusinessExt, EmployeeEmail = @EmployeeEmail, OriginalHireDate = @OriginalHireDate, SeniorityDate = @SeniorityDate, TerminationDate = @TerminationDate, TerminationCode = @TerminationCode, Rehired = @Rehired, LicenseNumber = @LicenseNumber, LicenseExp = @LicenseExp, CDL_NonCDL = @CDL_NonCDL, CDLMedExp = @CDLMedExp, CDLDriverType = @CDLDriverType, LicenseNotes = @LicenseNotes
    WHERE Id = @Id;
END
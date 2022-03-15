-----------------------------------------------------------
-- Author:      Jim Lynch
-- Date:        11/05/2021
-- Description: Retrieves Applicant information by Applicant Id
-----------------------------------------------------------

CREATE PROCEDURE [dbo].[spApplicants_GetById]
    @Id int
AS
BEGIN
    SELECT [Id], [RequisitionId], [FirstName], [MiddleName], [LastName], [Nickname], [Address1], [Address2], [City], [State], [Zip], [HomePhone], [CellPhone], [Sex], [Ethnicity], [Disabled], [ProtectedVet], [DisabledVet], [ApplicationDate], [GeneralStatus], [StatusAsOf], [Interviewed], [LetterCode], [LetterSendDate], [AcceptDate], [RejectionCode], [Comment]
	FROM [Applicants]
    WHERE [Id] = @Id;
END
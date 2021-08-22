namespace Hasslefree.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class InitialCreate : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"Login",
				c => new
				{
					LoginId = c.Int(nullable: false, identity: true),
					PersonId = c.Int(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Email = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
					Password = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					PasswordSalt = c.String(maxLength: 32, storeType: "nvarchar"),
					Salutation = c.String(unicode: false),
					Active = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.LoginId)
				.ForeignKey("Person", t => t.PersonId, cascadeDelete: true)
				.Index(t => t.PersonId, unique: true, name: "UIX_Person_PersonId")
				.Index(t => t.Email, unique: true, name: "UIX_Login_Email");

			CreateTable(
				"Person",
				c => new
				{
					PersonId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					PersonGuid = c.Guid(nullable: false),
					Title = c.String(maxLength: 16, storeType: "nvarchar"),
					FirstName = c.String(maxLength: 32, storeType: "nvarchar"),
					Surname = c.String(maxLength: 32, storeType: "nvarchar"),
					Birthday = c.DateTime(precision: 0),
					Phone = c.String(maxLength: 16, storeType: "nvarchar"),
					Fax = c.String(maxLength: 16, storeType: "nvarchar"),
					Mobile = c.String(maxLength: 16, storeType: "nvarchar"),
					Email = c.String(nullable: false, maxLength: 64, storeType: "nvarchar"),
					GenderEnum = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
					PersonStatusEnum = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.PersonId)
				.Index(t => t.PersonGuid, unique: true, name: "UIX_Person_PersonGuid")
				.Index(t => t.Email, unique: true, name: "UIX_Person_Email");

			CreateTable(
				"SecurityGroupLogin",
				c => new
				{
					LoginId = c.Int(nullable: false),
					SecurityGroupId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.LoginId, t.SecurityGroupId })
				.ForeignKey("Login", t => t.LoginId, cascadeDelete: true)
				.ForeignKey("SecurityGroup", t => t.SecurityGroupId, cascadeDelete: true)
				.Index(t => t.LoginId)
				.Index(t => t.SecurityGroupId);

			CreateTable(
				"SecurityGroup",
				c => new
				{
					SecurityGroupId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					SecurityGroupName = c.String(nullable: false, unicode: false),
					SecurityGroupDesc = c.String(unicode: false),
					IsSystemSecurityGroup = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.SecurityGroupId);

			CreateTable(
				"Permission",
				c => new
				{
					PermissionId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					PermissionUniqueName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					PermissionDisplayName = c.String(nullable: false, unicode: false),
					PermissionDescription = c.String(unicode: false),
					PermissionGroupName = c.String(unicode: false),
				})
				.PrimaryKey(t => t.PermissionId)
				.Index(t => t.PermissionUniqueName, unique: true, name: "UIX_SecurityGroup_PermissionUniqueName");

			CreateTable(
				"Session",
				c => new
				{
					SessionId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Reference = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
					IpAddress = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
					Latitude = c.Double(nullable: false),
					Longitude = c.Double(nullable: false),
					LoginId = c.Int(),
					ExpiresOn = c.DateTime(precision: 0),
				})
				.PrimaryKey(t => t.SessionId)
				.ForeignKey("Login", t => t.LoginId)
				.Index(t => t.Reference, unique: true, name: "UIX_Session_Ref")
				.Index(t => t.LoginId);

			CreateTable(
				"LandlordAddress",
				c => new
				{
					LandlordAddressId = c.Int(nullable: false, identity: true),
					RentalLandlordId = c.Int(nullable: false),
					AddressId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.LandlordAddressId)
				.ForeignKey("Address", t => t.AddressId)
				.ForeignKey("RentalLandlord", t => t.RentalLandlordId)
				.Index(t => t.RentalLandlordId)
				.Index(t => t.AddressId);

			CreateTable(
				"Address",
				c => new
				{
					AddressId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Address1 = c.String(maxLength: 128, storeType: "nvarchar"),
					Address2 = c.String(maxLength: 128, storeType: "nvarchar"),
					Address3 = c.String(maxLength: 128, storeType: "nvarchar"),
					Town = c.String(maxLength: 64, storeType: "nvarchar"),
					Code = c.String(maxLength: 24, storeType: "nvarchar"),
					Country = c.String(maxLength: 64, storeType: "nvarchar"),
					RegionName = c.String(maxLength: 32, storeType: "nvarchar"),
					Deleted = c.Boolean(nullable: false),
					Latitude = c.String(maxLength: 24, storeType: "nvarchar"),
					Longitude = c.String(maxLength: 24, storeType: "nvarchar"),
					TypeEnum = c.String(maxLength: 16, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.AddressId);

			CreateTable(
				"RentalLandlord",
				c => new
				{
					RentalLandlordId = c.Int(nullable: false, identity: true),
					UniqueId = c.Guid(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Tempdata = c.String(maxLength: 1000, storeType: "nvarchar"),
					IdNumber = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
					PersonId = c.Int(),
					RentalId = c.Int(nullable: false),
					VatNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					IncomeTaxNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					PhysicalAddressId = c.Int(),
					PostalAddressId = c.Int(),
					SignatureId = c.Int(),
					InitialsId = c.Int(),
				})
				.PrimaryKey(t => t.RentalLandlordId)
				.ForeignKey("Picture", t => t.InitialsId)
				.ForeignKey("Person", t => t.PersonId)
				.ForeignKey("Address", t => t.PhysicalAddressId)
				.ForeignKey("Address", t => t.PostalAddressId)
				.ForeignKey("Rental", t => t.RentalId)
				.ForeignKey("Picture", t => t.SignatureId)
				.Index(t => t.PersonId)
				.Index(t => t.RentalId)
				.Index(t => t.PhysicalAddressId)
				.Index(t => t.PostalAddressId)
				.Index(t => t.SignatureId)
				.Index(t => t.InitialsId);

			CreateTable(
				"Picture",
				c => new
				{
					PictureId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					MimeType = c.String(maxLength: 50, storeType: "nvarchar"),
					FormatEnum = c.String(nullable: false, maxLength: 32, storeType: "nvarchar"),
					Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					Folder = c.String(maxLength: 255, storeType: "nvarchar"),
					Path = c.String(maxLength: 255, storeType: "nvarchar"),
					AltText = c.String(maxLength: 255, storeType: "nvarchar"),
					Transforms = c.String(maxLength: 255, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.PictureId)
				.Index(t => t.Path, unique: true, name: "UIX_Picture_Path");

			CreateTable(
				"Category",
				c => new
				{
					CategoryId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					NestedLevel = c.Int(nullable: false),
					DisplayOrder = c.Int(nullable: false),
					Path = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
					Description = c.String(unicode: false),
					Hidden = c.Boolean(nullable: false),
					Tag = c.String(maxLength: 1024, storeType: "nvarchar"),
					ParentCategoryId = c.Int(),
					PictureId = c.Int(),
					SeoId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.CategoryId)
				.ForeignKey("Category", t => t.ParentCategoryId)
				.ForeignKey("Picture", t => t.PictureId)
				.Index(t => new { t.NestedLevel, t.DisplayOrder }, name: "IX_Category_Nesting")
				.Index(t => t.Path, unique: true, name: "UIX_Category_Path")
				.Index(t => t.Name, name: "IX_Category_Name")
				.Index(t => t.ParentCategoryId)
				.Index(t => t.PictureId);

			CreateTable(
				"Rental",
				c => new
				{
					RentalId = c.Int(nullable: false, identity: true),
					UniqueId = c.Guid(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					AgentId = c.Int(nullable: false),
					RentalTypeEnum = c.String(nullable: false, maxLength: 55, storeType: "nvarchar"),
					LeaseTypeEnum = c.String(nullable: false, maxLength: 55, storeType: "nvarchar"),
					RentalStatusEnum = c.String(nullable: false, maxLength: 55, storeType: "nvarchar"),
					Premises = c.String(maxLength: 255, storeType: "nvarchar"),
					StandErf = c.String(maxLength: 255, storeType: "nvarchar"),
					Township = c.String(maxLength: 255, storeType: "nvarchar"),
					Address = c.String(maxLength: 255, storeType: "nvarchar"),
					MonthlyRental = c.Decimal(precision: 15, scale: 5),
					Deposit = c.Decimal(precision: 15, scale: 5),
					MonthlyPaymentDate = c.DateTime(precision: 0),
					DepositPaymentDate = c.DateTime(precision: 0),
					Marketing = c.Boolean(nullable: false),
					Procurement = c.Boolean(nullable: false),
					Management = c.Boolean(nullable: false),
					SpecificRequirements = c.String(maxLength: 2500, storeType: "nvarchar"),
					SpecialConditions = c.String(maxLength: 2500, storeType: "nvarchar"),
					Negotiating = c.Boolean(nullable: false),
					Informing = c.Boolean(nullable: false),
					IncomingSnaglist = c.Boolean(nullable: false),
					OutgoingSnaglist = c.Boolean(nullable: false),
					Explaining = c.Boolean(nullable: false),
					PayingLandlord = c.Boolean(nullable: false),
					ContactLandlord = c.Boolean(nullable: false),
					ProvideLandlord = c.Boolean(nullable: false),
					AskLandlordConsent = c.Boolean(nullable: false),
					ProcureDepositLandlord = c.Boolean(nullable: false),
					ProcureDepositPreviousRentalAgent = c.Boolean(nullable: false),
					ProcureDepositOther = c.String(maxLength: 255, storeType: "nvarchar"),
					TransferDeposit = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.RentalId)
				.ForeignKey("Agent", t => t.AgentId)
				.Index(t => t.AgentId);

			CreateTable(
				"Agent",
				c => new
				{
					AgentId = c.Int(nullable: false, identity: true),
					AgentGuid = c.Guid(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					AgentTypeEnum = c.String(nullable: false, maxLength: 32, storeType: "nvarchar"),
					AgentStatusEnum = c.String(nullable: false, maxLength: 32, storeType: "nvarchar"),
					IdNumber = c.String(maxLength: 32, storeType: "nvarchar"),
					PersonId = c.Int(),
					Nationality = c.String(maxLength: 55, storeType: "nvarchar"),
					Race = c.String(maxLength: 55, storeType: "nvarchar"),
					PreviousEmployer = c.String(maxLength: 128, storeType: "nvarchar"),
					VatNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					Ffc = c.Boolean(nullable: false),
					FfcNumber = c.String(maxLength: 55, storeType: "nvarchar"),
					FfcIssueDate = c.DateTime(precision: 0),
					EaabReference = c.String(maxLength: 55, storeType: "nvarchar"),
					Dismissed = c.Boolean(nullable: false),
					Convicted = c.Boolean(nullable: false),
					Insolvent = c.Boolean(nullable: false),
					Withdrawn = c.Boolean(nullable: false),
					SignatureId = c.Int(),
					InitialsId = c.Int(),
					EaabProofOfPaymentId = c.Int(),
					TempData = c.String(maxLength: 255, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.AgentId)
				.ForeignKey("Download", t => t.EaabProofOfPaymentId)
				.ForeignKey("Picture", t => t.InitialsId)
				.ForeignKey("Person", t => t.PersonId)
				.ForeignKey("Picture", t => t.SignatureId)
				.Index(t => t.PersonId)
				.Index(t => t.SignatureId)
				.Index(t => t.InitialsId)
				.Index(t => t.EaabProofOfPaymentId);

			CreateTable(
				"Download",
				c => new
				{
					DownloadId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					FileName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					Extension = c.String(nullable: false, maxLength: 8, storeType: "nvarchar"),
					Binary = c.Binary(),
					RelativeFolderPath = c.String(maxLength: 255, storeType: "nvarchar"),
					ContentType = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					Size = c.Long(nullable: false),
					DownloadTypeEnum = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
					MediaStorageEnum = c.String(nullable: false, maxLength: 16, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.DownloadId);

			CreateTable(
				"LandlordBankAccount",
				c => new
				{
					LandlordBankAccountId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalId = c.Int(nullable: false),
					AccountHolder = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					Bank = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					Branch = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					BranchCode = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
					AccountNumber = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
					BankReference = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.LandlordBankAccountId)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId);

			CreateTable(
				"LandlordDocumentation",
				c => new
				{
					LandlordDocumentationId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalLandlordId = c.Int(nullable: false),
					DownloadId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.LandlordDocumentationId)
				.ForeignKey("Download", t => t.DownloadId)
				.ForeignKey("RentalLandlord", t => t.RentalLandlordId)
				.Index(t => t.RentalLandlordId)
				.Index(t => t.DownloadId);

			CreateTable(
				"RentalFica",
				c => new
				{
					RentalFicaId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalId = c.Int(nullable: false),
					RegisteredBusinessName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					RegistrationNumber = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					CompanyTypeEnum = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					CompanyType = c.Int(nullable: false),
					RegisteredAddressId = c.Int(),
					HeadOfficeAddressId = c.Int(),
					BranchAddressId = c.Int(),
					TradeName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					Phone = c.String(maxLength: 30, storeType: "nvarchar"),
					Work = c.String(maxLength: 30, storeType: "nvarchar"),
					Fax = c.String(maxLength: 30, storeType: "nvarchar"),
					Mobile = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
					Email = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					Partner1Name = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner1Surname = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner1IdNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner1Nationality = c.String(maxLength: 55, storeType: "nvarchar"),
					Partner1AddressId = c.Int(),
					Partner1Phone = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner1Work = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner1Fax = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner1Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner1Email = c.String(maxLength: 100, storeType: "nvarchar"),
					Partner2Name = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner2Surname = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner2IdNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner2Nationality = c.String(maxLength: 55, storeType: "nvarchar"),
					Partner2AddressId = c.Int(),
					Partner2Phone = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner2Work = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner2Fax = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner2Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner2Email = c.String(maxLength: 100, storeType: "nvarchar"),
					Partner3Name = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner3Surname = c.String(maxLength: 50, storeType: "nvarchar"),
					Partner3IdNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner3Nationality = c.String(maxLength: 55, storeType: "nvarchar"),
					Partner3AddressId = c.Int(),
					Partner3Phone = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner3Work = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner3Fax = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner3Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					Partner3Email = c.String(maxLength: 100, storeType: "nvarchar"),
					StaffMember = c.String(maxLength: 100, storeType: "nvarchar"),
					TransactionType = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalFicaId)
				.ForeignKey("Address", t => t.BranchAddressId)
				.ForeignKey("Address", t => t.HeadOfficeAddressId)
				.ForeignKey("Address", t => t.Partner1AddressId)
				.ForeignKey("Address", t => t.Partner2AddressId)
				.ForeignKey("Address", t => t.Partner3AddressId)
				.ForeignKey("Address", t => t.RegisteredAddressId)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId)
				.Index(t => t.RegisteredAddressId)
				.Index(t => t.HeadOfficeAddressId)
				.Index(t => t.BranchAddressId)
				.Index(t => t.Partner1AddressId)
				.Index(t => t.Partner2AddressId)
				.Index(t => t.Partner3AddressId);

			CreateTable(
				"RentalForm",
				c => new
				{
					RentalFormId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalId = c.Int(nullable: false),
					DownloadId = c.Int(nullable: false),
					RentalFormNameEnum = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalFormId)
				.ForeignKey("Download", t => t.DownloadId)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId)
				.Index(t => t.DownloadId);

			CreateTable(
				"RentalMandate",
				c => new
				{
					RentalMandateId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					RentalId = c.Int(nullable: false),
					Procurement1Percentage = c.Decimal(precision: 15, scale: 5),
					Procurement1Amount = c.Decimal(precision: 15, scale: 5),
					Procurement2Percentage = c.Decimal(precision: 15, scale: 5),
					Procurement2Amount = c.Decimal(precision: 15, scale: 5),
					Procurement3Percentage = c.Decimal(precision: 15, scale: 5),
					Procurement3Amount = c.Decimal(precision: 15, scale: 5),
					ManagementPercentage = c.Decimal(precision: 15, scale: 5),
					ManagementAmount = c.Decimal(precision: 15, scale: 5),
					SalePercentage = c.Decimal(precision: 15, scale: 5),
					SaleAmount = c.Decimal(precision: 15, scale: 5),
					AddendumProcurement = c.Decimal(precision: 15, scale: 5),
					AddendumManagement = c.Decimal(precision: 15, scale: 5),
					AddendumStartDate = c.DateTime(precision: 0),
					AddendumEndDate = c.DateTime(precision: 0),
					AddendumAmendment = c.String(maxLength: 2500, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalMandateId)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId);

			CreateTable(
				"RentalResolution",
				c => new
				{
					RentalResolutionId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalId = c.Int(nullable: false),
					HeldAt = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					HeldOn = c.DateTime(nullable: false, precision: 0),
					LeaseName = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
					AuthorizedName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					AuthorizedSurname = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalResolutionId)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId);

			CreateTable(
				"RentalResolutionMember",
				c => new
				{
					RentalResolutionMemberId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					RentalResolutionId = c.Int(nullable: false),
					Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					Surname = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					IdNumber = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
					SignatureId = c.Int(),
				})
				.PrimaryKey(t => t.RentalResolutionMemberId)
				.ForeignKey("RentalResolution", t => t.RentalResolutionId)
				.ForeignKey("Picture", t => t.SignatureId)
				.Index(t => t.RentalResolutionId)
				.Index(t => t.SignatureId);

			CreateTable(
				"RentalWitness",
				c => new
				{
					RentalWitnessId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					UniqueId = c.Guid(nullable: false),
					RentalId = c.Int(nullable: false),
					LandlordWitness1Name = c.String(maxLength: 55, storeType: "nvarchar"),
					LandlordWitness1Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					LandlordWitness1Email = c.String(maxLength: 155, storeType: "nvarchar"),
					LandlordWitness1Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					LandlordWitness2Name = c.String(maxLength: 55, storeType: "nvarchar"),
					LandlordWitness2Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					LandlordWitness2Email = c.String(maxLength: 155, storeType: "nvarchar"),
					LandlordWitness2Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					AgentWitness1Name = c.String(maxLength: 55, storeType: "nvarchar"),
					AgentWitness1Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					AgentWitness1Email = c.String(maxLength: 155, storeType: "nvarchar"),
					AgentWitness1Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					AgentWitness2Name = c.String(maxLength: 55, storeType: "nvarchar"),
					AgentWitness2Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					AgentWitness2Email = c.String(maxLength: 155, storeType: "nvarchar"),
					AgentWitness2Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					AgentWitness1Id = c.Int(),
					AgentWitness2Id = c.Int(),
					LandlordWitness1Id = c.Int(),
					LandlordWitness2Id = c.Int(),
					WitnessStatusEnum = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalWitnessId)
				.ForeignKey("Picture", t => t.AgentWitness1Id)
				.ForeignKey("Picture", t => t.AgentWitness2Id)
				.ForeignKey("Picture", t => t.LandlordWitness1Id)
				.ForeignKey("Picture", t => t.LandlordWitness2Id)
				.ForeignKey("Rental", t => t.RentalId)
				.Index(t => t.RentalId)
				.Index(t => t.AgentWitness1Id)
				.Index(t => t.AgentWitness2Id)
				.Index(t => t.LandlordWitness1Id)
				.Index(t => t.LandlordWitness2Id);

			CreateTable(
				"Content",
				c => new
				{
					ContentId = c.Int(nullable: false, identity: true),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Html = c.String(nullable: false, unicode: false),
					ContentTypeEnum = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.ContentId);

			CreateTable(
				"Country",
				c => new
				{
					CountryId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
					AllowsBilling = c.Boolean(nullable: false),
					AllowsShipping = c.Boolean(nullable: false),
					TwoLetterIsoCode = c.String(maxLength: 2, storeType: "nvarchar"),
					ThreeLetterIsoCode = c.String(maxLength: 3, storeType: "nvarchar"),
					NumericIsoCode = c.Int(nullable: false),
					SubjectToVat = c.Boolean(nullable: false),
					Published = c.Boolean(nullable: false),
					DisplayOrder = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.CountryId)
				.Index(t => t.Name, unique: true, name: "UIX_Country_Name")
				.Index(t => t.TwoLetterIsoCode, unique: true, name: "UIX_Country_TwoLetterIsoCode")
				.Index(t => t.ThreeLetterIsoCode, unique: true, name: "UIX_Country_ThreeLetterIsoCode");

			CreateTable(
				"Firm",
				c => new
				{
					FirmId = c.Int(nullable: false, identity: true),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					BusinessName = c.String(maxLength: 128, storeType: "nvarchar"),
					TradeName = c.String(maxLength: 128, storeType: "nvarchar"),
					PhysicalAddressId = c.Int(nullable: false),
					PostalAddressId = c.Int(nullable: false),
					Phone = c.String(maxLength: 32, storeType: "nvarchar"),
					Fax = c.String(maxLength: 32, storeType: "nvarchar"),
					Email = c.String(maxLength: 128, storeType: "nvarchar"),
					ReferenceNumber = c.String(maxLength: 55, storeType: "nvarchar"),
					AiNumber = c.String(maxLength: 128, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.FirmId)
				.ForeignKey("Address", t => t.PhysicalAddressId, cascadeDelete: true)
				.ForeignKey("Address", t => t.PostalAddressId, cascadeDelete: true)
				.Index(t => t.PhysicalAddressId)
				.Index(t => t.PostalAddressId);

			CreateTable(
				"Setting",
				c => new
				{
					SettingId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					Key = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
					Value = c.String(nullable: false, maxLength: 2048, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.SettingId)
				.Index(t => t.Key, name: "UIX_Setting_Key");

			CreateTable(
				"AgentAddress",
				c => new
				{
					AgentAddressId = c.Int(nullable: false, identity: true),
					AgentId = c.Int(nullable: false),
					AddressId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.AgentAddressId)
				.ForeignKey("Address", t => t.AddressId)
				.ForeignKey("Agent", t => t.AgentId)
				.Index(t => t.AgentId)
				.Index(t => t.AddressId);

			CreateTable(
				"AgentDocumentation",
				c => new
				{
					AgentDocumentationId = c.Int(nullable: false, identity: true),
					AgentId = c.Int(nullable: false),
					DownloadId = c.Int(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
				})
				.PrimaryKey(t => t.AgentDocumentationId)
				.ForeignKey("Agent", t => t.AgentId)
				.ForeignKey("Download", t => t.DownloadId)
				.Index(t => t.AgentId)
				.Index(t => t.DownloadId);

			CreateTable(
				"AgentForm",
				c => new
				{
					AgentFormId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					AgentId = c.Int(nullable: false),
					DownloadId = c.Int(nullable: false),
					FormNameEnum = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.AgentFormId)
				.ForeignKey("Agent", t => t.AgentId)
				.ForeignKey("Download", t => t.DownloadId)
				.Index(t => t.AgentId)
				.Index(t => t.DownloadId);

			CreateTable(
				"SecurityGroupPermissions",
				c => new
				{
					SecurityGroupId = c.Int(nullable: false),
					PermissionId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.SecurityGroupId, t.PermissionId })
				.ForeignKey("SecurityGroup", t => t.SecurityGroupId, cascadeDelete: true)
				.ForeignKey("Permission", t => t.PermissionId, cascadeDelete: true)
				.Index(t => t.SecurityGroupId)
				.Index(t => t.PermissionId);

		}

		public override void Down()
		{
			DropForeignKey("AgentForm", "DownloadId", "Download");
			DropForeignKey("AgentForm", "AgentId", "Agent");
			DropForeignKey("AgentDocumentation", "DownloadId", "Download");
			DropForeignKey("AgentDocumentation", "AgentId", "Agent");
			DropForeignKey("AgentAddress", "AgentId", "Agent");
			DropForeignKey("AgentAddress", "AddressId", "Address");
			DropForeignKey("Firm", "PostalAddressId", "Address");
			DropForeignKey("Firm", "PhysicalAddressId", "Address");
			DropForeignKey("RentalWitness", "RentalId", "Rental");
			DropForeignKey("RentalWitness", "LandlordWitness2Id", "Picture");
			DropForeignKey("RentalWitness", "LandlordWitness1Id", "Picture");
			DropForeignKey("RentalWitness", "AgentWitness2Id", "Picture");
			DropForeignKey("RentalWitness", "AgentWitness1Id", "Picture");
			DropForeignKey("RentalResolutionMember", "SignatureId", "Picture");
			DropForeignKey("RentalResolutionMember", "RentalResolutionId", "RentalResolution");
			DropForeignKey("RentalResolution", "RentalId", "Rental");
			DropForeignKey("RentalMandate", "RentalId", "Rental");
			DropForeignKey("RentalForm", "RentalId", "Rental");
			DropForeignKey("RentalForm", "DownloadId", "Download");
			DropForeignKey("RentalFica", "RentalId", "Rental");
			DropForeignKey("RentalFica", "RegisteredAddressId", "Address");
			DropForeignKey("RentalFica", "Partner3AddressId", "Address");
			DropForeignKey("RentalFica", "Partner2AddressId", "Address");
			DropForeignKey("RentalFica", "Partner1AddressId", "Address");
			DropForeignKey("RentalFica", "HeadOfficeAddressId", "Address");
			DropForeignKey("RentalFica", "BranchAddressId", "Address");
			DropForeignKey("LandlordDocumentation", "RentalLandlordId", "RentalLandlord");
			DropForeignKey("LandlordDocumentation", "DownloadId", "Download");
			DropForeignKey("LandlordBankAccount", "RentalId", "Rental");
			DropForeignKey("LandlordAddress", "RentalLandlordId", "RentalLandlord");
			DropForeignKey("RentalLandlord", "SignatureId", "Picture");
			DropForeignKey("RentalLandlord", "RentalId", "Rental");
			DropForeignKey("Rental", "AgentId", "Agent");
			DropForeignKey("Agent", "SignatureId", "Picture");
			DropForeignKey("Agent", "PersonId", "Person");
			DropForeignKey("Agent", "InitialsId", "Picture");
			DropForeignKey("Agent", "EaabProofOfPaymentId", "Download");
			DropForeignKey("RentalLandlord", "PostalAddressId", "Address");
			DropForeignKey("RentalLandlord", "PhysicalAddressId", "Address");
			DropForeignKey("RentalLandlord", "PersonId", "Person");
			DropForeignKey("RentalLandlord", "InitialsId", "Picture");
			DropForeignKey("Category", "PictureId", "Picture");
			DropForeignKey("Category", "ParentCategoryId", "Category");
			DropForeignKey("LandlordAddress", "AddressId", "Address");
			DropForeignKey("Session", "LoginId", "Login");
			DropForeignKey("SecurityGroupLogin", "SecurityGroupId", "SecurityGroup");
			DropForeignKey("SecurityGroupPermissions", "PermissionId", "Permission");
			DropForeignKey("SecurityGroupPermissions", "SecurityGroupId", "SecurityGroup");
			DropForeignKey("SecurityGroupLogin", "LoginId", "Login");
			DropForeignKey("Login", "PersonId", "Person");
			DropIndex("SecurityGroupPermissions", new[] { "PermissionId" });
			DropIndex("SecurityGroupPermissions", new[] { "SecurityGroupId" });
			DropIndex("AgentForm", new[] { "DownloadId" });
			DropIndex("AgentForm", new[] { "AgentId" });
			DropIndex("AgentDocumentation", new[] { "DownloadId" });
			DropIndex("AgentDocumentation", new[] { "AgentId" });
			DropIndex("AgentAddress", new[] { "AddressId" });
			DropIndex("AgentAddress", new[] { "AgentId" });
			DropIndex("Setting", "UIX_Setting_Key");
			DropIndex("Firm", new[] { "PostalAddressId" });
			DropIndex("Firm", new[] { "PhysicalAddressId" });
			DropIndex("Country", "UIX_Country_ThreeLetterIsoCode");
			DropIndex("Country", "UIX_Country_TwoLetterIsoCode");
			DropIndex("Country", "UIX_Country_Name");
			DropIndex("RentalWitness", new[] { "LandlordWitness2Id" });
			DropIndex("RentalWitness", new[] { "LandlordWitness1Id" });
			DropIndex("RentalWitness", new[] { "AgentWitness2Id" });
			DropIndex("RentalWitness", new[] { "AgentWitness1Id" });
			DropIndex("RentalWitness", new[] { "RentalId" });
			DropIndex("RentalResolutionMember", new[] { "SignatureId" });
			DropIndex("RentalResolutionMember", new[] { "RentalResolutionId" });
			DropIndex("RentalResolution", new[] { "RentalId" });
			DropIndex("RentalMandate", new[] { "RentalId" });
			DropIndex("RentalForm", new[] { "DownloadId" });
			DropIndex("RentalForm", new[] { "RentalId" });
			DropIndex("RentalFica", new[] { "Partner3AddressId" });
			DropIndex("RentalFica", new[] { "Partner2AddressId" });
			DropIndex("RentalFica", new[] { "Partner1AddressId" });
			DropIndex("RentalFica", new[] { "BranchAddressId" });
			DropIndex("RentalFica", new[] { "HeadOfficeAddressId" });
			DropIndex("RentalFica", new[] { "RegisteredAddressId" });
			DropIndex("RentalFica", new[] { "RentalId" });
			DropIndex("LandlordDocumentation", new[] { "DownloadId" });
			DropIndex("LandlordDocumentation", new[] { "RentalLandlordId" });
			DropIndex("LandlordBankAccount", new[] { "RentalId" });
			DropIndex("Agent", new[] { "EaabProofOfPaymentId" });
			DropIndex("Agent", new[] { "InitialsId" });
			DropIndex("Agent", new[] { "SignatureId" });
			DropIndex("Agent", new[] { "PersonId" });
			DropIndex("Rental", new[] { "AgentId" });
			DropIndex("Category", new[] { "PictureId" });
			DropIndex("Category", new[] { "ParentCategoryId" });
			DropIndex("Category", "IX_Category_Name");
			DropIndex("Category", "UIX_Category_Path");
			DropIndex("Category", "IX_Category_Nesting");
			DropIndex("Picture", "UIX_Picture_Path");
			DropIndex("RentalLandlord", new[] { "InitialsId" });
			DropIndex("RentalLandlord", new[] { "SignatureId" });
			DropIndex("RentalLandlord", new[] { "PostalAddressId" });
			DropIndex("RentalLandlord", new[] { "PhysicalAddressId" });
			DropIndex("RentalLandlord", new[] { "RentalId" });
			DropIndex("RentalLandlord", new[] { "PersonId" });
			DropIndex("LandlordAddress", new[] { "AddressId" });
			DropIndex("LandlordAddress", new[] { "RentalLandlordId" });
			DropIndex("Session", new[] { "LoginId" });
			DropIndex("Session", "UIX_Session_Ref");
			DropIndex("Permission", "UIX_SecurityGroup_PermissionUniqueName");
			DropIndex("SecurityGroupLogin", new[] { "SecurityGroupId" });
			DropIndex("SecurityGroupLogin", new[] { "LoginId" });
			DropIndex("Person", "UIX_Person_Email");
			DropIndex("Person", "UIX_Person_PersonGuid");
			DropIndex("Login", "UIX_Login_Email");
			DropIndex("Login", "UIX_Person_PersonId");
			DropTable("SecurityGroupPermissions");
			DropTable("AgentForm");
			DropTable("AgentDocumentation");
			DropTable("AgentAddress");
			DropTable("Setting");
			DropTable("Firm");
			DropTable("Country");
			DropTable("Content");
			DropTable("RentalWitness");
			DropTable("RentalResolutionMember");
			DropTable("RentalResolution");
			DropTable("RentalMandate");
			DropTable("RentalForm");
			DropTable("RentalFica");
			DropTable("LandlordDocumentation");
			DropTable("LandlordBankAccount");
			DropTable("Download");
			DropTable("Agent");
			DropTable("Rental");
			DropTable("Category");
			DropTable("Picture");
			DropTable("RentalLandlord");
			DropTable("Address");
			DropTable("LandlordAddress");
			DropTable("Session");
			DropTable("Permission");
			DropTable("SecurityGroup");
			DropTable("SecurityGroupLogin");
			DropTable("Person");
			DropTable("Login");
		}
	}
}

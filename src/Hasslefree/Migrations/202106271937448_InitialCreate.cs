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
				.ForeignKey("Address", t => t.AddressId, cascadeDelete: true)
				.ForeignKey("Agent", t => t.AgentId, cascadeDelete: true)
				.Index(t => t.AgentId)
				.Index(t => t.AddressId);

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
				.ForeignKey("Picture", t => t.EaabProofOfPaymentId, cascadeDelete: true)
				.ForeignKey("Picture", t => t.InitialsId, cascadeDelete: true)
				.ForeignKey("Person", t => t.PersonId, cascadeDelete: true)
				.ForeignKey("Picture", t => t.SignatureId, cascadeDelete: true)
				.Index(t => t.PersonId)
				.Index(t => t.SignatureId)
				.Index(t => t.InitialsId)
				.Index(t => t.EaabProofOfPaymentId);

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
				.ForeignKey("Agent", t => t.AgentId, cascadeDelete: true)
				.ForeignKey("Download", t => t.DownloadId, cascadeDelete: true)
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
				.ForeignKey("Agent", t => t.AgentId, cascadeDelete: true)
				.ForeignKey("Download", t => t.DownloadId, cascadeDelete: true)
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
			DropForeignKey("Agent", "SignatureId", "Picture");
			DropForeignKey("Agent", "PersonId", "Person");
			DropForeignKey("Agent", "InitialsId", "Picture");
			DropForeignKey("Agent", "EaabProofOfPaymentId", "Picture");
			DropForeignKey("AgentAddress", "AddressId", "Address");
			DropForeignKey("Firm", "PostalAddressId", "Address");
			DropForeignKey("Firm", "PhysicalAddressId", "Address");
			DropForeignKey("Category", "PictureId", "Picture");
			DropForeignKey("Category", "ParentCategoryId", "Category");
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
			DropIndex("Agent", new[] { "EaabProofOfPaymentId" });
			DropIndex("Agent", new[] { "InitialsId" });
			DropIndex("Agent", new[] { "SignatureId" });
			DropIndex("Agent", new[] { "PersonId" });
			DropIndex("AgentAddress", new[] { "AddressId" });
			DropIndex("AgentAddress", new[] { "AgentId" });
			DropIndex("Setting", "UIX_Setting_Key");
			DropIndex("Firm", new[] { "PostalAddressId" });
			DropIndex("Firm", new[] { "PhysicalAddressId" });
			DropIndex("Country", "UIX_Country_ThreeLetterIsoCode");
			DropIndex("Country", "UIX_Country_TwoLetterIsoCode");
			DropIndex("Country", "UIX_Country_Name");
			DropIndex("Category", new[] { "PictureId" });
			DropIndex("Category", new[] { "ParentCategoryId" });
			DropIndex("Category", "IX_Category_Name");
			DropIndex("Category", "UIX_Category_Path");
			DropIndex("Category", "IX_Category_Nesting");
			DropIndex("Picture", "UIX_Picture_Path");
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
			DropTable("Agent");
			DropTable("AgentAddress");
			DropTable("Setting");
			DropTable("Firm");
			DropTable("Country");
			DropTable("Address");
			DropTable("Category");
			DropTable("Picture");
			DropTable("Download");
			DropTable("Session");
			DropTable("Permission");
			DropTable("SecurityGroup");
			DropTable("SecurityGroupLogin");
			DropTable("Person");
			DropTable("Login");
		}
	}
}

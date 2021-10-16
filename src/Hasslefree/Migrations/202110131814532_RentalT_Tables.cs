namespace Hasslefree.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class RentalT_Tables : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"RentalT",
				c => new
				{
					RentalTId = c.Int(nullable: false, identity: true),
					UniqueId = c.Guid(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					AgentId = c.Int(nullable: false),
					LeaseTypeEnum = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					RentalTStatusEnum = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
					Children = c.Int(),
					Adults = c.Int(),
					ChildAge1 = c.Int(),
					ChildAge2 = c.Int(),
					ChildAge3 = c.Int(),
					ChildAge4 = c.Int(),
					SchoolNames = c.String(maxLength: 255, storeType: "nvarchar"),
					Pets = c.Int(),
					PetTypes = c.String(maxLength: 255, storeType: "nvarchar"),
					Vehicle1Type = c.String(maxLength: 50, storeType: "nvarchar"),
					Vehicle1Registration = c.String(maxLength: 20, storeType: "nvarchar"),
					Vehicle2Type = c.String(maxLength: 50, storeType: "nvarchar"),
					Vehicle2Registration = c.String(maxLength: 20, storeType: "nvarchar"),
					Vehicle3Type = c.String(maxLength: 50, storeType: "nvarchar"),
					Vehicle3Registration = c.String(maxLength: 20, storeType: "nvarchar"),
					Defaults = c.Boolean(nullable: false),
					DefaultsDescription = c.String(maxLength: 800, storeType: "nvarchar"),
					DebtReview = c.Boolean(nullable: false),
					DebtReviewDescription = c.String(maxLength: 800, storeType: "nvarchar"),
					Deposit = c.Decimal(precision: 15, scale: 5),
					KeyDeposit = c.Decimal(precision: 15, scale: 5),
					ElectricityDeposit = c.Decimal(precision: 15, scale: 5),
					LeaseFee = c.Decimal(precision: 15, scale: 5),
					ProRataRent = c.Decimal(precision: 15, scale: 5),
					FirstMonthRent = c.Decimal(precision: 15, scale: 5),
					UnitNumber = c.String(maxLength: 10, storeType: "nvarchar"),
					ComplexName = c.String(maxLength: 100, storeType: "nvarchar"),
					StreetNumber = c.String(maxLength: 10, storeType: "nvarchar"),
					StreetName = c.String(maxLength: 100, storeType: "nvarchar"),
					Suburb = c.String(maxLength: 100, storeType: "nvarchar"),
					City = c.String(maxLength: 100, storeType: "nvarchar"),
					Province = c.String(maxLength: 50, storeType: "nvarchar"),
					PostalCode = c.String(maxLength: 10, storeType: "nvarchar"),
					ParkingBayNumber = c.String(maxLength: 100, storeType: "nvarchar"),
					Smoking = c.Boolean(nullable: false),
					AllowPets = c.Boolean(nullable: false),
					PaymentMethod = c.String(maxLength: 100, storeType: "nvarchar"),
					DepositInterestAgent = c.Boolean(nullable: false),
					DepositInterestTenant = c.Boolean(nullable: false),
					PayDepositInAdvance = c.Boolean(nullable: false),
					ParkingFees = c.Decimal(precision: 15, scale: 5),
					LeaseAdminFee = c.Decimal(precision: 15, scale: 5),
					CreditCheckFee = c.Decimal(precision: 15, scale: 5),
					RentalEscalationPercentage = c.Decimal(precision: 15, scale: 5),
					InspectionFee = c.Decimal(precision: 15, scale: 5),
					Surcharge = c.Decimal(precision: 15, scale: 5),
					InitialPeriod = c.Int(),
					LeaseCommencementDate = c.DateTime(precision: 0),
					LeaseTerminationDate = c.DateTime(precision: 0),
					TenantFinancialBenefits = c.String(maxLength: 800, storeType: "nvarchar"),
					KeyReturnDateTime = c.DateTime(precision: 0),
					DirectMarketing = c.Boolean(nullable: false),
					MaximumOccupants = c.Int(),
					PermanentVehicles = c.String(maxLength: 255, storeType: "nvarchar"),
					Occupants = c.String(maxLength: 255, storeType: "nvarchar"),
					OccupantIdentityNumbers = c.String(maxLength: 100, storeType: "nvarchar"),
					MinimumCancellationMonths = c.Int(),
					MaximumCancellationMonthRent = c.Decimal(precision: 15, scale: 5),
					SalesCommision = c.Decimal(precision: 15, scale: 5),
					SpecialConditions = c.String(maxLength: 800, storeType: "nvarchar"),
					CommencementKeys = c.Int(),
					CommencementRemotes = c.Int(),
					CommencementSecurityTags = c.Int(),
					ReturnKeys = c.Int(),
					ReturnRemotes = c.Int(),
					ReturnSecurityTags = c.Int(),
				})
				.PrimaryKey(t => t.RentalTId)
				.ForeignKey("Agent", t => t.AgentId)
				.Index(t => t.AgentId);

			CreateTable(
				"RentalTJuristicApplicant",
				c => new
				{
					RentalTJuristicApplicantId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					UniqueId = c.Guid(nullable: false),
					RentalTJuristicId = c.Int(nullable: false),
					Position = c.String(maxLength: 50, storeType: "nvarchar"),
					Name = c.String(maxLength: 50, storeType: "nvarchar"),
					Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					IdNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					Nationality = c.String(maxLength: 50, storeType: "nvarchar"),
					TelHome = c.String(maxLength: 20, storeType: "nvarchar"),
					TelWork = c.String(maxLength: 20, storeType: "nvarchar"),
					Mobile = c.String(maxLength: 20, storeType: "nvarchar"),
					Fax = c.String(maxLength: 20, storeType: "nvarchar"),
					Email = c.String(maxLength: 100, storeType: "nvarchar"),
					InitialsId = c.Int(nullable: false),
					SignatureId = c.Int(nullable: false),
					SignedAt = c.String(maxLength: 50, storeType: "nvarchar"),
					SignedOn = c.DateTime(precision: 0),
					RentalT_RentalTId = c.Int(),
				})
				.PrimaryKey(t => t.RentalTJuristicApplicantId)
				.ForeignKey("Picture", t => t.InitialsId)
				.ForeignKey("RentalTJuristic", t => t.RentalTJuristicId)
				.ForeignKey("Picture", t => t.SignatureId)
				.ForeignKey("RentalT", t => t.RentalT_RentalTId)
				.Index(t => t.RentalTJuristicId)
				.Index(t => t.InitialsId)
				.Index(t => t.SignatureId)
				.Index(t => t.RentalT_RentalTId);

			CreateTable(
				"RentalTJuristic",
				c => new
				{
					RentalTJuristicId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					UniqueId = c.Guid(nullable: false),
					RentalTId = c.Int(nullable: false),
					EntityName = c.String(maxLength: 100, storeType: "nvarchar"),
					Address = c.String(maxLength: 255, storeType: "nvarchar"),
					RegisteredName = c.String(maxLength: 100, storeType: "nvarchar"),
					TradingName = c.String(maxLength: 50, storeType: "nvarchar"),
					RegistrationNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					VatRegistrationNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					DateOfIncorporation = c.DateTime(precision: 0),
					NatureOfBusiness = c.String(maxLength: 50, storeType: "nvarchar"),
					PeriodInBusiness = c.String(maxLength: 30, storeType: "nvarchar"),
					FinancialYearEnd = c.DateTime(precision: 0),
					BusinessAddressId = c.Int(nullable: false),
					PostalAddressId = c.Int(nullable: false),
					ContactPersonFirstName = c.String(maxLength: 50, storeType: "nvarchar"),
					ContactPersonSurname = c.String(maxLength: 50, storeType: "nvarchar"),
					ContactPersonPosition = c.String(maxLength: 50, storeType: "nvarchar"),
					Tel = c.String(maxLength: 20, storeType: "nvarchar"),
					Mobile = c.String(maxLength: 20, storeType: "nvarchar"),
					Fax = c.String(maxLength: 20, storeType: "nvarchar"),
					Email = c.String(maxLength: 100, storeType: "nvarchar"),
					Bank = c.String(maxLength: 50, storeType: "nvarchar"),
					BranchName = c.String(maxLength: 20, storeType: "nvarchar"),
					BranchCode = c.String(maxLength: 10, storeType: "nvarchar"),
					AccountNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					AccountType = c.String(maxLength: 50, storeType: "nvarchar"),
				})
				.PrimaryKey(t => t.RentalTJuristicId)
				.ForeignKey("Address", t => t.BusinessAddressId)
				.ForeignKey("Address", t => t.PostalAddressId)
				.ForeignKey("RentalT", t => t.RentalTId)
				.Index(t => t.RentalTId)
				.Index(t => t.BusinessAddressId)
				.Index(t => t.PostalAddressId);

			CreateTable(
				"Tenant",
				c => new
				{
					TenantId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					UniqueId = c.Guid(nullable: false),
					RentalTId = c.Int(nullable: false),
					Name = c.String(maxLength: 50, storeType: "nvarchar"),
					Surname = c.String(maxLength: 100, storeType: "nvarchar"),
					MaidenName = c.String(maxLength: 50, storeType: "nvarchar"),
					IdNumber = c.String(maxLength: 20, storeType: "nvarchar"),
					VatNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					Nationality = c.String(maxLength: 50, storeType: "nvarchar"),
					Married = c.Boolean(nullable: false),
					MarriedType = c.String(maxLength: 50, storeType: "nvarchar"),
					TelHome = c.String(maxLength: 20, storeType: "nvarchar"),
					TelWork = c.String(maxLength: 20, storeType: "nvarchar"),
					Fax = c.String(maxLength: 20, storeType: "nvarchar"),
					Mobile = c.String(maxLength: 20, storeType: "nvarchar"),
					Email = c.String(maxLength: 100, storeType: "nvarchar"),
					PhysicalAddressId = c.Int(nullable: false),
					PostalAddressId = c.Int(nullable: false),
					NextOfKin = c.String(maxLength: 100, storeType: "nvarchar"),
					OwnerOfProperty = c.Boolean(nullable: false),
					PreviousRentPaid = c.Decimal(precision: 15, scale: 5),
					PreviousStayDuration = c.String(maxLength: 50, storeType: "nvarchar"),
					PreviousLandlord = c.String(maxLength: 100, storeType: "nvarchar"),
					PreviousLandlordContactNumber = c.String(maxLength: 20, storeType: "nvarchar"),
					Bank = c.String(maxLength: 30, storeType: "nvarchar"),
					Branch = c.String(maxLength: 50, storeType: "nvarchar"),
					BranchCode = c.String(maxLength: 10, storeType: "nvarchar"),
					AccountNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					TypeOfAccount = c.String(maxLength: 50, storeType: "nvarchar"),
					BankReference = c.String(maxLength: 50, storeType: "nvarchar"),
					SelfEmployed = c.Boolean(),
					Occupation = c.String(maxLength: 100, storeType: "nvarchar"),
					CurrentEmployer = c.String(maxLength: 100, storeType: "nvarchar"),
					EmployerAddressId = c.Int(nullable: false),
					PeriodOfEmployment = c.String(maxLength: 50, storeType: "nvarchar"),
					GrossMonthlySalary = c.Decimal(precision: 15, scale: 5),
					NetMonthlySalary = c.Decimal(precision: 15, scale: 5),
					SalaryPaymentDate = c.DateTime(precision: 0),
					CurrentMonthlyExpenses = c.Decimal(precision: 15, scale: 5),
					MainApplicant = c.Boolean(nullable: false),
					InitialsId = c.Int(nullable: false),
					SignatureId = c.Int(nullable: false),
					SignedAt = c.String(unicode: false),
					SignedOn = c.DateTime(precision: 0),
					RentalT_RentalTId = c.Int(),
				})
				.PrimaryKey(t => t.TenantId)
				.ForeignKey("Address", t => t.EmployerAddressId)
				.ForeignKey("Picture", t => t.InitialsId)
				.ForeignKey("Address", t => t.PhysicalAddressId)
				.ForeignKey("Address", t => t.PostalAddressId)
				.ForeignKey("RentalT", t => t.RentalTId)
				.ForeignKey("Picture", t => t.SignatureId)
				.ForeignKey("RentalT", t => t.RentalT_RentalTId)
				.Index(t => t.RentalTId)
				.Index(t => t.PhysicalAddressId)
				.Index(t => t.PostalAddressId)
				.Index(t => t.EmployerAddressId)
				.Index(t => t.InitialsId)
				.Index(t => t.SignatureId)
				.Index(t => t.RentalT_RentalTId);

			CreateTable(
				"RentalTFica",
				c => new
				{
					RentalTFicaId = c.Int(nullable: false, identity: true),
					UniqueId = c.Guid(nullable: false),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					RentalTId = c.Int(nullable: false),
					ForeignMemberOfRoyalFamily = c.Boolean(nullable: false),
					ForeignMemberOfCabinet = c.Boolean(nullable: false),
					ForeignSeniorPolitical = c.Boolean(nullable: false),
					ForeignSeniorJudicialOfficer = c.Boolean(nullable: false),
					ForeignSeniorExecutives = c.Boolean(nullable: false),
					ForeignHighRankingMilitary = c.Boolean(nullable: false),
					DomesticDeputyPresident = c.Boolean(nullable: false),
					DomesticCabinetMinister = c.Boolean(nullable: false),
					DomesticPremier = c.Boolean(nullable: false),
					DomesticMemberOfCouncil = c.Boolean(nullable: false),
					DomesticMayor = c.Boolean(nullable: false),
					DomesticLeaderPolitical = c.Boolean(nullable: false),
					DomesticRoyalFamily = c.Boolean(nullable: false),
					DomesticCfoProvincial = c.Boolean(nullable: false),
					DomesticCfoMunicipality = c.Boolean(nullable: false),
					DomesticCeo = c.Boolean(nullable: false),
					DomesticJudge = c.Boolean(nullable: false),
					DomesticAmbassador = c.Boolean(nullable: false),
					DomesticOther = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.RentalTFicaId)
				.ForeignKey("RentalT", t => t.RentalTId)
				.Index(t => t.RentalTId);

			CreateTable(
				"RentalTLandlord",
				c => new
				{
					RentalTLandlordId = c.Int(nullable: false, identity: true),
					CreatedOn = c.DateTime(nullable: false, precision: 0),
					ModifiedOn = c.DateTime(nullable: false, precision: 0),
					UniqueId = c.Guid(nullable: false),
					Tempdata = c.String(maxLength: 255, storeType: "nvarchar"),
					IdNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					PersonId = c.Int(nullable: false),
					RentalTId = c.Int(nullable: false),
					VatNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					IncomeTaxNumber = c.String(maxLength: 50, storeType: "nvarchar"),
					Bank = c.String(maxLength: 50, storeType: "nvarchar"),
					Branch = c.String(maxLength: 50, storeType: "nvarchar"),
					BranchCode = c.String(maxLength: 10, storeType: "nvarchar"),
					AccountNumber = c.String(maxLength: 30, storeType: "nvarchar"),
					TypeOfAccount = c.String(maxLength: 30, storeType: "nvarchar"),
					BankReference = c.String(maxLength: 50, storeType: "nvarchar"),
					TelHome = c.String(maxLength: 20, storeType: "nvarchar"),
					TelWork = c.String(maxLength: 20, storeType: "nvarchar"),
					Fax = c.String(maxLength: 20, storeType: "nvarchar"),
					Mobile = c.String(maxLength: 30, storeType: "nvarchar"),
					Email = c.String(maxLength: 100, storeType: "nvarchar"),
					SignatureId = c.Int(nullable: false),
					InitialsId = c.Int(nullable: false),
					SignedAt = c.String(maxLength: 50, storeType: "nvarchar"),
					SignedOn = c.DateTime(precision: 0),
				})
				.PrimaryKey(t => t.RentalTLandlordId)
				.ForeignKey("Picture", t => t.InitialsId)
				.ForeignKey("Person", t => t.PersonId)
				.ForeignKey("RentalT", t => t.RentalTId)
				.ForeignKey("Picture", t => t.SignatureId)
				.Index(t => t.PersonId)
				.Index(t => t.RentalTId)
				.Index(t => t.SignatureId)
				.Index(t => t.InitialsId);

			AddColumn("RentalLandlord", "PhysicalAddressId", c => c.Int());
			AddColumn("RentalLandlord", "PostalAddressId", c => c.Int());
			AddColumn("RentalLandlord", "PhysicalAddress_AddressId", c => c.Int());
			AddColumn("RentalLandlord", "PostalAddress_AddressId", c => c.Int());
			CreateIndex("RentalLandlord", "PhysicalAddress_AddressId");
			CreateIndex("RentalLandlord", "PostalAddress_AddressId");
			AddForeignKey("RentalLandlord", "PhysicalAddress_AddressId", "Address", "AddressId");
			AddForeignKey("RentalLandlord", "PostalAddress_AddressId", "Address", "AddressId");
		}

		public override void Down()
		{
			DropForeignKey("RentalTLandlord", "SignatureId", "Picture");
			DropForeignKey("RentalTLandlord", "RentalTId", "RentalT");
			DropForeignKey("RentalTLandlord", "PersonId", "Person");
			DropForeignKey("RentalTLandlord", "InitialsId", "Picture");
			DropForeignKey("RentalTFica", "RentalTId", "RentalT");
			DropForeignKey("Tenant", "RentalT_RentalTId", "RentalT");
			DropForeignKey("Tenant", "SignatureId", "Picture");
			DropForeignKey("Tenant", "RentalTId", "RentalT");
			DropForeignKey("Tenant", "PostalAddressId", "Address");
			DropForeignKey("Tenant", "PhysicalAddressId", "Address");
			DropForeignKey("Tenant", "InitialsId", "Picture");
			DropForeignKey("Tenant", "EmployerAddressId", "Address");
			DropForeignKey("RentalTJuristicApplicant", "RentalT_RentalTId", "RentalT");
			DropForeignKey("RentalTJuristicApplicant", "SignatureId", "Picture");
			DropForeignKey("RentalTJuristicApplicant", "RentalTJuristicId", "RentalTJuristic");
			DropForeignKey("RentalTJuristic", "RentalTId", "RentalT");
			DropForeignKey("RentalTJuristic", "PostalAddressId", "Address");
			DropForeignKey("RentalTJuristic", "BusinessAddressId", "Address");
			DropForeignKey("RentalTJuristicApplicant", "InitialsId", "Picture");
			DropForeignKey("RentalT", "AgentId", "Agent");
			DropForeignKey("RentalLandlord", "PostalAddress_AddressId", "Address");
			DropForeignKey("RentalLandlord", "PhysicalAddress_AddressId", "Address");
			DropIndex("RentalTLandlord", new[] { "InitialsId" });
			DropIndex("RentalTLandlord", new[] { "SignatureId" });
			DropIndex("RentalTLandlord", new[] { "RentalTId" });
			DropIndex("RentalTLandlord", new[] { "PersonId" });
			DropIndex("RentalTFica", new[] { "RentalTId" });
			DropIndex("Tenant", new[] { "RentalT_RentalTId" });
			DropIndex("Tenant", new[] { "SignatureId" });
			DropIndex("Tenant", new[] { "InitialsId" });
			DropIndex("Tenant", new[] { "EmployerAddressId" });
			DropIndex("Tenant", new[] { "PostalAddressId" });
			DropIndex("Tenant", new[] { "PhysicalAddressId" });
			DropIndex("Tenant", new[] { "RentalTId" });
			DropIndex("RentalTJuristic", new[] { "PostalAddressId" });
			DropIndex("RentalTJuristic", new[] { "BusinessAddressId" });
			DropIndex("RentalTJuristic", new[] { "RentalTId" });
			DropIndex("RentalTJuristicApplicant", new[] { "RentalT_RentalTId" });
			DropIndex("RentalTJuristicApplicant", new[] { "SignatureId" });
			DropIndex("RentalTJuristicApplicant", new[] { "InitialsId" });
			DropIndex("RentalTJuristicApplicant", new[] { "RentalTJuristicId" });
			DropIndex("RentalT", new[] { "AgentId" });
			DropIndex("RentalLandlord", new[] { "PostalAddress_AddressId" });
			DropIndex("RentalLandlord", new[] { "PhysicalAddress_AddressId" });
			DropColumn("RentalLandlord", "PostalAddress_AddressId");
			DropColumn("RentalLandlord", "PhysicalAddress_AddressId");
			DropColumn("RentalLandlord", "PostalAddressId");
			DropColumn("RentalLandlord", "PhysicalAddressId");
			DropTable("RentalTLandlord");
			DropTable("RentalTFica");
			DropTable("Tenant");
			DropTable("RentalTJuristic");
			DropTable("RentalTJuristicApplicant");
			DropTable("RentalT");
		}
	}
}

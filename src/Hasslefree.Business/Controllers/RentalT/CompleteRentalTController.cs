using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Landlords.Crud;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.RentalT
{
    public class CompleteRentalTController : BaseController
    {
        #region Private Properties 

        //Repos
        private IDataRepository<LandlordAddress> LandlordAddressRepo { get; }
        private IReadOnlyRepository<Person> PersonRepo { get; }

        // Services
        private IGetRentalTService GetRental { get; }
        private IUpdateRentalTService UpdateRentalService { get; }
        private IUpdateRentalLandlordService UpdateRentalLandlordService { get; }
        private IUpdateRentalMandateService UpdateRentalMandateService { get; }
        private ICreatePersonService CreatePerson { get; }
        private ILogoutService LogoutService { get; }
        private ILoginService LoginService { get; }
        private ICountryQueryService Countries { get; }
        private ICreateLandlordBankAccountService CreateLandlordBankAccountService { get; }
        private IUpdateRentalFicaService UpdateRentalFicaService { get; }
        private ICreateRentalResolutionService CreateRentalResolutionService { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalTController
        (
            //Repos
            IDataRepository<LandlordAddress> landlordAddressRepo,
            IReadOnlyRepository<Person> personRepo,

            //Services
            IGetRentalTService getRental,
            IUpdateRentalTService updateRentalService,
            IUpdateRentalMandateService updateRentalMandateService,
            ICreatePersonService createPerson,
            ILogoutService logoutService,
            ICountryQueryService countries,
            IUpdateRentalLandlordService updateRentalLandlordService,
            ICreateLandlordBankAccountService createLandlordBankAccountService,
            ILoginService loginService,
            IUpdateRentalFicaService updateRentalFicaService,
            ICreateRentalResolutionService createRentalResolutionService,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            //Repos
            LandlordAddressRepo = landlordAddressRepo;
            PersonRepo = personRepo;

            // Services
            GetRental = getRental;
            UpdateRentalService = updateRentalService;
            CreatePerson = createPerson;
            LogoutService = logoutService;
            Countries = countries;
            UpdateRentalLandlordService = updateRentalLandlordService;
            UpdateRentalMandateService = updateRentalMandateService;
            CreateLandlordBankAccountService = createLandlordBankAccountService;
            LoginService = loginService;
            UpdateRentalFicaService = updateRentalFicaService;
            CreateRentalResolutionService = createRentalResolutionService;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rentalt/complete-rental")]
        public ActionResult CompleteRegistration(string hash)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rentalt/complete-rental?hash={hash}");
            }

            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            string rentalUniqueId = decodedHash.Split(';')[0];
            int tenantId = Int32.Parse(decodedHash.Split(';')[1]);

            var rental = GetRental[rentalUniqueId].Get();
            var tenant = rental.Tenants.FirstOrDefault(r => r.TenantId == tenantId);

            if (rental.Status != RentalTStatus.PendingNew) return Redirect($"/account/rentalt/complete-documentation?hash={hash}");

            var model = new CompleteRentalT
            {
                RentalTId = rental.RentalTId,
                Name = GetTempData(tenant.Tempdata).Split(';')[0],
                Surname = GetTempData(tenant.Tempdata).Split(';')[1],
                Email = GetTempData(tenant.Tempdata).Split(';')[2],
                Mobile = GetTempData(tenant.Tempdata).Split(';')[3],
                IdNumber = GetTempData(tenant.Tempdata).Split(';')[4],
                Address = rental.Rental.Address,
                Premises = rental.Rental.Premises,
                StandErf = rental.Rental.StandErf,
                Township = rental.Rental.Township
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteRegistration", model);

            // Default
            return View("../Rentals/RentalTs/CompleteRegistration", model);
        }

        [HttpPost, Route("account/rentalt/complete-rental")]
        [SessionFilter(Order = 3)]
        public ActionResult CompleteRegistration(CompleteRentalT model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
                    {
                        var rental = GetRental[model.RentalTId].Get();
                        var tenant = rental.Tenants.FirstOrDefault(r => r.TenantId == model.TenantId);

                        int personId;
                        Guid personGuid;
                        if (PersonRepo.Table.Any(p => p.Email.ToLower() == model.Email.ToLower()))
                        {
                            var person = PersonRepo.Table.FirstOrDefault(p => p.Email.ToLower() == model.Email.ToLower());
                            personId = person.PersonId;
                            personGuid = person.PersonGuid;
                        }
                        else
                        {
                            //create the person (landlord)
                            CreatePerson
                            .New(model.Name, "", model.Surname, model.Email, model.Title.ResolveTitle(), null, model.Gender, model.IdNumber)
                            .WithContactDetails(null, null, model.Mobile)
                            .WithPassword(model.Password, "")
                            .WithSecurityGroup("Landlord")
                            .Create();

                            personId = CreatePerson.PersonId;
                            personGuid = CreatePerson.PersonGuid;
                        }

                        var success = UpdateRentalService[model.RentalTId]
                        //.Set(a => a.Address, model.Address)
                        //.Set(a => a.Deposit, model.Deposit)
                        //.Set(a => a.DepositPaymentDate, model.DepositPaymentDate)
                        //.Set(a => a.ModifiedOn, DateTime.Now)
                        //.Set(a => a.MonthlyPaymentDate, model.RentalPaymentDate)
                        //.Set(a => a.MonthlyRental, model.MonthlyRental)
                        //.Set(a => a.Premises, model.Premises)
                        //.Set(a => a.StandErf, model.StandErf)
                        //.Set(a => a.Township, model.Township)
                        //.Set(a => a.RentalStatus, RentalStatus.PendingLandlordDocumentation)
                        //.Set(a => a.Marketing, model.Marketing)
                        //.Set(a => a.Procurement, model.Procurement)
                        //.Set(a => a.Management, model.Management)
                        //.Set(a => a.Negotiating, model.Negotiating)
                        //.Set(a => a.Informing, model.Informing)
                        //.Set(a => a.IncomingSnaglist, model.IncomingSnaglist)
                        //.Set(a => a.OutgoingSnaglist, model.OutgoingSnaglist)
                        //.Set(a => a.Explaining, model.Explaining)
                        //.Set(a => a.PayingLandlord, model.PayingLandlord)
                        //.Set(a => a.ContactLandlord, model.ContactLandlord)
                        //.Set(a => a.ProvideLandlord, model.ProvideLandlord)
                        //.Set(a => a.AskLandlordConsent, model.AskLandlordConsent)
                        //.Set(a => a.ProcureDepositLandlord, model.ProcureDepositLandlord)
                        //.Set(a => a.ProcureDepositPreviousRentalAgent, model.ProcureDepositPreviousRentalAgent)
                        //.Set(a => a.ProcureDepositOther, model.ProcureDepositOther)
                        //.Set(a => a.TransferDeposit, model.TransferDeposit)
                        //.Set(a => a.SpecificRequirements, model.SpecificRequirements)
                        //.Set(a => a.SpecialConditions, model.SpecialConditions)
                        //.Set(a => a.PowerOfAttorney, model.PowerOfAttorney)
                        .Update();

                        //var physicalAddress = new Address()
                        //{
                        //    Type = AddressType.Residential,
                        //    Address1 = model.ResidentialAddress1,
                        //    Address2 = model.ResidentialAddress2,
                        //    Address3 = model.ResidentialAddress3,
                        //    Code = model.ResidentialAddressCode,
                        //    Country = model.ResidentialAddressCountry,
                        //    RegionName = model.ResidentialAddressProvince,
                        //    Town = model.ResidentialAddressTown
                        //};

                        //var postalAddress = new Address()
                        //{
                        //    Type = AddressType.Postal,
                        //    Address1 = model.PostalAddress1,
                        //    Address2 = model.PostalAddress2,
                        //    Address3 = model.PostalAddress3,
                        //    Code = model.PostalAddressCode,
                        //    Country = model.PostalAddressCountry,
                        //    RegionName = model.PostalAddressProvince,
                        //    Town = model.PostalAddressTown
                        //};

                        ////add the addresses
                        //LandlordAddressRepo.Insert(new LandlordAddress()
                        //{
                        //    Address = physicalAddress,
                        //    RentalLandlordId = landlord.RentalLandlordId
                        //});

                        //LandlordAddressRepo.Insert(new LandlordAddress()
                        //{
                        //    Address = postalAddress,
                        //    RentalLandlordId = landlord.RentalLandlordId
                        //});

                        //success = UpdateRentalLandlordService[landlord.RentalLandlordId]
                        //.Set(x => x.PersonId, personId)
                        //.Set(x => x.VatNumber, model.VatNumber)
                        //.Set(x => x.IncomeTaxNumber, model.IncomeTaxNumber)
                        //.Update();

                        //var rentalMandateId = 0;
                        //if (rental.RentalMandate != null) rentalMandateId = rental.RentalMandate.RentalMandateId;

                        //success = UpdateRentalMandateService.WithRentalId(rental.RentalId)[rentalMandateId]
                        //.Set(x => x.Procurement1Percentage, model.Procurement1Percentage)
                        //.Set(x => x.Procurement1Amount, model.Procurement1Amount)
                        //.Set(x => x.Procurement2Percentage, model.Procurement2Percentage)
                        //.Set(x => x.Procurement2Amount, model.Procurement2Amount)
                        //.Set(x => x.Procurement3Percentage, model.Procurement3Percentage)
                        //.Set(x => x.Procurement3Amount, model.Procurement3Amount)
                        //.Set(x => x.ManagementAmount, model.ManagementAmount)
                        //.Set(x => x.ManagementPercentage, model.ManagementPercentage)
                        //.Set(x => x.SaleAmount, model.SaleAmount)
                        //.Set(x => x.SalePercentage, model.SalePercentage)
                        //.Update();

                        //var rentalFicaId = 0;
                        //if (rental.RentalFica != null) rentalFicaId = rental.RentalFica.RentalFicaId;

                        //if (rental.LeaseType == LeaseType.ClosedCorporation || rental.LeaseType == LeaseType.Company || rental.LeaseType == LeaseType.Trust)
                        //{
                        //    var branchAddress = new Address()
                        //    {
                        //        Address1 = model.BranchAddress1,
                        //        Address2 = model.BranchAddress2,
                        //        Address3 = model.BranchAddress3,
                        //        Code = model.BranchAddressCode,
                        //        Country = model.BranchAddressCountry,
                        //        RegionName = model.BranchAddressProvince,
                        //        Town = model.BranchAddressTown,
                        //        Type = AddressType.Residential
                        //    };

                        //    var headOfficeAddress = new Address()
                        //    {
                        //        Address1 = model.HeadOfficeAddress1,
                        //        Address2 = model.HeadOfficeAddress2,
                        //        Address3 = model.HeadOfficeAddress3,
                        //        Code = model.HeadOfficeAddressCode,
                        //        Country = model.HeadOfficeAddressCountry,
                        //        RegionName = model.HeadOfficeAddressProvince,
                        //        Town = model.HeadOfficeAddressTown,
                        //        Type = AddressType.Residential
                        //    };

                        //    var partner1Address = new Address()
                        //    {
                        //        Address1 = model.Partner1Address1,
                        //        Address2 = model.Partner1Address2,
                        //        Address3 = model.Partner1Address3,
                        //        Code = model.Partner1AddressPostalCode,
                        //        Country = model.Partner1AddressCountry,
                        //        RegionName = model.Partner1AddressProvince,
                        //        Town = model.Partner1AddressCity,
                        //        Type = AddressType.Residential
                        //    };

                        //    var partner2Address = new Address()
                        //    {
                        //        Address1 = model.Partner2Address1,
                        //        Address2 = model.Partner2Address2,
                        //        Address3 = model.Partner2Address3,
                        //        Code = model.Partner2AddressPostalCode,
                        //        Country = model.Partner2AddressCountry,
                        //        RegionName = model.Partner2AddressProvince,
                        //        Town = model.Partner2AddressCity,
                        //        Type = AddressType.Residential
                        //    };

                        //    var partner3Address = new Address()
                        //    {
                        //        Address1 = model.Partner3Address1,
                        //        Address2 = model.Partner3Address2,
                        //        Address3 = model.Partner3Address3,
                        //        Code = model.Partner3AddressPostalCode,
                        //        Country = model.Partner3AddressCountry,
                        //        RegionName = model.Partner3AddressProvince,
                        //        Town = model.Partner3AddressCity,
                        //        Type = AddressType.Residential
                        //    };

                        //    var registeredAddress = new Address()
                        //    {
                        //        Address1 = model.RegisteredAddress1,
                        //        Address2 = model.RegisteredAddress2,
                        //        Address3 = model.RegisteredAddress3,
                        //        Code = model.RegisteredAddressCode,
                        //        Country = model.RegisteredAddressCountry,
                        //        RegionName = model.RegisteredAddressProvince,
                        //        Town = model.RegisteredAddressTown,
                        //        Type = AddressType.Residential
                        //    };

                        //    var updateFica = UpdateRentalFicaService.WithRentalId(rental.RentalId)[rentalFicaId]
                        //    .Set(x => x.Mobile, model.Mobile)
                        //    .Set(x => x.Email, model.Email)
                        //    .Set(x => x.BranchAddress, branchAddress)
                        //    .Set(x => x.HeadOfficeAddress, headOfficeAddress)
                        //    .Set(x => x.Partner1Email, model.Partner1Email)
                        //    .Set(x => x.Partner1Fax, model.Partner1Fax)
                        //    .Set(x => x.Partner1IdNumber, model.Partner1IdNumber)
                        //    .Set(x => x.Partner1Mobile, model.Partner1Mobile)
                        //    .Set(x => x.Partner1Name, model.Partner1Name)
                        //    .Set(x => x.Partner1Nationality, model.Partner1Nationality)
                        //    .Set(x => x.Partner1Phone, model.Partner1Phone)
                        //    .Set(x => x.Partner1Surname, model.Partner1Surname)
                        //    .Set(x => x.Partner1Work, model.Partner1Work)
                        //    .Set(x => x.Partner2Email, model.Partner2Email)
                        //    .Set(x => x.Partner2Fax, model.Partner2Fax)
                        //    .Set(x => x.Partner2IdNumber, model.Partner2IdNumber)
                        //    .Set(x => x.Partner2Mobile, model.Partner2Mobile)
                        //    .Set(x => x.Partner2Name, model.Partner2Name)
                        //    .Set(x => x.Partner2Nationality, model.Partner2Nationality)
                        //    .Set(x => x.Partner2Phone, model.Partner2Phone)
                        //    .Set(x => x.Partner2Surname, model.Partner2Surname)
                        //    .Set(x => x.Partner2Work, model.Partner2Work)
                        //    .Set(x => x.Partner3Email, model.Partner3Email)
                        //    .Set(x => x.Partner3Fax, model.Partner3Fax)
                        //    .Set(x => x.Partner3IdNumber, model.Partner3IdNumber)
                        //    .Set(x => x.Partner3Mobile, model.Partner3Mobile)
                        //    .Set(x => x.Partner3Name, model.Partner3Name)
                        //    .Set(x => x.Partner3Nationality, model.Partner3Nationality)
                        //    .Set(x => x.Partner3Phone, model.Partner3Phone)
                        //    .Set(x => x.Partner3Surname, model.Partner3Surname)
                        //    .Set(x => x.Partner3Work, model.Partner3Work)
                        //    .Set(x => x.RegisteredAddress, registeredAddress)
                        //    .Set(x => x.RegisteredBusinessName, model.RegisteredBusinessName)
                        //    .Set(x => x.RegistrationNumber, model.RegistrationNumber)
                        //    .Set(x => x.StaffMember, model.StaffMember)
                        //    .Set(x => x.TradeName, model.TradeName)
                        //    .Set(x => x.CompanyType, model.CompanyType)
                        //    .Set(x => x.TransactionType, model.TransactionType);

                        //    if (!String.IsNullOrEmpty(model.Partner1Address1)) updateFica = updateFica.Set(x => x.Partner1Address, partner1Address);
                        //    if (!String.IsNullOrEmpty(model.Partner2Address1)) updateFica = updateFica.Set(x => x.Partner2Address, partner2Address);
                        //    if (!String.IsNullOrEmpty(model.Partner3Address1)) updateFica = updateFica.Set(x => x.Partner3Address, partner3Address);

                        //    success = updateFica.Update();

                        //    //create the rental resolution member
                        //    CreateRentalResolutionService.WithRentalId(rental.RentalId)
                        //    .New(model.HeldAt, DateTime.Parse(model.HeldOn), model.LeaseName, model.AuthorizedName, model.AuthorizedSurname);

                        //    foreach (var member in model.Members) CreateRentalResolutionService.WithMember(member.Name, member.Surname, member.Email, member.IdNumber);

                        //    CreateRentalResolutionService.Create();
                        //}

                        ////create the landlord bank account
                        //success = CreateLandlordBankAccountService.WithRentalId(rental.RentalId)
                        //.New(model.AccountHolder, model.Bank, model.Branch, model.BranchCode, model.AccountNumber, model.BankReference)
                        //.Create();

                        //// Success
                        //if (success)
                        //{
                        //    //Auto login the new landlord
                        //    LoginService.WithGuid(personGuid).Login();

                        //    //complete the scope
                        //    transactionScope.Complete();

                        //    // Ajax (+ Json)
                        //    if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        //    {
                        //        Success = true,
                        //        AgentId = 1,
                        //    }, JsonRequestBehavior.AllowGet);

                        //    var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalGuid.ToString().ToLower()};{landlord.UniqueId.ToString().ToLower()}"));

                        //    // Default
                        //    return Redirect($"/account/rental/complete-documentation?hash={hash}");
                        //}

                    }
                }
            }
            catch (DbEntityValidationException ev)
            {
                foreach (var e in ev.EntityValidationErrors)
                {
                    foreach (var error in e.ValidationErrors) ModelState.AddModelError("", error.ErrorMessage);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            PrepViewBags();

            var errors = "";

            if (UpdateRentalService.HasWarnings) UpdateRentalService.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteRegistration", model);

            // Default
            return View("../Rentals/CompleteRegistration", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Rental Tenant";

            ViewBag.Titles = new List<string> { "Mr", "Miss", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
            ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
            ViewBag.CompanyTypes = Enum.GetNames(typeof(CompanyType)).ToList();
            ViewBag.Provinces = new List<string> { "Eastern Cape", "Free State", "Gauteng", "KwaZulu Natal", "Limpopo", "Mpumalanga", "North West", "Northern Cape", "Western Cape" };
            ViewBag.Countries = Countries.Get().Select(c => c.Name).ToList();
        }

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        private DateTime CalculateDateOfBirth(string idNumber)
        {
            string id = idNumber.Substring(0, 6);
            string y = id.Substring(0, 2);
            string year = $"20{y}";
            if (Int32.Parse(id.Substring(0, 1)) > 2) year = $"19{y}";

            int month = Int32.Parse(id.Substring(2, 2));
            int day = Int32.Parse(id.Substring(4, 2));

            return new DateTime(Int32.Parse(year), month, day);
        }

        #endregion
    }
}
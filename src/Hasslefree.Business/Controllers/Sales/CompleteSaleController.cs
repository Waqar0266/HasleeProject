using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Sales;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Sales.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Sales;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Sales
{
    public class CompleteSaleController : BaseController
    {
        #region Private Properties 

        //Repos
        private IReadOnlyRepository<Person> PersonRepo { get; }
        private IDataRepository<Address> AddressRepo { get; }

        // Services
        private IGetSaleService GetSale { get; }
        private IUpdateSaleService UpdateSaleService { get; }
        private IUpdateSellerService UpdateSellerService { get; }
        private ICreatePersonService CreatePerson { get; }
        private ILogoutService LogoutService { get; }
        private ILoginService LoginService { get; }
        private ICountryQueryService CountryService { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }
        private ISendMail SendMail { get; }

        #endregion

        #region Constructor

        public CompleteSaleController
        (
            //Repos
            IReadOnlyRepository<Person> personRepo,
            IDataRepository<Address> addressRepo,

            //Services
            IGetSaleService getSale,
            IUpdateSaleService updateSaleService,
            IUpdateSellerService updateSellerService,
            ICreatePersonService createPerson,
            ILogoutService logoutService,
            ILoginService loginService,
            ICountryQueryService countryService,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager,
            ISendMail sendMail
        )
        {
            //Repos
            PersonRepo = personRepo;
            AddressRepo = addressRepo;

            // Services
            GetSale = getSale;
            UpdateSaleService = updateSaleService;
            UpdateSellerService = updateSellerService;
            CreatePerson = createPerson;
            LogoutService = logoutService;
            LoginService = loginService;
            CountryService = countryService;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
            SendMail = sendMail;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/sale/complete-sale")]
        public ActionResult CompleteRegistration(string hash)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/sale/complete-sale?hash={hash}");
            }

            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            string saleUniqueId = decodedHash.Split(';')[0];
            string sellerUniqueId = decodedHash.Split(';')[1];

            var sale = GetSale[saleUniqueId].Get();
            var seller = sale.Sellers.FirstOrDefault(r => r.UniqueId.ToString().ToLower() == sellerUniqueId.ToLower());

            if (sale.SaleStatus != Core.Domain.Sales.SaleStatus.PendingNew) return Redirect($"/account/sale/complete-documentation?hash={hash}");
            if (seller.PersonId.HasValue) return Redirect("/account/sales");

            var model = new CompleteSale
            {
                SaleGuid = decodedHash.Split(';')[0],
                SaleId = sale.SaleId,
                SaleSellerId = decodedHash.Split(';')[1],
                Name = GetTempData(seller.Tempdata).Split(';')[0],
                Surname = GetTempData(seller.Tempdata).Split(';')[1],
                Email = GetTempData(seller.Tempdata).Split(';')[2],
                Mobile = GetTempData(seller.Tempdata).Split(';')[3],
                IdNumber = seller.IdNumber,
                Address = sale.Address,
                StandErf = sale.StandErf,
                Township = sale.Township,
                SaleType = sale.SaleType
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Sales/CompleteRegistration", model);

            // Default
            return View("../Sales/CompleteRegistration", model);
        }

        [HttpPost, Route("account/sale/complete-sale")]
        [SessionFilter(Order = 3)]
        public ActionResult CompleteRegistration(CompleteSale model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
                    {
                        var sale = GetSale[model.SaleGuid].Get();
                        var seller = sale.Sellers.FirstOrDefault(r => r.UniqueId.ToString().ToLower() == model.SaleSellerId.ToLower());

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
                            .WithSecurityGroup("Seller")
                            .Create();

                            personId = CreatePerson.PersonId;
                            personGuid = CreatePerson.PersonGuid;
                        }

                        var success = false;

                        success = UpdateSaleService[model.SaleId]
                        .Set(a => a.Address, model.Address)
                        .Set(a => a.ModifiedOn, DateTime.Now)
                        .Set(a => a.StandErf, model.StandErf)
                        .Set(a => a.Township, model.Township)
                        .Update();

                        var physicalAddress = new Address()
                        {
                            Type = AddressType.Residential,
                            Address1 = model.ResidentialAddress1,
                            Address2 = model.ResidentialAddress2,
                            Address3 = model.ResidentialAddress3,
                            Code = model.ResidentialAddressCode,
                            Country = model.ResidentialAddressCountry,
                            RegionName = model.ResidentialAddressProvince,
                            Town = model.ResidentialAddressTown
                        };

                        var postalAddress = new Address()
                        {
                            Type = AddressType.Postal,
                            Address1 = model.PostalAddress1,
                            Address2 = model.PostalAddress2,
                            Address3 = model.PostalAddress3,
                            Code = model.PostalAddressCode,
                            Country = model.PostalAddressCountry,
                            RegionName = model.PostalAddressProvince,
                            Town = model.PostalAddressTown
                        };

                        //add the addresses
                        AddressRepo.Insert(physicalAddress);
                        AddressRepo.Insert(postalAddress);

                        success = UpdateSellerService[seller.SellerId]
                        .Set(x => x.PersonId, personId)
                        .Set(x => x.VatNumber, model.VatNumber)
                        .Update();

                        sale = GetSale[model.SaleGuid].Get();

                        var sendUpdatedSellerEmails = false;
                        if (sale.Sellers.All(l => l.PersonId.HasValue))
                        {
                            success = UpdateSaleService[model.SaleId].Set(a => a.SaleStatus, SaleStatus.PendingSellerDocumentation).Update();
                            sendUpdatedSellerEmails = true;
                        }

                        // Success
                        if (success)
                        {
                            //Auto login the new landlord
                            LoginService.WithGuid(personGuid).Login();

                            //complete the scope
                            transactionScope.Complete();

                            // Ajax (+ Json)
                            if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                            {
                                Success = true,
                                AgentId = 1,
                            }, JsonRequestBehavior.AllowGet);

                            //send the updated emails
                            if (sendUpdatedSellerEmails)
                            {
                                foreach (var ss in sale.Sellers)
                                {
                                    var email = GetTempData(ss.Tempdata).Split(';')[2];
                                    SendMail.WithUrlBody($"/account/sales/emails/sale-seller-documentation-email?saleId={sale.SaleId}&sellerId={ss.SellerId}").Send("Complete Sale Documentation", email);
                                }
                            }

                            // Default
                            return Redirect($"/account/sales");
                        }

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

            if (UpdateSaleService.HasWarnings) UpdateSaleService.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Sales/CompleteRegistration", model);

            // Default
            return View("../Sales/CompleteRegistration", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Sale";

            ViewBag.Titles = new List<string> { "Mr", "Miss", "Mrs", "Advocate", "Professor", "Doctor", "Other" };
            ViewBag.Genders = Enum.GetNames(typeof(Gender)).ToList();
            ViewBag.Provinces = new List<string> { "Eastern Cape", "Free State", "Gauteng", "KwaZulu Natal", "Limpopo", "Mpumalanga", "North West", "Northern Cape", "Western Cape" };
            ViewBag.Countries = CountryService.Get().Select(c => c.Name).ToList();
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
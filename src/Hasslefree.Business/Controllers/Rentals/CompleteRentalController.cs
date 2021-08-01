using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
    public class CompleteRentalController : BaseController
    {
        #region Private Properties 

        //Repos
        private IReadOnlyRepository<Rental> RentalRepo { get; }
        private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }

        // Services
        private IUpdateRentalService UpdateRentalService { get; }
        private ICreatePersonService CreatePerson { get; }
        private ILogoutService LogoutService { get; }
        private ICountryQueryService Countries { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalController
        (
            //Repos
            IReadOnlyRepository<Rental> rentalRepo,
            IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,

            //Services
            IUpdateRentalService updateRentalService,
            ICreatePersonService createPerson,
            ILogoutService logoutService,
            ICountryQueryService countries,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            //Repos
            RentalRepo = rentalRepo;
            RentalLandlordRepo = rentalLandlordRepo;

            // Services
            UpdateRentalService = updateRentalService;
            CreatePerson = createPerson;
            LogoutService = logoutService;
            Countries = countries;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rental/complete-rental")]
        public ActionResult CompleteRegistration(string id, string lid)
        {
            if (SessionManager.IsLoggedIn())
            {
                LogoutService.Logout();
                return Redirect($"/account/rental/complete-rental?id={id}&lid={lid}");
            }

            var rental = RentalRepo.Table.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == id.ToLower());
            var landlord = RentalLandlordRepo.Table.FirstOrDefault(r => r.UniqueId.ToString().ToLower() == lid.ToLower());

            var model = new CompleteRental
            {
                RentalGuid = id,
                RentalId = rental.RentalId,
                Name = GetTempData(landlord.Tempdata).Split(';')[0],
                Surname = GetTempData(landlord.Tempdata).Split(';')[1],
                Email = GetTempData(landlord.Tempdata).Split(';')[2],
                Mobile = GetTempData(landlord.Tempdata).Split(';')[3],
                IdNumber = landlord.IdNumber
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Agents/CompleteRegistration", model);

            // Default
            return View("../Rentals/CompleteRegistration", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Rental";
        }

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}
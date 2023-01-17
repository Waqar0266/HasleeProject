using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Landlords.Crud;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
    public class CompleteRentalDocumentationController : BaseController
    {
        #region Private Properties 

        // Services
        private IUpdateRentalService UpdateRentalService { get; }
        private ILogoutService LogoutService { get; }
        private IGetRentalService GetRental { get; }
        private ICreateLandlordDocumentationService CreateLandlordDocumentation { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalDocumentationController
        (
            //Services
            IUpdateRentalService updateRentalService,
            ILogoutService logoutService,
            IGetRentalService getRental,
            ICreateLandlordDocumentationService createLandlordDocumentation,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            // Services
            UpdateRentalService = updateRentalService;
            LogoutService = logoutService;
            GetRental = getRental;
            CreateLandlordDocumentation = createLandlordDocumentation;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rental/complete-documentation")]
        [AccessControlFilter]
        public ActionResult CompleteDocumentation(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalUniqueId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalUniqueId].Get();
            if (rental.RentalStatus != RentalStatus.PendingLandlordDocumentation) return Redirect($"/account/rental/l/complete-signature?hash={hash}");

            var model = new CompleteRentalLandlordDocumentation
            {
                RentalId = Int32.Parse(decodedHash.Split(';')[0]),
                LandlordGuid = decodedHash.Split(';')[1],
                DocumentsToUpload = GetDocumentsToUpload(rental)
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/CompleteDocumentation", model);

            // Default
            return View("../Rentals/CompleteDocumentation", model);
        }

        [HttpPost, Route("account/rental/complete-documentation")]
        [AccessControlFilter]
        public ActionResult CompleteDocumentation(CompleteRentalLandlordDocumentation model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var rental = GetRental[model.RentalId].Get();
                    var rentalLandlord = rental.RentalLandlords.FirstOrDefault(a => a.UniqueId.ToString().ToLower() == model.LandlordGuid.ToLower());

                    foreach (var i in model.UploadIds.Split(','))
                    {
                        if (Int32.TryParse(i, out int id))
                        {
                            if (id > 0) CreateLandlordDocumentation.Add(rentalLandlord.RentalLandlordId, id);
                        }
                    }

                    var success = CreateLandlordDocumentation.Process();

                    success = UpdateRentalService[rental.RentalId]
                    .Set(a => a.RentalStatus, RentalStatus.PendingLandlordSignature)
                    .Update();

                    // Success
                    if (success)
                    {
                        // Ajax (+ Json)
                        if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        {
                            Success = true,
                            AgentId = 1,
                        }, JsonRequestBehavior.AllowGet);

                        var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalId.ToString().ToLower()};{model.LandlordGuid.ToLower()}"));

                        // Default
                        return Redirect($"/account/rental/l/complete-signature?hash={hash}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            PrepViewBags();

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                //Message = CreateAgentService.Warnings.FirstOrDefault()?.Message ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rental/CompleteDocumentation", model);

            // Default
            return View("../Rental/CompleteDocumentation", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Landlord Documentation";
        }

        private List<string> GetDocumentsToUpload(RentalGet rental)
        {
            if (rental.LeaseType == LeaseType.Natural) return new List<string>() { "ID - Smart card ID (both sides)", "Proof of current address to be leased", "Proof of SARS income tax number" };
            if (rental.LeaseType == LeaseType.ClosedCorporation) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Members" };
            if (rental.LeaseType == LeaseType.Company) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Directors" };
            if (rental.LeaseType == LeaseType.Trust) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Trustees" };
            return new List<string>();
        }

        #endregion
    }
}
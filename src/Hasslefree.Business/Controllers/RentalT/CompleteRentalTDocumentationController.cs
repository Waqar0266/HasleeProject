using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Services.Tenants.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.RentalT
{
    public class CompleteRentalTDocumentationController : BaseController
    {
        #region Private Properties 

        // Services
        private IUpdateRentalTService UpdateRentalService { get; }
        private ILogoutService LogoutService { get; }
        private IGetRentalTService GetRental { get; }
        private ICreateTenantDocumentationService CreateTenantDocumentation { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalTDocumentationController
        (
            //Services
            IUpdateRentalTService updateRentalService,
            ILogoutService logoutService,
            IGetRentalTService getRental,
            ICreateTenantDocumentationService createTenantDocumentation,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            // Services
            UpdateRentalService = updateRentalService;
            LogoutService = logoutService;
            GetRental = getRental;
            CreateTenantDocumentation = createTenantDocumentation;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rentalt/complete-documentation")]
        [AccessControlFilter]
        public ActionResult CompleteDocumentation(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);
            int tenantId = Int32.Parse(decodedHash.Split(';')[1]);

            var rental = GetRental[rentalTId].Get();
            if (rental.Status != RentalTStatus.PendingTenantDocumentation) return Redirect($"/account/rental/l/complete-signature?hash={hash}");

            var model = new CompleteRentalTenantDocumentation
            {
                RentalTId = rentalTId,
                TenantId = tenantId,
                DocumentsToUpload = GetDocumentsToUpload(rental)
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteDocumentation", model);

            // Default
            return View("../Rentals/RentalTs/CompleteDocumentation", model);
        }

        [HttpPost, Route("account/rentalt/complete-documentation")]
        [AccessControlFilter]
        public ActionResult CompleteDocumentation(CompleteRentalTenantDocumentation model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var rental = GetRental[model.RentalTId].Get();
                    var rentalTenant = rental.Tenants.FirstOrDefault(a => a.TenantId == model.TenantId);

                    foreach (var i in model.UploadIds.Split(','))
                    {
                        if (Int32.TryParse(i, out int id))
                        {
                            if (id > 0) CreateTenantDocumentation.Add(rentalTenant.TenantId, id);
                        }
                    }

                    var success = CreateTenantDocumentation.Process();

                    success = UpdateRentalService[rental.RentalTId]
                    .Set(a => a.RentalTStatus, RentalTStatus.PendingTenantSignature)
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

                        var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rental.RentalTId};{model.TenantId}"));

                        // Default
                        return Redirect($"/account/rentalt/complete-signature?hash={hash}");
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
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rental/RentalTs/CompleteDocumentation", model);

            // Default
            return View("../Rental/RentalTs/CompleteDocumentation", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Tenant Documentation";
        }

        private List<string> GetDocumentsToUpload(RentalTGet rental)
        {
            if (rental.RentalTType == RentalTType.NaturalHolidayLease || rental.RentalTType == RentalTType.NaturalFixedTerm || rental.RentalTType == RentalTType.NaturalMonthToMonth || rental.RentalTType == RentalTType.NaturalStudentLease) return new List<string>() { "ID - Smart card ID (both sides)", "Proof of current address", "Latest 3 months consecutive payslips", "Latest 3 months consecutive bank statements", "Proof of SARS income tax number" };
            //if (rental.RentalTType == RentalTType.) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Members" };
            //if (rental.LeaseType == LeaseType.Company) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Directors" };
            //if (rental.LeaseType == LeaseType.Trust) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Trustees" };
            return new List<string>();
        }

        #endregion
    }
}
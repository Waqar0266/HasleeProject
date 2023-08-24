using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Emails;
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
        private ICreateTenantAgentDocumentationService CreateTenantAgentDocumentation { get; }
        private ISendMail SendMail { get; }

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
            ICreateTenantAgentDocumentationService createTenantAgentDocumentation,
            ISendMail sendMail,

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
            CreateTenantAgentDocumentation = createTenantAgentDocumentation;
            SendMail = sendMail;

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

        [HttpGet, Route("account/rentalt/complete-agent-documentation")]
        [AccessControlFilter(Roles = "Agent")]
        public ActionResult CompleteAgentDocumentation(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalTId].Get();
            if (rental.Status != RentalTStatus.PendingAgentDocumentation) return Redirect($"/account/tenants");

            var model = new CompleteRentalAgentDocumentation
            {
                RentalTId = rentalTId,
                DocumentsToUpload = GetDocumentsToUpload(rental)
            };

            PrepViewBags();

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteAgentDocumentation", model);

            // Default
            return View("../Rentals/RentalTs/CompleteAgentDocumentation", model);
        }

        [HttpPost, Route("account/rentalt/complete-agent-documentation")]
        [AccessControlFilter(Roles = "Agent")]
        public ActionResult CompleteAgentDocumentation(CompleteRentalAgentDocumentation model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var rental = GetRental[model.RentalTId].Get();

                    foreach (var i in model.UploadIds.Split(','))
                    {
                        if (Int32.TryParse(i, out int id))
                        {
                            if (id > 0) CreateTenantAgentDocumentation.Add(rental.RentalTId, rental.Rental.AgentId.Value, id);
                        }
                    }

                    var success = CreateTenantAgentDocumentation.Process();

                    success = UpdateRentalService[rental.RentalTId]
                    .Set(a => a.RentalTStatus, RentalTStatus.PendingLandlordApproval)
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

                        //Send Landlord emails
                        var landlords = rental.Rental.RentalLandlords.Select(l => new { Email = l.Person.Email, Id = l.RentalLandlordId }).ToList();
                        foreach (var landlord in landlords) SendLandlordEmail(landlord.Email, rental.RentalTId, landlord.Id);

                        // Default
                        return Redirect($"/account/tenants");
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
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rental/RentalTs/CompleteAgentDocumentation", model);

            // Default
            return View("../Rental/RentalTs/CompleteAgentDocumentation", model);
        }

        #endregion

        #region Private Methods

        private void PrepViewBags()
        {
            ViewBag.Title = "Complete Tenant Documentation";
        }

        private List<string> GetDocumentsToUpload(RentalTGet rental)
        {
            if (rental.RentalTType.ToString().ToLower().StartsWith("natural") && rental.Status == RentalTStatus.PendingTenantDocumentation) return new List<string>() { "ID - Smart card ID (both sides)", "Proof of current address", "Latest 3 months consecutive payslips", "Latest 3 months consecutive bank statements", "Proof of SARS income tax number" };
            if (rental.RentalTType.ToString().ToLower().StartsWith("natural") && rental.Status == RentalTStatus.PendingAgentDocumentation) return new List<string>() { "TPN Report", "Any other supporting documentaion" };
            //if (rental.RentalTType == RentalTType.) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Members" };
            //if (rental.LeaseType == LeaseType.Company) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Directors" };
            //if (rental.LeaseType == LeaseType.Trust) return new List<string>() { "Company registration document", "Proof of current address", "Proof of SARS income tax number", "Resolution of Trustees" };



            return new List<string>();
        }

        private bool SendLandlordEmail(string email, int rentalTId, int landlordId)
        {
            var url = $"account/rentals/emails/rental-tenant-landlord-approval-email?rentalTId={rentalTId}&landlordId={landlordId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("Pre-Approval Rental - Landlord Approval");
        }

        #endregion
    }
}
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Sessions;
using Hasslefree.Core;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hasslefree.Core.Logging;

namespace Hasslefree.Business.Controllers.RentalT
{
    public class CompleteRentalTApprovalController : BaseController
    {
        #region Private Properties 

        // Services
        private IUpdateRentalTService UpdateRentalService { get; }
        private IUpdateTenantService UpdateTenantService { get; }
        private IUploadPictureService UploadPicture { get; }
        private IUploadDownloadService UploadDownload { get; }
        private IGetFirmService GetFirmService { get; }
        private ILogoutService LogoutService { get; }
        private ISendMail SendMail { get; }
        private IGetRentalTService GetRental { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalTApprovalController
        (
            //Services
            IUpdateRentalTService updateRentalService,
            IUpdateTenantService updateTenantService,
            IUploadPictureService uploadPicture,
            IUploadDownloadService uploadDownload,
            IGetFirmService getFirmService,
            ILogoutService logoutService,
            ISendMail sendMail,
            IGetRentalTService getRental,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            // Services
            UpdateRentalService = updateRentalService;
            UpdateTenantService = updateTenantService;
            UploadPicture = uploadPicture;
            UploadDownload = uploadDownload;
            GetFirmService = getFirmService;
            LogoutService = logoutService;
            SendMail = sendMail;
            GetRental = getRental;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rentalt/approval")]
        [AccessControlFilter(Permission = "Landlord")]
        public ActionResult CompleteLandlordApproval(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalTId].Get();

            if (rental.Status != RentalTStatus.PendingLandlordApproval) return Redirect($"/account/tenants");

            var model = new CompleteRentalTLandlordApproval
            {
                RentalTId = rentalTId,
                Rental = rental,
                Hash = hash
            };

            ViewBag.Title = "Complete Rental Landlord Approval";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteLandlordApproval", model);

            // Default
            return View("../Rentals/RentalTs/CompleteLandlordApproval", model);
        }

        [HttpGet, Route("account/rentalt/approval-submit")]
        [AccessControlFilter(Permission = "Landlord")]
        public ActionResult CompleteLandlordApprovalSubmit(string hash, string action)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));
            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);
            var rental = GetRental[rentalTId].Get();

            if (rental.Status != RentalTStatus.PendingLandlordApproval) return Redirect($"/account/tenants");

            UpdateRentalService[rental.RentalTId].Set(a => a.RentalTStatus, RentalTStatus.PendingAgentApproval);

            if (action == "approved") UpdateRentalService.Set(x => x.LandlordApproved, true);

            UpdateRentalService.Update();

            SendAgentApprovalEmail(rental.Rental.Agent.Person.Email, rental.RentalTId);

            return Redirect($"/account/tenants");
        }

        [HttpGet, Route("account/rentalt/agent-approval")]
        [AccessControlFilter(Permission = "Agent")]
        public ActionResult CompleteAgentApproval(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalTId].Get();

            if (rental.Status != RentalTStatus.PendingAgentApproval) return Redirect($"/account/tenants");

            var model = new CompleteRentalTAgentApproval
            {
                RentalTId = rentalTId,
                Rental = rental,
                Hash = hash
            };

            ViewBag.Title = "Complete Rental Agent Approval";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteAgentApproval", model);

            // Default
            return View("../Rentals/RentalTs/CompleteAgentApproval", model);
        }

        [HttpGet, Route("account/rentalt/agent-approval-submit")]
        [AccessControlFilter(Permission = "Agent")]
        public ActionResult CompleteAgentApprovalSubmit(string hash, string action)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalTId].Get();

            if (rental.Status != RentalTStatus.PendingAgentApproval) return Redirect($"/account/tenants");

            var model = new CompleteRentalTAgentApproval
            {
                RentalTId = rentalTId,
                Rental = rental,
                Hash = hash
            };

            ViewBag.Title = "Complete Rental Agent Approval";

            if (action == "declined")
            {
                UpdateRentalService[rental.RentalTId].Set(a => a.RentalTStatus, RentalTStatus.PendingAgentApproval).Update();



                return Redirect($"/account/tenants");
            }

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteAgentApproval", model);

            // Default
            return View("../Rentals/RentalTs/CompleteAgentApproval", model);
        }

        #endregion

        #region Private Methods

        private bool SendAgentEmail(string email, int rentalTId)
        {
            var url = $"account/rentals/emails/rental-tenant-agent-documentation-email?rentalTId={rentalTId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("Pre-Approval Rental - Agent Documentation");
        }

        private bool SendAgentApprovalEmail(string email, int rentalTId)
        {
            var url = $"account/rentals/emails/rental-tenant-agent-approval-email?rentalTId={rentalTId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("Pre-Approval Rental - Agent Approval");
        }

        private bool SendTenantEmail(string email, int rentalTId, int tenantId)
        {
            var url = $"account/rentals/emails/rental-tenant-approval-email?rentalTId={rentalTId}&tenantId={tenantId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("Rental - Approval Result");
        }

        #endregion
    }
}
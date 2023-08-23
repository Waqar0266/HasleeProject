using Hasslefree.Core;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.RentalTs;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.RentalT
{
    public class RentalTEmailsController : BaseController
    {
        #region Private Properties 

        //Services
        private IGetRentalTService GetRentalT { get; }

        // Other
        private IWebHelper WebHelper { get; }

        #endregion

        #region Constructor

        public RentalTEmailsController
        (
            //Services
            IGetRentalTService getRentalT,

            //Other
            IWebHelper webHelper
        )
        {
            //Services
            GetRentalT = getRentalT;

            // Other
            WebHelper = webHelper;
        }

        #endregion

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/rentals/emails/rental-tenant-initial-email")]
        public ActionResult LandlordWitnessEmail(int rentalTId, int tenantId)
        {
            var rentalT = GetRentalT[rentalTId].Get();
            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rentalT.RentalTId};{tenantId}"));

            var model = new RentalTenantEmail()
            {
                Name = GetTempData(rentalT.Tenants.FirstOrDefault(t => t.TenantId == tenantId).Tempdata).Split(';')[0],
                Surname = GetTempData(rentalT.Tenants.FirstOrDefault(t => t.TenantId == tenantId).Tempdata).Split(';')[1],
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rentalt/complete-rental?hash={hash}",
                AgentName = rentalT.Rental.AgentPerson.FirstName,
                AgentSurname = rentalT.Rental.AgentPerson.Surname
            };

            return View("../Emails/Tenant-Initial-Email", model);
        }

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/rentals/emails/rental-tenant-agent-documentation-email")]
        public ActionResult AgentDocumentationEmail(int rentalTId)
        {
            var rentalT = GetRentalT[rentalTId].Get();
            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rentalT.RentalTId}"));

            var model = new RentalTAgentDocumentationEmail()
            {
                Name = rentalT.Rental.Agent.Person.FirstName,
                Surname = rentalT.Rental.Agent.Person.Surname,
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rentalt/complete-agent-documentation?hash={hash}"
            };

            return View("../Emails/Tenant-Agent-Documentation-Email", model);
        }

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/rentals/emails/rental-tenant-landlord-approval-email")]
        public ActionResult LandlordApprovalEmail(int rentalTId, int landlordId)
        {
            var rentalT = GetRentalT[rentalTId].Get();
            var landlord = rentalT.Rental.RentalLandlords.FirstOrDefault(x => x.RentalLandlordId == landlordId);
            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rentalT.RentalTId}"));

            var model = new RentalTLandlordApprovalEmail()
            {
                Name = landlord.Person.FirstName,
                Surname = landlord.Person.Surname,
                AgentName = rentalT.Rental.Agent.Person.FirstName,
                AgentSurname = rentalT.Rental.Agent.Person.Surname,
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rentalt/approval?hash={hash}"
            };

            return View("../Emails/Tenant-Landlord-Approval-Email", model);
        }

        [HttpGet]
        [Email]
        [AllowAnonymous]
        [Route("account/rentals/emails/rental-tenant-agent-approval-email")]
        public ActionResult AgentApprovalEmail(int rentalTId)
        {
            var rentalT = GetRentalT[rentalTId].Get();
            var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rentalT.RentalTId}"));

            var model = new RentalTAgentApprovalEmail()
            {
                Name = rentalT.Rental.Agent.Person.FirstName,
                Surname = rentalT.Rental.Agent.Person.Surname,
                Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rentalt/agent-approval?hash={hash}"
            };

            return View("../Emails/Tenant-Agent-Approval-Email", model);
        }

        #region Private Methods

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}
using Hasslefree.Core;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Annotations;
using Hasslefree.Web.Models.RentalTs;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
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

        #region Private Methods

        private string GetTempData(string tempData)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
        }

        #endregion
    }
}
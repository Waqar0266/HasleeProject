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
			var hash = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rentalT.RentalTGuid.ToString()};{tenantId}"));

			var model = new RentalTenantEmail()
			{
				//Name = rentalT.Tenants.FirstOrDefault(t=>t.TenantId == tenantId),
				//Surname = witnessNumber == 1 ? rental.RentalWitness.LandlordWitness1Surname : rental.RentalWitness.LandlordWitness2Surname,
				//Address = rental.Address,
				//StandErf = rental.StandErf,
				//ThePremises = rental.Premises,
				//Link = $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}/account/rental/l/complete-witness-signature?hash={hash}"
			};

			return View("../Emails/Tenant-Initial-Email", model);
		}
	}
}
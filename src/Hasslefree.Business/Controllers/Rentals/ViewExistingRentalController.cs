using Hasslefree.Core;
using Hasslefree.Services.Rentals.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Rentals
{
	[AccessControlFilter(Permission = "Director,Landlord,Agent")]
	public class ViewExistingRentalController : BaseController
	{
		#region Private Properties

		// Services
		private IGetExistingRentalService GetExistingRentalService { get; }

		// Other
		private IWebHelper WebHelper { get; }

		#endregion

		#region Constructor

		public ViewExistingRentalController
		(
			//Services
			IGetExistingRentalService getExistingRentalService,

			//Other
			IWebHelper webHelper
		)
		{
			// Services
			GetExistingRentalService = getExistingRentalService;

			// Other
			WebHelper = webHelper;
		}

		#endregion

		[HttpGet, Route("account/existing-rental")]
		public ActionResult View(int existingRentalId)
		{
			// Model
			var existingRental = GetExistingRentalService[existingRentalId].Get();

			if (existingRental == null)
			{
				if (WebHelper.IsAjaxRequest())
					return Json(new { }, JsonRequestBehavior.AllowGet);

				return RedirectToAction("Index", "ListRentals");
			}

			// View bags
			PrepViewBag();

			// Ajax
			if (WebHelper.IsAjaxRequest())
				return PartialView("../ExistingRentals/View", existingRental);

			// Default
			return View("../ExistingRentals/View", existingRental);
		}

		#region Private 

		/// <summary>
		/// Set required view bags for action and the crud title
		/// </summary>
		private void PrepViewBag()
		{
			// Set current crud title to display
			ViewBag.Title = "View Existing Rental";
		}

		#endregion
	}
}